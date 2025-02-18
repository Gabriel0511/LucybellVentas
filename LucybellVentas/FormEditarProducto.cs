using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LucybellVentas
{
    public partial class FormEditarProducto : Form
    {
        private string nombreProducto;

        public FormEditarProducto(string nombre)
        {
            InitializeComponent();
            nombreProducto = nombre;
        }

        private void FormEditarProducto_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";
        
            using (SqlConnection con =  new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT precio, stock FROM Productos WHERE nombre = @nombre";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nombre", nombreProducto);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtNombre.Text = nombreProducto;
                        txtPrecio.Text = reader["precio"].ToString();
                        txtStock.Text = reader["stock"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Producto no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE Productos SET nombre = @nuevoNombre, precio = @precio, stock = @stock WHERE nombre = @nombreOriginal";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nuevoNombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@precio", Convert.ToDecimal(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(txtStock.Text));
                    cmd.Parameters.AddWithValue("@nombreOriginal", nombreProducto);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se encontró el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
    }
}
