using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LucybellVentas.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace LucybellVentas
{
    public partial class Form1 : Form
    {
        private List<Producto> productos = new List<Producto>();
        private List<Venta> ventas = new List<Venta>();

        public Form1()
        {
            InitializeComponent();
        }

        #region Botones

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            db.RegistrarVenta(txtNombreProducto.Text, Convert.ToInt32(nudCantidad.Text));
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            db.AgregarProducto(txtNombre2.Text, Convert.ToDecimal(txtPrecio.Text), Convert.ToInt32(txtStock.Text));
        }

        #endregion

        #region Metodos

        private void ActualizarResumenVentas()
        {
            
        }

        private void ActualizarStockDisponible()
        {
            
        }

        private void CalcularTotalVenta()
        {
           
        }

        private void nudCantidad_ValueChanged(object sender, EventArgs e)
        {
            CalcularTotalVenta();
        }

        private void txtNombreProducto_TextChanged(object sender, EventArgs e)
        {
            ActualizarStockDisponible();
            CalcularTotalVenta();
        }

        #endregion

        #region SQL Conexion y Funciones
        public class DatabaseHelper
        {
            public string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

            public void AgregarProducto(string nombre, decimal precio, int stock)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Producto agregado correctamente.");
                }
            }

            public void RegistrarVenta(string nombreProducto, int cantidad)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // Buscar el id_producto con el nombre del producto
                        string queryProducto = "SELECT id_producto, stock FROM Productos WHERE nombre = @nombreProducto";
                        SqlCommand cmdProducto = new SqlCommand(queryProducto, con, transaction);
                        cmdProducto.Parameters.AddWithValue("@nombreProducto", nombreProducto);

                        int idProducto = 0;
                        int stockDisponible = 0;

                        using (SqlDataReader reader = cmdProducto.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Producto no encontrado.");
                                return;
                            }

                            idProducto = reader.GetInt32(0);
                            stockDisponible = reader.GetInt32(1);
                        } // **Aquí se cierra automáticamente el DataReader**

                        // Verificar si hay suficiente stock
                        if (stockDisponible < cantidad)
                        {
                            MessageBox.Show("No hay suficiente stock para realizar la venta.");
                            return;
                        }

                        // Crear la venta
                        string queryVenta = "INSERT INTO Ventas DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                        SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaction);
                        int idVenta = Convert.ToInt32(cmdVenta.ExecuteScalar());

                        // Obtener el precio del producto
                        string queryPrecio = "SELECT precio FROM Productos WHERE id_producto = @idProducto";
                        SqlCommand cmdPrecio = new SqlCommand(queryPrecio, con, transaction);
                        cmdPrecio.Parameters.AddWithValue("@idProducto", idProducto);
                        decimal precio = Convert.ToDecimal(cmdPrecio.ExecuteScalar());

                        // Insertar detalle de la venta
                        string queryDetalle = "INSERT INTO DetallesVenta (id_venta, id_producto, cantidad, precio_unitario, total) VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @total)";
                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaction);
                        cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdDetalle.Parameters.AddWithValue("@precioUnitario", precio);
                        cmdDetalle.Parameters.AddWithValue("@total", cantidad * precio);
                        cmdDetalle.ExecuteNonQuery();

                        // Reducir stock
                        string queryStock = "UPDATE Productos SET stock = stock - @cantidad WHERE id_producto = @idProducto";
                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaction);
                        cmdStock.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdStock.ExecuteNonQuery();

                        // Confirmar transacción
                        transaction.Commit();
                        MessageBox.Show("Venta registrada correctamente.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error al registrar la venta: " + ex.Message);
                    }
                }
            }

        }
        #endregion

      
    }
}
