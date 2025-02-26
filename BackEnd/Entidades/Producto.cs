using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BackEnd.Modelos
{
    public class Producto
    {
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }

        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

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
    }
}