using FrontEnd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LucybellVentas
{
    public partial class FormInicioSesion : Form
    {
        public FormInicioSesion()
        {
            InitializeComponent();
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contrasena = txtContrasena.Text;

            // Validación de credenciales (ejemplo básico)
            if (usuario == "ADM" && contrasena == "1234")
            {
                this.Hide();
                Form1 Formm = new Form1(); // Asumiendo que hay otro formulario principal
                // Muestra el formulario principal como modal, bloqueando el acceso al formulario de inicio de sesión
                // Después de cerrar Form1, puedes decidir si hacer algo más en el programa.
                Formm.ShowDialog();
                this.Close(); // Cierra el formulario de inicio de sesión
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
