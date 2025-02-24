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
                try
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        // Actualizar DetalleVentas para desvincular el producto
                        string queryActualizarDetalleVentas = "UPDATE DetallesVenta SET id_producto = NULL WHERE id_producto = @idProducto";
                        using (SqlCommand cmdActualizarDetalleVentas = new SqlCommand(queryActualizarDetalleVentas, con, transaction))
                        {
                            cmdActualizarDetalleVentas.Parameters.AddWithValue("@idProducto", idProducto);
                            cmdActualizarDetalleVentas.ExecuteNonQuery();
                        }

                        // Eliminar producto de la tabla productos
                        string queryEliminarProducto = "DELETE FROM productos WHERE id_producto = @idProducto";
                        using (SqlCommand cmdEliminarProducto = new SqlCommand(queryEliminarProducto, con, transaction))
                        {
                            cmdEliminarProducto.Parameters.AddWithValue("@idProducto", idProducto);
                            int rowsAffected = cmdEliminarProducto.ExecuteNonQuery();

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
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
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
                    string query = "SELECT v.Fecha AS 'Fecha y hora', p.nombre AS 'Producto', p.precio AS 'Precio Unitario', " +
                                   "dv.cantidad AS 'Cantidad', dv.total AS 'Subtotal', v.Estado, v.id_venta " +
                                   "FROM Productos p " +
                                   "JOIN DetallesVenta dv ON p.id_producto = dv.id_producto " +
                                   "JOIN Ventas v ON dv.id_venta = v.id_venta";

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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // 1. Cambiar el estado de la venta a 'Suspendida'
                    string queryAnularVenta = "UPDATE Ventas SET Estado = 'Suspendida' WHERE id_venta = @idVenta AND Estado = 'Completada'";
                    SqlCommand cmdAnularVenta = new SqlCommand(queryAnularVenta, conn, transaction);
                    cmdAnularVenta.Parameters.AddWithValue("@idVenta", idVenta);
                    int rowsAffected = cmdAnularVenta.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // No se encontró la venta o ya estaba suspendida
                        transaction.Rollback();
                        return false;
                    }

                    // 2. Obtener los productos y cantidades de la venta
                    string queryObtenerProductos = "SELECT id_producto, cantidad FROM DetallesVenta WHERE id_venta = @idVenta";
                    SqlCommand cmdObtenerProductos = new SqlCommand(queryObtenerProductos, conn, transaction);
                    cmdObtenerProductos.Parameters.AddWithValue("@idVenta", idVenta);

                    List<(int idProducto, int cantidad)> productos = new List<(int, int)>();
                    using (SqlDataReader reader = cmdObtenerProductos.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add((reader.GetInt32(0), reader.GetInt32(1)));
                        }
                    }

                    // 3. Devolver el stock a los productos
                    foreach (var (idProducto, cantidad) in productos)
                    {
                        string queryActualizarStock = "UPDATE Productos SET stock = stock + @cantidad WHERE id_producto = @idProducto";
                        SqlCommand cmdActualizarStock = new SqlCommand(queryActualizarStock, conn, transaction);
                        cmdActualizarStock.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdActualizarStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarStock.ExecuteNonQuery();
                    }

                    // 4. Si todo salió bien, hacer commit
                    transaction.Commit();
                    ventaSuspendida = true;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    throw new Exception("Error al anular la venta: " + ex.Message);
                }
            }

            return ventaSuspendida;
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

        public DataTable ObtenerVentasPorFecha(DateTime fecha)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";
            string query = @"SELECT V.fecha AS 'Fecha y hora', P.nombre AS Producto, DV.cantidad AS 'Cantidad', DV.precio_unitario AS 'Precio Unitario', DV.total AS 'Subtotal', V.Estado
                     FROM Ventas V
                     INNER JOIN DetallesVenta DV ON V.id_venta = DV.id_venta
                     INNER JOIN Productos P ON DV.id_producto = P.id_producto
                     WHERE CONVERT(date, V.fecha) = @fecha";

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fecha", fecha.Date); // Solo la fecha, sin hora
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable ObtenerTodasLasVentas()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";
            string query = @"SELECT V.fecha AS 'Fecha y hora', P.nombre AS Producto, DV.cantidad AS 'Cantidad', 
                            DV.precio_unitario AS 'Precio Unitario', DV.total AS 'Subtotal', V.Estado
                     FROM Ventas V
                     INNER JOIN DetallesVenta DV ON V.id_venta = DV.id_venta
                     INNER JOIN Productos P ON DV.id_producto = P.id_producto";

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }


    }
}
