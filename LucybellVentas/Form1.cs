using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using BackEnd.Modelos;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using LucybellVentas;


namespace FrontEnd
{
    public partial class Form1 : Form
    {
        private List<Producto> productos = new List<Producto>();
        private List<Venta> ventas = new List<Venta>();

        SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;");
        SqlDataAdapter adapt;

        public Form1()
        {
            InitializeComponent();
            VerVentas();
        }

        #region TextBox

        private void nudCantidad_ValueChanged(object sender, EventArgs e)
        {
            CalcularSubtotal();
        }

        private void txtNombreProducto_TextChanged(object sender, EventArgs e)
        {
            ActualizarInfoProducto();
            CargarAutocompletado();
        }

        #endregion

        #region Botones

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            db.RegistrarVenta(txtNombreProducto.Text, Convert.ToInt32(nudCantidad.Text));
            VerVentas();

            //limpiar textbox
            txtNombreProducto.Clear();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FormAgregarProducto formAgregar = new FormAgregarProducto();
            formAgregar.ShowDialog();
        }



        #endregion

        #region Metodos

        private void VerVentas()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;"))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapt = new SqlDataAdapter(
                        "SELECT p.nombre AS 'Producto', p.precio AS 'Precio Unitario', " +
                        "dv.cantidad AS 'Cantidad', dv.total AS 'Total Venta' " +
                        "FROM Productos p " +
                        "JOIN DetallesVenta dv ON p.id_producto = dv.id_producto " +
                        "JOIN Ventas v ON dv.id_venta = v.id_venta " +
                        "WHERE CAST(v.fecha AS DATE) = CAST(GETDATE() AS DATE);", con))
                    {
                        adapt.Fill(dt);
                    }
                    dgvResumenVentas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las ventas: " + ex.Message);
            }
        }


        private void ActualizarInfoProducto()
        {
            con.Open();
            string query = "SELECT nombre, precio, stock FROM productos WHERE nombre LIKE @nombre";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@nombre", txtNombreProducto.Text);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int totalStock = 0;
                    decimal totalPrecio = 0;

                    while (reader.Read())
                    {
                        string nombre = reader["nombre"].ToString();
                        decimal precio = Convert.ToDecimal(reader["precio"]);
                        int stock = Convert.ToInt32(reader["stock"]);

                        totalStock += stock;
                        totalPrecio += precio;
                    }
                    lblStockDisponible.Text = "Stock : " + totalStock.ToString();
                    lblPrecioUnitario.Text = "Precio : " + totalPrecio.ToString();


                    if (lblStockDisponible.Text != "Stock : 0")
                    {
                        nudCantidad.Value = 1;
                        nudCantidad.Enabled = true;

                    }
                    else
                    {
                        nudCantidad.Value = 0;
                        nudCantidad.Enabled = false;
                    }

                }
            }
            con.Close();

        }

        private void CargarAutocompletado()
        {
            AutoCompleteStringCollection nombresProductos = new AutoCompleteStringCollection();
            con.Open();
            string query = "SELECT nombre FROM productos";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        nombresProductos.Add(reader["nombre"].ToString());
                    }
                }
            }

            txtNombreProducto.AutoCompleteCustomSource = nombresProductos;
            con.Close();
        }

        private void CalcularSubtotal()
        {

            if (string.IsNullOrWhiteSpace(lblPrecioUnitario.Text))
            {
                MessageBox.Show("El precio unitario no está disponible.");
                return;
            }

            decimal precioUnitario = 0;

            // Intentar convertir el texto del Label a decimal
            if (!decimal.TryParse(lblPrecioUnitario.Text.Replace("Precio : ", ""), out precioUnitario))
            {
                MessageBox.Show("Error: Precio unitario no válido.");
                return;
            }

            decimal cantidad = nudCantidad.Value; // Ya es decimal, no necesita conversión
            decimal subtotal = precioUnitario * cantidad;

            lblSubtotal.Text = "Subtotal : " + subtotal.ToString("0.00"); // Mostrar con formato adecuado
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
                        if (stockDisponible == 0)
                        {
                            MessageBox.Show("Error: No hay stock disponible para este producto.", "Stock agotado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (stockDisponible < cantidad)
                        {
                            MessageBox.Show($"Error: Stock insuficiente. Solo hay {stockDisponible} unidades disponibles.", "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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