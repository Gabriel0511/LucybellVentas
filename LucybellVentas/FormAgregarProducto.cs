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
using System.Runtime.InteropServices;

namespace LucybellVentas
{
    public partial class FormAgregarProducto : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWndn, int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public FormAgregarProducto()
        {
            InitializeComponent();
            SendMessage(txtNombre.Handle, EM_SETCUEBANNER, 0, "Ingrese el nombre del producto");
            SendMessage(txtPrecio.Handle, EM_SETCUEBANNER, 0, "Ingrese el precio del producto");
            SendMessage(txtStock.Handle, EM_SETCUEBANNER, 0, "Ingrese el stock del producto");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Verificar si el producto ya existe
                    string checkQuery = "SELECT COUNT(*) FROM Productos WHERE nombre = @nombre";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Ya existe un producto con este nombre.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insertar nuevo producto
                    string insertQuery = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    insertCmd.Parameters.AddWithValue("@precio", Convert.ToDecimal(txtPrecio.Text));
                    insertCmd.Parameters.AddWithValue("@stock", Convert.ToInt32(txtStock.Text));

                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("Producto agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
