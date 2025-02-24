using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Modelos;

namespace BackEnd
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

        public void AgregarProducto(Producto producto)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", producto.nombre);
                cmd.Parameters.AddWithValue("@precio", producto.precio);
                cmd.Parameters.AddWithValue("@stock", producto.stock);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public bool VerifProducto(string nombre)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open(); // La conexión debe abrirse antes de ejecutar la consulta

                // Verificar si el producto ya existe
                string checkQuery = "SELECT COUNT(*) FROM Productos WHERE nombre = @nombre";
                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    int count = (int)cmd.ExecuteScalar(); // Obtener el resultado de la consulta
                    return count > 0; // Retorna true si el producto existe, false si no
                }
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
                    string queryProducto = "SELECT id_producto, stock, precio FROM Productos WHERE nombre = @nombreProducto";
                    SqlCommand cmdProducto = new SqlCommand(queryProducto, con, transaction);
                    cmdProducto.Parameters.AddWithValue("@nombreProducto", nombreProducto);

                    int idProducto = 0, stockDisponible = 0;
                    decimal precio = 0;

                    using (SqlDataReader reader = cmdProducto.ExecuteReader())
                    {
                        if (!reader.Read()) throw new Exception("Producto no encontrado.");
                        idProducto = reader.GetInt32(0);
                        stockDisponible = reader.GetInt32(1);
                        precio = reader.GetDecimal(2);
                    }

                    if (stockDisponible < cantidad)
                        throw new Exception("Stock insuficiente.");

                    string queryVenta = "INSERT INTO Ventas DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaction);
                    int idVenta = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    string queryDetalle = "INSERT INTO DetallesVenta (id_venta, id_producto, cantidad, precio_unitario, total) VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @total)";
                    SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaction);
                    cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                    cmdDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                    cmdDetalle.Parameters.AddWithValue("@precioUnitario", precio);
                    cmdDetalle.Parameters.AddWithValue("@total", cantidad * precio);
                    cmdDetalle.ExecuteNonQuery();

                    string queryStock = "UPDATE Productos SET stock = stock - @cantidad WHERE id_producto = @idProducto";
                    SqlCommand cmdStock = new SqlCommand(queryStock, con, transaction);
                    cmdStock.Parameters.AddWithValue("@cantidad", cantidad);
                    cmdStock.Parameters.AddWithValue("@idProducto", idProducto);
                    cmdStock.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al registrar la venta: " + ex.Message);
                }
            }
        }

        public List<string> BuscarProductos(string filtro)
        {
            List<string> productos = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT nombre FROM productos WHERE nombre LIKE @filtro";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(reader["nombre"].ToString());
                    }
                }
            }

            return productos;
        }

        public List<String> VerProductos() 
        {
            List<string> productos = new List<string>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT nombre FROM productos";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(reader["nombre"].ToString());
                    }
                }
            }

            return productos;
        } 

        public Producto ObtenerInfoProducto(string nombre)
        {
            Producto producto = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT id_producto, nombre, precio, stock FROM productos WHERE nombre = @nombre";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", nombre);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        producto = new Producto
                        {
                            id_producto = Convert.ToInt32(reader["id_producto"]),
                            nombre = reader["nombre"].ToString(),
                            precio = Convert.ToDecimal(reader["precio"]),
                            stock = Convert.ToInt32(reader["stock"])
                        };
                    }
                }
            }

            return producto;
        }

        public bool EliminarProducto(int idProducto)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Iniciar una transacción
                SqlTransaction transaction = null;

                // Consultas
                string queryActualizarDetalleVentas = "UPDATE DetalleVentas SET id_producto = NULL WHERE id_producto = @idProducto";
                string queryEliminarProducto = "DELETE FROM productos WHERE id_producto = @id";

                SqlCommand cmdActualizarDetalleVentas = new SqlCommand(queryActualizarDetalleVentas, con);
                cmdActualizarDetalleVentas.Parameters.AddWithValue("@idProducto", idProducto);

                SqlCommand cmdEliminarProducto = new SqlCommand(queryEliminarProducto, con);
                cmdEliminarProducto.Parameters.AddWithValue("@id", idProducto);

                try
                {
                    con.Open();

                    // Iniciar la transacción
                    transaction = con.BeginTransaction();

                    // Asignar la transacción a los comandos
                    cmdActualizarDetalleVentas.Transaction = transaction;
                    cmdEliminarProducto.Transaction = transaction;

                    // Ejecutar primero la actualización de DetalleVentas
                    cmdActualizarDetalleVentas.ExecuteNonQuery();

                    // Luego eliminar el producto de Productos
                    int rowsAffected = cmdEliminarProducto.ExecuteNonQuery();

                    // Si se eliminaron filas, hacer commit, si no, hacer rollback
                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
                catch (Exception)
                {
                    // En caso de error, hacer rollback
                    transaction?.Rollback();
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public DataTable ObtenerVentasDelDia()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT p.nombre AS 'Producto', p.precio AS 'Precio Unitario', " +
                                   "dv.cantidad AS 'Cantidad', dv.total AS 'Subtotal', v.Estado, v.id_venta " +
                                   "FROM Productos p " +
                                   "JOIN DetallesVenta dv ON p.id_producto = dv.id_producto " +
                                   "JOIN Ventas v ON dv.id_venta = v.id_venta " +
                                   "WHERE CAST(v.fecha AS DATE) = CAST(GETDATE() AS DATE);";

                    using (SqlDataAdapter adapt = new SqlDataAdapter(query, con))
                    {
                        adapt.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ventas: " + ex.Message);
            }
            return dt;
        }

        public bool AnularVenta(int idVenta)
        {
            bool ventaSuspendida = false;
            string query = "UPDATE Ventas SET Estado = 'Suspendida' WHERE id_venta = @idVenta AND Estado = 'Completada'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idVenta", idVenta);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Si se actualizó la venta correctamente
                    if (rowsAffected > 0)
                    {
                        ventaSuspendida = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al anular la venta: " + ex.Message);
            }

            return ventaSuspendida;
        }

        public void DevolverStock(int idDetallesVenta, int idproducto)
        {

        }

        public int EditarProducto(Producto producto, string nombreOriginal)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Productos SET nombre = @nuevoNombre, precio = @precio, stock = @stock WHERE nombre = @nombreOriginal";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nuevoNombre", producto.nombre);
                    cmd.Parameters.AddWithValue("@precio", producto.precio);
                    cmd.Parameters.AddWithValue("@stock", producto.stock);
                    cmd.Parameters.AddWithValue("@nombreOriginal", nombreOriginal);

                    con.Open(); // ✅ Primero abrimos la conexión
                    rowsAffected = cmd.ExecuteNonQuery(); // ✅ Ejecutamos solo una vez
                    con.Close();
                }
            }

            return rowsAffected; // ✅ Devolvemos la cantidad de filas afectadas
        }

    }
}
