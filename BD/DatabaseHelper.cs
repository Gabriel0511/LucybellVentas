using BackEnd.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BD
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

        // Método genérico para ejecutar consultas que devuelven un DataTable
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // Método genérico para ejecutar consultas que no devuelven resultados
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Método genérico para ejecutar consultas que devuelven un solo valor
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        // Método para manejar transacciones
        public void ExecuteTransaction(Action<SqlTransaction> action)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    action(transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error en la transacción: " + ex.Message);
                }
            }
        }

        // Método para obtener las ventas del día
        public DataTable ObtenerVentasDelDia()
        {
            string query = "SELECT p.nombre AS 'Producto', p.precio AS 'Precio Unitario', " +
                          "dv.cantidad AS 'Cantidad', dv.total AS 'Subtotal', v.Estado, v.id_venta " +
                          "FROM Productos p " +
                          "JOIN DetallesVenta dv ON p.id_producto = dv.id_producto " +
                          "JOIN Ventas v ON dv.id_venta = v.id_venta " +
                          "WHERE CAST(v.fecha AS DATE) = CAST(GETDATE() AS DATE);";
            return ExecuteQuery(query);
        }

        // Método para registrar una venta
        public void RegistrarVenta(string nombreProducto, int cantidad)
        {
            ExecuteTransaction(transaction =>
            {
                // Buscar el id_producto con el nombre del producto
                string queryProducto = "SELECT id_producto, stock FROM Productos WHERE nombre = @nombreProducto";
                using (SqlCommand cmdProducto = new SqlCommand(queryProducto, transaction.Connection, transaction))
                {
                    cmdProducto.Parameters.AddWithValue("@nombreProducto", nombreProducto);
                    using (SqlDataReader reader = cmdProducto.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception("Producto no encontrado.");
                        }

                        int idProducto = reader.GetInt32(0);
                        int stockDisponible = reader.GetInt32(1);

                        // Verificar si hay suficiente stock
                        if (stockDisponible == 0)
                        {
                            throw new Exception("No hay stock disponible para este producto.");
                        }
                        if (stockDisponible < cantidad)
                        {
                            throw new Exception($"Stock insuficiente. Solo hay {stockDisponible} unidades disponibles.");
                        }

                        // Crear la venta
                        string queryVenta = "INSERT INTO Ventas DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmdVenta = new SqlCommand(queryVenta, transaction.Connection, transaction))
                        {
                            int idVenta = Convert.ToInt32(cmdVenta.ExecuteScalar());

                            // Obtener el precio del producto
                            string queryPrecio = "SELECT precio FROM Productos WHERE id_producto = @idProducto";
                            using (SqlCommand cmdPrecio = new SqlCommand(queryPrecio, transaction.Connection, transaction))
                            {
                                cmdPrecio.Parameters.AddWithValue("@idProducto", idProducto);
                                decimal precio = Convert.ToDecimal(cmdPrecio.ExecuteScalar());

                                // Insertar detalle de la venta
                                string queryDetalle = "INSERT INTO DetallesVenta (id_venta, id_producto, cantidad, precio_unitario, total) VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @total)";
                                using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, transaction.Connection, transaction))
                                {
                                    cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                                    cmdDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                                    cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                                    cmdDetalle.Parameters.AddWithValue("@precioUnitario", precio);
                                    cmdDetalle.Parameters.AddWithValue("@total", cantidad * precio);
                                    cmdDetalle.ExecuteNonQuery();
                                }

                                // Reducir stock
                                string queryStock = "UPDATE Productos SET stock = stock - @cantidad WHERE id_producto = @idProducto";
                                using (SqlCommand cmdStock = new SqlCommand(queryStock, transaction.Connection, transaction))
                                {
                                    cmdStock.Parameters.AddWithValue("@cantidad", cantidad);
                                    cmdStock.Parameters.AddWithValue("@idProducto", idProducto);
                                    cmdStock.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            });
        }

        // Método para agregar un producto
        public void AgregarProducto(string nombre, decimal precio, int stock)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.");
            }
            if (precio <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor que cero.");
            }
            if (stock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo.");
            }

            string query = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";
            SqlParameter[] parameters = {
                new SqlParameter("@nombre", nombre),
                new SqlParameter("@precio", precio),
                new SqlParameter("@stock", stock)
            };

            ExecuteNonQuery(query, parameters);
        }

        // Método para obtener la información de un producto por su nombre
        public Producto ObtenerInfoProducto(string nombreProducto)
        {
            string query = "SELECT id_producto, nombre, precio, stock FROM productos WHERE nombre = @nombre";
            SqlParameter[] parameters = {
                new SqlParameter("@nombre", nombreProducto)
            };

            DataTable dt = ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Producto
                {
                    id_producto = Convert.ToInt32(row["id_producto"]),
                    nombre = row["nombre"].ToString(),
                    precio = Convert.ToDecimal(row["precio"]),
                    stock = Convert.ToInt32(row["stock"])
                };
            }
            return null;
        }

        // Método para obtener los nombres de los productos que coinciden con un filtro
        public List<string> ObtenerNombresProductos(string filtro)
        {
            string query = "SELECT nombre FROM productos WHERE nombre LIKE @filtro";
            SqlParameter[] parameters = {
                new SqlParameter("@filtro", $"%{filtro}%")
            };

            DataTable dt = ExecuteQuery(query, parameters);
            List<string> nombres = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                nombres.Add(row["nombre"].ToString());
            }
            return nombres;
        }

        // Método para eliminar un producto
        public bool EliminarProducto(int idProducto)
        {
            string query = "DELETE FROM productos WHERE id_producto = @id";
            SqlParameter[] parameters = {
                new SqlParameter("@id", idProducto)
            };

            int rowsAffected = ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        // Método para anular una venta
        public bool AnularVenta(int idVenta)
        {
            string query = "UPDATE Ventas SET Estado = 'Suspendida' WHERE id_venta = @idVenta AND Estado = 'Completada'";
            SqlParameter[] parameters = {
                new SqlParameter("@idVenta", idVenta)
            };

            int rowsAffected = ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }
    }
}