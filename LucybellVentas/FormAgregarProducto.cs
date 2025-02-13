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
    public partial class FormAgregarProducto : Form
    {
        public FormAgregarProducto()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@precio", Convert.ToDecimal(txtPrecio.Text));
                cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(txtStock.Text));

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Producto agregado correctamente.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
