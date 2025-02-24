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
using BackEnd;
using BackEnd.Modelos;

namespace LucybellVentas
{
    public partial class FormAgregarProducto : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWndn, int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public FormAgregarProducto()
        {
            InitializeComponent();
            SendMessage(txtNombre.Handle, EM_SETCUEBANNER, 0, "Ingrese el nombre del producto");
            SendMessage(txtPrecio.Handle, EM_SETCUEBANNER, 0, "Ingrese el precio del producto");
            SendMessage(txtStock.Handle, EM_SETCUEBANNER, 0, "Ingrese el stock del producto");

            this.Shown += (s, e) => btnGuardar.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombreProducto = txtNombre.Text.Trim();
            string precioProducto = txtPrecio.Text.Trim();
            string stockProducto = txtStock.Text.Trim();

            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(nombreProducto) || string.IsNullOrWhiteSpace(precioProducto) || string.IsNullOrWhiteSpace(stockProducto))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar si el producto ya existe
            if (dbHelper.VerifProducto(nombreProducto))
            {
                MessageBox.Show("El producto ya existe en la base de datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Convertir los valores ingresados
                if (!decimal.TryParse(precioProducto, out decimal precio) || !int.TryParse(stockProducto, out int stock))
                {
                    MessageBox.Show("El precio y el stock deben ser valores numéricos válidos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear el producto con los valores ingresados
                Producto producto = new Producto
                {
                    nombre = nombreProducto,
                    precio = precio,
                    stock = stock
                };

                // Agregar el producto a la base de datos
                dbHelper.AgregarProducto(producto);

                MessageBox.Show("Producto agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar los campos después de guardar
                txtNombre.Clear();
                txtPrecio.Clear();
                txtStock.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
