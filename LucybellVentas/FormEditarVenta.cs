using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LucybellVentas
{
    public partial class FormEditarVenta: Form
    {
        public FormEditarVenta()
        {
            InitializeComponent();

            this.Shown += (s, e) => btnEditar.Focus();

        }

    }
}
