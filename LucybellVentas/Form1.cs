using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using BackEnd.Modelos;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using LucybellVentas;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Reflection;
using static iTextSharp.text.pdf.XfaForm;
using System.Drawing;
using BackEnd;
using System.Net.Http;


namespace FrontEnd
{
    public partial class Form1 : Form
    {
        private Producto ListaProducto = new Producto();
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private int idProductoSeleccionado = -1;

        public string TextoNombre
        {
            get { return txtNombreProducto.Text; }
            set { txtNombreProducto.Text = value; }
        }

        public ListBox ListBoxProductos
        {
            get { return listBoxProductos; }
        }

        public Form1()
        {
            InitializeComponent();
            VerVentas();

            dgvResumenVentas.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvResumenVentas_CellFormatting);
            this.Shown += (s, e) => txtNombreProducto.Focus();

            // Manejar el evento cuando cambia la fecha
            dtpFecha.ValueChanged += dtpFecha_ValueChanged_1;

            // Configurar el formato personalizado al principio
            dtpFecha.CustomFormat = "dd/MM/yyyy";
            dtpFecha.Format = DateTimePickerFormat.Custom; // Usar formato personalizado
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dgvResumenVentas.CellValueChanged += dgvResumenVentas_CellValueChanged;
            // Desactivar la selección automática
            dgvResumenVentas.ClearSelection();
        }

        #region Controles

        private void nudCantidad_ValueChanged(object sender, EventArgs e)
        {
            CalcularSubtotal();
        }

        private void txtNombreProducto_TextChanged(object sender, EventArgs e)
        {
            // Si el TextBox está vacío, oculta el ListBox
            if (string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                listBoxProductos.Visible = false;
                return;
            }

            // Si no es un nombre exacto, mostrar el ListBox con autocompletado
            CargarAutocompletado(txtNombreProducto.Text);
            listBoxProductos.Visible = true;
        }

        private void listBoxProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProductos.SelectedItem != null)
            {
                // Deshabilitar temporalmente el evento para evitar que se dispare nuevamente
                txtNombreProducto.TextChanged -= txtNombreProducto_TextChanged;

                txtNombreProducto.Text = listBoxProductos.SelectedItem.ToString();
                listBoxProductos.Visible = false;

                // Reactivar el evento después de la actualización
                txtNombreProducto.TextChanged += txtNombreProducto_TextChanged;

                // Ahora sí actualizar la información del producto
                ActualizarInfoProducto();
            }
        }

        private void dtpFecha_ValueChanged_1(object sender, EventArgs e)
        {

            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();
            
            // Si hay una fecha seleccionada, mostrarla en formato de fecha normal
            dtpFecha.CustomFormat = "dd/MM/yyyy"; // Formato normal de fecha

            VerVentasPorFecha(dtpFecha.Value);

            // Ocultar la columna id_venta
             dgvResumenVentas.Columns["id_venta"].Visible = false;

        }


        #endregion


        #region Botones

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();
            dtpFecha.Value = DateTime.Now;

            string nombreProducto = txtNombreProducto.Text;
            int cantidad = int.Parse(nudCantidad.Text);

            try
            {
                if (nudCantidad.Value != 0 && txtNombreProducto.Text != "")
                {
                    dbHelper.RegistrarVenta(nombreProducto, cantidad);
                    
                    VerVentas();

                    //limpiar textbox
                    txtNombreProducto.Clear();
                    nudCantidad.Value = 0;
                    nudCantidad.Enabled = false;
                    lblPrecioUnitario.Text = "Precio : 0,00";
                    lblStockDisponible.Text = "Stock : 0";

                    MessageBox.Show("Venta registrada correctamente.");
                }
                else
                {
                    MessageBox.Show("Todos los campos son obligatorios.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();

            FormAgregarProducto formAgregar = new FormAgregarProducto();
            formAgregar.ShowDialog();
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();

            GenerarReporteVentasDelDia();
        }

        private void btnAnularVenta_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;

            // Verificar si hay una fila seleccionada
            if (dgvResumenVentas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona una venta para anular.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el ID de la venta seleccionada
            DataGridViewRow selectedRow = dgvResumenVentas.SelectedRows[0];

            if (!int.TryParse(selectedRow.Cells["id_venta"].Value?.ToString(), out int idVenta))
            {
                MessageBox.Show("ID de venta inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verificar si la venta ya está anulada
            string estadoVenta = selectedRow.Cells["Estado"].Value?.ToString();
            if (estadoVenta == "Suspendida")
            {
                MessageBox.Show("Esta venta ya está anulada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Primera confirmación
            DialogResult confirmacion1 = MessageBox.Show(
                "¿Estás seguro de que quieres anular esta venta?\nEsta acción es PERMANENTE",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion1 == DialogResult.No)
            {
                return; // Cancelar eliminación
            }

            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                bool ventaSuspendida = dbHelper.AnularVenta(idVenta);

                if (ventaSuspendida)
                {
                    MessageBox.Show("Venta anulada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar el estado de la venta en el DataGridView
                    if (dgvResumenVentas.Columns.Contains("Estado"))
                    {
                        selectedRow.Cells["Estado"].Value = "Suspendida";
                    }

                }
                else
                {
                    MessageBox.Show("No se pudo anular la venta. Puede que ya esté suspendida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al anular la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ActualizarInfoProducto();

        }

        private void btnEditProducto_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();

            string nombreProducto = txtNombreProducto.Text;
            if (string.IsNullOrWhiteSpace(nombreProducto))
            {
                MessageBox.Show("Ingrese el nombre del producto a editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormEditarProducto formEditar = new FormEditarProducto(nombreProducto);
            formEditar.ShowDialog();

            if (dtpFecha.CustomFormat == "TODAS")
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                DataTable ventas = dbHelper.ObtenerTodasLasVentas();
                dgvResumenVentas.DataSource = ventas;
            }
            else
            {
                VerVentasPorFecha(dtpFecha.Value);
            }
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            dgvResumenVentas.ClearSelection();
            listBoxProductos.Visible = false;

            Producto producto = ListaProducto.ObtenerInfoProducto(txtNombreProducto.Text);

            // Verificar si el producto existe antes de continuar
            if (producto == null)
            {
                MessageBox.Show("No se encontró el producto especificado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int idProducto = producto.id_producto;

            // Primera confirmación
            DialogResult confirmacion1 = MessageBox.Show(
                "¿Estás seguro de que quieres eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion1 == DialogResult.No)
            {
                return; // Cancelar eliminación
            }

            // Segunda confirmación
            DialogResult confirmacion2 = MessageBox.Show(
                "Este producto también se eliminará de las ventas. ¿Deseas continuar?",
                "Advertencia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion2 == DialogResult.No)
            {
                return; // Cancelar eliminación
            }

            // Proceder con la eliminación
            bool productoEliminado = dbHelper.EliminarProducto(idProducto);

            if (productoEliminado)
            {
                MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo eliminar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtNombreProducto.Clear();
            lblStockDisponible.Text = "Stock : 0 ";
            lblPrecioUnitario.Text = "Precio : 0,00 ";
            nudCantidad.Value = 0;

            if (dtpFecha.CustomFormat == "TODAS")
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                DataTable ventas = dbHelper.ObtenerTodasLasVentas();
                dgvResumenVentas.DataSource = ventas;
            }
            else
            {
                VerVentasPorFecha(dtpFecha.Value);
            }
        }

        private void btnVerProductos_Click(object sender, EventArgs e)
        {
            dgvResumenVentas.ClearSelection();
            if (listBoxProductos.Visible)
            {
                listBoxProductos.Visible = false;
            }
            else
            {
                // Limpiar el ListBox antes de cargar nuevos datos
                listBoxProductos.Items.Clear();
                listBoxProductos.Visible = true;

                // Llamar al backend para obtener los productos filtrados
                List<string> productos = dbHelper.VerProductos();

                // Agregar los productos al ListBox
                foreach (string producto in productos)
                {
                    listBoxProductos.Items.Add(producto);
                }
            }         
        }

        private void btnVerTodas_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;

            DatabaseHelper dbHelper = new DatabaseHelper();
            DataTable ventas = dbHelper.ObtenerTodasLasVentas();
            dgvResumenVentas.DataSource = ventas;
            CalcularTotal(ventas);

            dgvResumenVentas.ClearSelection();
            dtpFecha.CustomFormat = "TODAS";  // Texto para indicar que no se ha seleccionado una fecha
        }

        #endregion


        #region Metodos

        private void VerVentas()
        {
            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                DataTable ventasDelDia = dbHelper.ObtenerVentasDelDia();

                dgvResumenVentas.DataSource = ventasDelDia;

                // Ocultar la columna id_venta
                dgvResumenVentas.Columns["id_venta"].Visible = false;

                // Llamar a CalcularTotal con los datos cargados
                CalcularTotal(ventasDelDia);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las ventas: " + ex.Message);
            }
            dgvResumenVentas.ClearSelection();
        }

        private void CalcularTotal(DataTable dt)
        {
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Estado"].ToString() == "Completada") // Filtrar solo ventas completadas
                {
                    total += Convert.ToDecimal(row["Subtotal"]);
                }
            }

            lblTotal.Text = "Total: $" + total.ToString("0.00");
        }

        private void ActualizarInfoProducto()
        {
            try
            {
                // Llamamos al backend para obtener la información del producto
                DatabaseHelper dbHelper = new DatabaseHelper();
                Producto producto = ListaProducto.ObtenerInfoProducto(txtNombreProducto.Text);

                if (producto != null)
                {
                    idProductoSeleccionado = producto.id_producto;
                    lblStockDisponible.Text = "Stock : " + producto.stock.ToString();
                    lblPrecioUnitario.Text = "Precio : " + producto.precio.ToString();
                    CalcularSubtotal();
                    nudCantidad.Enabled = (producto.stock > 0);

                    if (producto.stock == 0)
                    {
                        nudCantidad.Value = 0;
                    }
                    else
                    {
                        nudCantidad.Value = 1;
                    }
                }
                else
                {
                    idProductoSeleccionado = -1; // Si no encuentra el producto
                    lblStockDisponible.Text = "Stock : 0";
                    lblPrecioUnitario.Text = "Precio : 0";
                    lblSubtotal.Text = "Subtotal : 0";
                    nudCantidad.Enabled = false;
                    nudCantidad.Value = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la información del producto: " + ex.Message);
            }
        }

        private void CargarAutocompletado(string filtro)
        {
            try
            {
                // Limpiar el ListBox antes de cargar nuevos datos
                listBoxProductos.Items.Clear();

                // Llamar al backend para obtener los productos filtrados
                DatabaseHelper dbHelper = new DatabaseHelper();
                List<string> productos = dbHelper.BuscarProductos(filtro);

                // Agregar los productos al ListBox
                foreach (string producto in productos)
                {
                    listBoxProductos.Items.Add(producto);
                }

                // Mostrar el ListBox si hay resultados
                if (listBoxProductos.Items.Count > 0)
                {
                    listBoxProductos.Visible = true;
                }
                else
                {
                    listBoxProductos.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message);
            }
        }

        private void CalcularSubtotal()
        {

            if (string.IsNullOrWhiteSpace(lblPrecioUnitario.Text))
            {
                MessageBox.Show("El precio unitario no está disponible.");
                return;
            }

            decimal precioUnitario = 0;

            // Intentar convertir el texto del Label a decimal
            if (!decimal.TryParse(lblPrecioUnitario.Text.Replace("Precio : ", ""), out precioUnitario))
            {
                MessageBox.Show("Error: Precio unitario no válido.");
                return;
            }

            decimal cantidad = nudCantidad.Value; // Ya es decimal, no necesita conversión
            decimal subtotal = precioUnitario * cantidad;

            lblSubtotal.Text = "Subtotal : " + subtotal.ToString("0.00"); // Mostrar con formato adecuado
        }

        public void GenerarReporteVentasDelDia()
        {
            if (dgvResumenVentas.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para generar el reporte.", "Reporte de ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Crear DataTable con los datos del DataGridView
            DataTable dtReporte = new DataTable();

            // Agregar las columnas del DataGridView al DataTable (excepto "id_venta")
            foreach (DataGridViewColumn col in dgvResumenVentas.Columns)
            {
                if (col.HeaderText != "id_venta") // Excluir la columna "id_venta"
                {
                    dtReporte.Columns.Add(col.HeaderText);
                }
            }

            // Agregar las filas del DataGridView al DataTable
            foreach (DataGridViewRow row in dgvResumenVentas.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dr = dtReporte.NewRow();
                    int colIndex = 0;

                    for (int i = 0; i < dgvResumenVentas.Columns.Count; i++)
                    {
                        if (dgvResumenVentas.Columns[i].HeaderText != "id_venta") // Excluir la columna "id_venta"
                        {
                            dr[colIndex] = row.Cells[i].Value ?? DBNull.Value;
                            colIndex++;
                        }
                    }
                    dtReporte.Rows.Add(dr);
                }
            }

            // Crear PDF
            Document doc = new Document(PageSize.A4);
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Reporte_Ventas_{DateTime.Now:yyyy-MM-dd}.pdf");

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Crear tabla para logo y título
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.WidthPercentage = 100;
                headerTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                // Agregar logo desde recursos incrustados
                string logoPath = "LucybellVentas.img.epico.jpg";
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(logoPath))
                {
                    if (stream != null)
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(stream);
                        logo.ScaleToFit(100f, 100f);
                        PdfPCell logoCell = new PdfPCell(logo) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                        headerTable.AddCell(logoCell);
                    }
                }

                // Agregar título
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                PdfPCell titleCell = new PdfPCell(new Phrase("Reporte de Ventas \n ", titleFont))
                {
                    Border = iTextSharp.text.Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                headerTable.AddCell(titleCell);
                doc.Add(headerTable);

                // Crear tabla para los datos en PDF (sin "id_venta")
                PdfPTable table = new PdfPTable(dtReporte.Columns.Count);
                table.WidthPercentage = 100;

                // Agregar encabezados
                foreach (DataColumn col in dtReporte.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName))
                    {
                        BackgroundColor = new BaseColor(192, 192, 255),
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                // Agregar filas de datos
                foreach (DataRow row in dtReporte.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(item.ToString()));
                        string estado = row["Estado"].ToString();

                        if (estado == "Completada")
                        {
                            cell.BackgroundColor = new BaseColor(255, 239, 184);
                        }
                        else if (estado == "Suspendida")
                        {
                            cell.BackgroundColor = new BaseColor(211, 211, 211);
                        }
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);

                // Agregar total
                string totalVenta = lblTotal.Text;
                iTextSharp.text.Font totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Paragraph totalParagraph = new Paragraph(totalVenta, totalFont) { Alignment = Element.ALIGN_RIGHT, SpacingBefore = 20 };
                doc.Add(totalParagraph);

                doc.Close();
                MessageBox.Show($"Reporte generado con éxito. \nGuardado en: {filePath}", "Reporte generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message);
            }
        }


        private void dgvResumenVentas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Verificamos que la columna editada sea la de Estado
            if (dgvResumenVentas.Columns[e.ColumnIndex].Name == "Estado")
            {
                // Obtener los datos actuales del DataGridView
                DataTable dt = (DataTable)dgvResumenVentas.DataSource;
                CalcularTotal(dt);
            }
        }

        private void dgvResumenVentas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Verificar si estamos en la columna 'Estado' y si el valor de la celda es 'Suspendida'
            if (dgvResumenVentas.Columns[e.ColumnIndex].Name == "Estado")
            {
                // Verificar si el estado es 'Suspendida'
                if (e.Value != null && e.Value.ToString() == "Suspendida")
                {
                    // Cambiar el color de fondo de la fila usando valores RGB
                    dgvResumenVentas.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(211, 211, 211); // Gris claro (RGB)

                    // Opcional: cambiar el color del texto
                    dgvResumenVentas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0); // Negro (RGB)
                }
                else
                {
                    // Si no está suspendida, restablecer el color a su valor predeterminado
                    dgvResumenVentas.Rows[e.RowIndex].DefaultCellStyle.BackColor = dgvResumenVentas.DefaultCellStyle.BackColor;
                    dgvResumenVentas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = dgvResumenVentas.DefaultCellStyle.ForeColor;
                }
            }
        }

        private void VerVentasPorFecha(DateTime fechaSeleccionada)
        {
            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                DataTable ventasPorFecha = dbHelper.ObtenerVentasPorFecha(fechaSeleccionada);

                dgvResumenVentas.DataSource = ventasPorFecha;

                // Calcular el total solo con los datos de la fecha seleccionada
                CalcularTotal(ventasPorFecha);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las ventas: " + ex.Message);
            }
            dgvResumenVentas.ClearSelection();
        }


        #endregion

        #region Clicks

        private void Form1_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();
        }

        private void dgvResumenVentas_MouseClick(object sender, MouseEventArgs e)
        {
            listBoxProductos.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();
        }

        private void lblTotal_Click(object sender, EventArgs e)
        {
            listBoxProductos.Visible = false;
            dgvResumenVentas.ClearSelection();
        }

        #endregion


    }
}