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
using BackEnd;
using BackEnd.Modelos;
using FrontEnd;

namespace LucybellVentas
{
    public partial class FormEditarProducto : Form
    {
        private Producto producto;
        private string nombreOriginal;
        private DatabaseHelper dbHelper = new DatabaseHelper();
        
        public FormEditarProducto(string nombre)
        {           
            InitializeComponent();
            nombreOriginal = nombre;
        }

        public void FormEditarProducto_Load(object sender, EventArgs e)
        {
            // Obtener la información del producto
            producto = dbHelper.ObtenerInfoProducto(nombreOriginal);

            if (producto != null) // Verificamos que el producto exista
            {
                txtNombre.Text = producto.nombre;
                txtPrecio.Text = producto.precio.ToString();
                txtStock.Text = producto.stock.ToString();
            }
            else
            {
                MessageBox.Show("No se encontró el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }


        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                    string.IsNullOrWhiteSpace(txtStock.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Intentar convertir los valores a los tipos adecuados
                if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un número válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtStock.Text.Trim(), out int stock))
                {
                    MessageBox.Show("El stock debe ser un número entero válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear objeto producto con los valores validados
                Producto producto = new Producto
                {
                    nombre = txtNombre.Text.Trim(),
                    precio = precio,
                    stock = stock
                };

                // Llamar al método de actualización
                int rowsAffected = dbHelper.EditarProducto(producto, nombreOriginal);

                this.Close();

                Form1 form1 = (Form1)Application.OpenForms["Form1"];
                if (form1 != null)
                {
                    form1.TextoNombre = producto.nombre;
                    form1.ListBoxProductos.Visible = false;
                }


                // Verificar si se realizó la actualización
                if (rowsAffected > 0)
                    MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("No se encontró el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
