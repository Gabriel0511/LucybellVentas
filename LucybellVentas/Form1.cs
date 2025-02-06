using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LucybellVentas.Modelos;

namespace LucybellVentas
{
    public partial class Form1 : Form
    {
        private List<Producto> productos = new List<Producto>();
        private List<Venta> ventas = new List<Venta>();

        public Form1()
        {
            InitializeComponent();
            productos.Add(new Producto { Nombre = "Pulsera Plata", Stock = 10, PrecioUnitario = 50.00m });
            productos.Add(new Producto { Nombre = "Collar Oro", Stock = 5, PrecioUnitario = 100.00m });

            // Configurar autocompletado
            txtNombreProducto.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtNombreProducto.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // Crear una lista de nombres de productos para el autocompletado
            var nombresProductos = productos.Select(p => p.Nombre).ToArray();
            txtNombreProducto.AutoCompleteCustomSource.AddRange(nombresProductos);
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreProducto.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre del producto.");
                return;
            }

            if (nudCantidad.Value <= 0)
            {
                MessageBox.Show("La cantidad vendida debe ser mayor que 0.");
                return;
            }

            string nombreProducto = txtNombreProducto.Text;
            int cantidadVendida = (int)nudCantidad.Value;

            Producto producto = productos.FirstOrDefault(p => p.Nombre == nombreProducto);
            if (producto != null && producto.Stock >= cantidadVendida)
            {
                producto.Stock -= cantidadVendida;
                decimal totalVenta = cantidadVendida * producto.PrecioUnitario;

                Venta venta = new Venta()
                {
                    FechaHora = DateTime.Now,
                    NombreProducto = producto.Nombre,
                    CantidadVendida = cantidadVendida,
                    TotalVenta = totalVenta
                };

                ventas.Add(venta);
                ActualizarResumenVentas();
                ActualizarStockDisponible();
            }
            else
            {
                MessageBox.Show("Producto no encontrado o stock insuficiente.");
            }
        }

        private void ActualizarResumenVentas()
        {
            dgvResumenVentas.DataSource = null;
            dgvResumenVentas.DataSource = ventas;

            DateTime hoy = DateTime.Today;
            decimal totalGanadoDiario = ventas
                .Where(v => v.FechaHora.Date == hoy)
                .Sum(v => v.TotalVenta);

            lblTotalGanado.Text = $"Total Ganado Hoy: {totalGanadoDiario:C}";
        }

        private void ActualizarStockDisponible()
        {
            var producto = productos.FirstOrDefault(p => p.Nombre == txtNombreProducto.Text);
            if (producto != null)
            {
                lblStockDisponible.Text = $"Stock disponible: {producto.Stock}";
                lblPrecioUnitario.Text = $"Precio unitario: {producto.PrecioUnitario:C}";
            }
            else
            {
                lblStockDisponible.Text = "Stock disponible: 0";
                lblPrecioUnitario.Text = "Precio unitario: $0.00";
            }
        }

        private void CalcularTotalVenta()
        {
            string nombreProducto = txtNombreProducto.Text;
            int cantidadVendida = (int)nudCantidad.Value;

            Producto producto = productos.FirstOrDefault(p => p.Nombre == nombreProducto);
            if (producto != null)
            {
                decimal totalVenta = cantidadVendida * producto.PrecioUnitario;
                lblTotalVenta.Text = $"Total Venta: {totalVenta:C}";
            }
            else
            {
                lblTotalVenta.Text = "Total Venta: $0.00";
            }
        }

        private void nudCantidad_ValueChanged(object sender, EventArgs e)
        {
            CalcularTotalVenta();
        }

        private void txtNombreProducto_TextChanged(object sender, EventArgs e)
        {
            ActualizarStockDisponible();
            CalcularTotalVenta();
        }
    }
}