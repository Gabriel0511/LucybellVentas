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


namespace FrontEnd
{
    public partial class Form1 : Form
    {
        private List<Producto> productos = new List<Producto>();
        private List<Venta> ventas = new List<Venta>();
        private int idProductoSeleccionado = -1;

        SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;");

        public Form1()
        {
            InitializeComponent();
            VerVentas();
            dgvResumenVentas.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvResumenVentas_CellFormatting);
            this.Shown += (s, e) => txtNombreProducto.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvResumenVentas.CellValueChanged += dgvResumenVentas_CellValueChanged;

        }

        #region TextBox

        private void nudCantidad_ValueChanged(object sender, EventArgs e)
        {
            CalcularSubtotal();
        }
        private void txtNombreProducto_TextChanged(object sender, EventArgs e)
        {
            // Mostrar u ocultar el ListBox según si hay texto en el TextBox
            if (string.IsNullOrWhiteSpace(txtNombreProducto.Text))
            {
                listBoxProductos.Visible = false;
            }
            else
            {
                CargarAutocompletado(txtNombreProducto.Text);
                listBoxProductos.Visible = true;
            }
            ActualizarInfoProducto();
        }

        #endregion

        #region Botones

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if(nudCantidad.Value != 0)
            {
                DatabaseHelper db = new DatabaseHelper();
                db.RegistrarVenta(txtNombreProducto.Text, Convert.ToInt32(nudCantidad.Text));

                VerVentas();

                //limpiar textbox
                txtNombreProducto.Clear();
                nudCantidad.Value = 0;
            }
            else
            {
                MessageBox.Show("La cantidad vendida no puede ser cero.");
            }
               
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FormAgregarProducto formAgregar = new FormAgregarProducto();
            formAgregar.ShowDialog();
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            GenerarReporteVentasDelDia();
        }

        private void BtnEditProducto_Click(object sender, EventArgs e)
        {
            string nombreProducto = txtNombreProducto.Text;
            if (string.IsNullOrWhiteSpace(nombreProducto))
            {
                MessageBox.Show("Ingrese el nombre del producto a editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormEditarProducto formEditar = new FormEditarProducto(nombreProducto);
            formEditar.ShowDialog();
        }
        private void btnAnularVenta_Click(object sender, EventArgs e)
        {
            // Verificar que se haya seleccionado una venta en el DataGridView
            if (dgvResumenVentas.SelectedRows.Count > 0)
            {
                // Obtener el ID de la venta seleccionada (supongamos que tienes una columna ID_Venta)
                int idVenta = Convert.ToInt32(dgvResumenVentas.SelectedRows[0].Cells["id_venta"].Value);

                // Definir la consulta SQL para actualizar el estado de la venta
                string query = "UPDATE Ventas SET Estado = 'Suspendida' WHERE id_venta = @idVenta AND Estado = 'Completada'";

                // Ejecutar la consulta SQL (asegurándote de usar parámetros para evitar inyección SQL)
                using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;"))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idVenta", idVenta);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Verificar si se actualizó correctamente
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Venta suspendida correctamente.");

                            // Opcional: Actualizar el DataGridView si es necesario
                            // Actualizar la fila en el DataGridView
                            dgvResumenVentas.SelectedRows[0].Cells["Estado"].Value = "Suspendida";
                        }
                        else
                        {
                            MessageBox.Show("La venta ya está anulada.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una venta para anular.");
            }
        }

        private void btnEditProducto_Click(object sender, EventArgs e)
        {
            string nombreProducto = txtNombreProducto.Text;
            if (string.IsNullOrWhiteSpace(nombreProducto))
            {
                MessageBox.Show("Ingrese el nombre del producto a editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormEditarProducto formEditar = new FormEditarProducto(nombreProducto);
            formEditar.ShowDialog();

            ActualizarInfoProducto();

            txtNombreProducto.Text = "";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            EliminarProducto();
        }

        #endregion

        #region Metodos

        private void VerVentas()
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;"))
                {
                    con.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapt = new SqlDataAdapter(
                         "SELECT p.nombre AS 'Producto', p.precio AS 'Precio Unitario', " +
                        "dv.cantidad AS 'Cantidad', dv.total AS 'Subtotal', v.Estado, v.id_venta " +
                        "FROM Productos p " +
                        "JOIN DetallesVenta dv ON p.id_producto = dv.id_producto " +
                        "JOIN Ventas v ON dv.id_venta = v.id_venta " +
                        "WHERE CAST(v.fecha AS DATE) = CAST(GETDATE() AS DATE);", con))
                    {
                        adapt.Fill(dt);
                    }
                    dgvResumenVentas.DataSource = dt;

                    // Ocultar la columna id_venta
                    dgvResumenVentas.Columns["id_venta"].Visible = false;

                    // Llamar a CalcularTotal con los datos cargados
                    CalcularTotal(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las ventas: " + ex.Message);
            }
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
                con.Open();
                string query = "SELECT id_producto, nombre, precio, stock FROM productos WHERE nombre = @nombre";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@nombre", txtNombreProducto.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idProductoSeleccionado = Convert.ToInt32(reader["id_producto"]);
                            lblStockDisponible.Text = "Stock : " + reader["stock"].ToString();
                            lblPrecioUnitario.Text = "Precio : " + reader["precio"].ToString();
                            nudCantidad.Enabled = (Convert.ToInt32(reader["stock"]) > 0);
                            if (lblStockDisponible.Text == "Stock : 0")
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la información del producto: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void CargarAutocompletado(string filtro)
        {
            try
            {
                // Limpiar el ListBox antes de cargar nuevos datos
                listBoxProductos.Items.Clear();

                using (SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;"))
                {
                    con.Open();
                    // Filtrar productos que coincidan con el texto ingresado
                    string query = "SELECT nombre FROM productos WHERE nombre LIKE @filtro";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%"); // Usar % para buscar coincidencias parciales

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Agregar cada producto al ListBox
                                listBoxProductos.Items.Add(reader["nombre"].ToString());
                            }
                        }
                    }
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
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";
            string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");
            string query = @"SELECT  V.fecha AS 'Fecha', P.nombre AS Producto, DV.cantidad AS 'Cantidad', DV.precio_unitario AS 'Precio Unitario', DV.total AS 'Subtotal', V.Estado
                    FROM Ventas V
                    INNER JOIN DetallesVenta DV ON V.id_venta = DV.id_venta
                    INNER JOIN Productos P ON DV.id_producto = P.id_producto
                    WHERE CONVERT(date, V.fecha) = @fechaHoy";

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fechaHoy", fechaHoy);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay ventas registradas para hoy.", "Reporte de ventas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Crear PDF
            Document doc = new Document(PageSize.A4);
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Reporte_Ventas_{fechaHoy}.pdf");

            try
            {
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Crear tabla para logo y título
                PdfPTable headerTable = new PdfPTable(1); // Una sola columna
                headerTable.WidthPercentage = 100;
                headerTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER; // Sin bordes en las celdas

                // Agregar logo desde recursos incrustados
                string logoPath = "LucybellVentas.img.epico.jpg"; // Nombre del recurso incrustado
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(logoPath))
                {
                    if (stream != null)
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(stream);
                        logo.ScaleToFit(100f, 100f); // Redimensiona el logo
                        PdfPCell logoCell = new PdfPCell(logo);
                        logoCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        logoCell.HorizontalAlignment = Element.ALIGN_CENTER; // Centrar logo
                        headerTable.AddCell(logoCell);
                    }
                    else
                    {
                        throw new Exception("No se pudo cargar el logo desde los recursos.");
                    }
                }

                // Agregar título debajo del logo
                iTextSharp.text.Font titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
                PdfPCell titleCell = new PdfPCell(new Phrase("Reporte de ventas del día \n ", titleFont));
                titleCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER; // Centrar título
                headerTable.AddCell(titleCell);

                doc.Add(headerTable);

                // Tabla con datos
                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;

                // Agregar encabezados
                foreach (DataColumn col in dt.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName));
                    cell.BackgroundColor = new BaseColor(192, 192, 255); // Violeta clarito
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                // Agregar filas de datos
                foreach (DataRow row in dt.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        string estado = row["Estado"].ToString();
                        PdfPCell cell = new PdfPCell(new Phrase(item.ToString()));

                        // Colorear las filas dependiendo del estado
                        if (estado == "Completada")
                        {
                            cell.BackgroundColor = new BaseColor(255, 239, 184); // Papaya
                        }
                        else if (estado == "Suspendida")
                        {
                            cell.BackgroundColor = new BaseColor(211, 211, 211); // Gris
                        }

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        table.AddCell(cell);
                    }
                }

                doc.Add(table);

                // Agregar total desde lblTotal.Text al final del PDF
                string totalVenta = lblTotal.Text; // Obtener el total desde lblTotal
                iTextSharp.text.Font totalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 12);
                Paragraph totalParagraph = new Paragraph(totalVenta, totalFont);
                totalParagraph.Alignment = Element.ALIGN_RIGHT;
                totalParagraph.SpacingBefore = 20; // Espacio antes del total
                doc.Add(totalParagraph);

                doc.Close();

                MessageBox.Show($"Reporte generado con éxito. \nGuardado en: {filePath}", "Reporte generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(filePath); // Abrir automáticamente
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

        private void EliminarProducto()
        {
            if (idProductoSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un producto válido para eliminar.");
                return;
            }

            DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este producto?",
                                                  "Confirmar eliminación",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;"))
                    {
                        con.Open();

                        // Eliminar el producto (sin afectar las ventas)
                        string queryEliminarProducto = "DELETE FROM productos WHERE id_producto = @id";
                        using (SqlCommand cmdProducto = new SqlCommand(queryEliminarProducto, con))
                        {
                            cmdProducto.Parameters.AddWithValue("@id", idProductoSeleccionado);
                            int rowsAffected = cmdProducto.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Producto eliminado correctamente.");
                                txtNombreProducto.Clear();
                                VerVentas(); // Actualizar lista después de eliminar
                            }
                            else
                            {
                                MessageBox.Show("No se encontró el producto para eliminar.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                }
            }
        }

        #endregion

        #region SQL Conexion y Funciones
        public class DatabaseHelper
        {
            public string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LucyBell;Integrated Security=True;";

            public void AgregarProducto(string nombre, decimal precio, int stock)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Productos (nombre, precio, stock) VALUES (@nombre, @precio, @stock)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@stock", stock);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Producto agregado correctamente.");
                }
            }

            public void RegistrarVenta(string nombreProducto, int cantidad)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // Buscar el id_producto con el nombre del producto
                        string queryProducto = "SELECT id_producto, stock FROM Productos WHERE nombre = @nombreProducto";
                        SqlCommand cmdProducto = new SqlCommand(queryProducto, con, transaction);
                        cmdProducto.Parameters.AddWithValue("@nombreProducto", nombreProducto);

                        int idProducto = 0;
                        int stockDisponible = 0;

                        using (SqlDataReader reader = cmdProducto.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Producto no encontrado.");
                                return;
                            }

                            idProducto = reader.GetInt32(0);
                            stockDisponible = reader.GetInt32(1);
                        } // **Aquí se cierra automáticamente el DataReader**

                        // Verificar si hay suficiente stock
                        if (stockDisponible == 0)
                        {
                            MessageBox.Show("Error: No hay stock disponible para este producto.", "Stock agotado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (stockDisponible < cantidad)
                        {
                            MessageBox.Show($"Error: Stock insuficiente. Solo hay {stockDisponible} unidades disponibles.", "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Crear la venta
                        string queryVenta = "INSERT INTO Ventas DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                        SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaction);
                        int idVenta = Convert.ToInt32(cmdVenta.ExecuteScalar());

                        // Obtener el precio del producto
                        string queryPrecio = "SELECT precio FROM Productos WHERE id_producto = @idProducto";
                        SqlCommand cmdPrecio = new SqlCommand(queryPrecio, con, transaction);
                        cmdPrecio.Parameters.AddWithValue("@idProducto", idProducto);
                        decimal precio = Convert.ToDecimal(cmdPrecio.ExecuteScalar());

                        // Insertar detalle de la venta
                        string queryDetalle = "INSERT INTO DetallesVenta (id_venta, id_producto, cantidad, precio_unitario, total) VALUES (@idVenta, @idProducto, @cantidad, @precioUnitario, @total)";
                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaction);
                        cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdDetalle.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdDetalle.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdDetalle.Parameters.AddWithValue("@precioUnitario", precio);
                        cmdDetalle.Parameters.AddWithValue("@total", cantidad * precio);
                        cmdDetalle.ExecuteNonQuery();

                        // Reducir stock
                        string queryStock = "UPDATE Productos SET stock = stock - @cantidad WHERE id_producto = @idProducto";
                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaction);
                        cmdStock.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdStock.ExecuteNonQuery();

                        // Confirmar transacción
                        transaction.Commit();
                        MessageBox.Show("Venta registrada correctamente.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error al registrar la venta: " + ex.Message);
                    }
                }
            }

        }

        #endregion

        private void listBoxProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cuando el usuario selecciona un producto del ListBox, colocarlo en el TextBox
            if (listBoxProductos.SelectedItem != null)
            {
                txtNombreProducto.Text = listBoxProductos.SelectedItem.ToString();
                listBoxProductos.Visible = false; // Ocultar el ListBox después de seleccionar
            }
        }
    }
}