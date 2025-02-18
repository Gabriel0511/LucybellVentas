namespace FrontEnd
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtNombreProducto = new System.Windows.Forms.TextBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblStockDisponible = new System.Windows.Forms.Label();
            this.lblPrecioUnitario = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.btnRegistrarVenta = new System.Windows.Forms.Button();
            this.dgvResumenVentas = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnGenerarReporte = new System.Windows.Forms.Button();
            this.btnAnularVenta = new System.Windows.Forms.Button();
            this.btnEditProducto = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumenVentas)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNombreProducto
            // 
            this.txtNombreProducto.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtNombreProducto.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtNombreProducto.Location = new System.Drawing.Point(8, 29);
            this.txtNombreProducto.Margin = new System.Windows.Forms.Padding(2);
            this.txtNombreProducto.Name = "txtNombreProducto";
            this.txtNombreProducto.Size = new System.Drawing.Size(312, 20);
            this.txtNombreProducto.TabIndex = 0;
            this.txtNombreProducto.TextChanged += new System.EventHandler(this.txtNombreProducto_TextChanged);
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(323, 29);
            this.nudCantidad.Margin = new System.Windows.Forms.Padding(2);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(103, 20);
            this.nudCantidad.TabIndex = 1;
            this.nudCantidad.ValueChanged += new System.EventHandler(this.nudCantidad_ValueChanged);
            // 
            // lblStockDisponible
            // 
            this.lblStockDisponible.AutoSize = true;
            this.lblStockDisponible.Location = new System.Drawing.Point(5, 56);
            this.lblStockDisponible.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStockDisponible.Name = "lblStockDisponible";
            this.lblStockDisponible.Size = new System.Drawing.Size(50, 13);
            this.lblStockDisponible.TabIndex = 2;
            this.lblStockDisponible.Text = "Stock : 0";
            // 
            // lblPrecioUnitario
            // 
            this.lblPrecioUnitario.AutoSize = true;
            this.lblPrecioUnitario.Location = new System.Drawing.Point(5, 79);
            this.lblPrecioUnitario.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPrecioUnitario.Name = "lblPrecioUnitario";
            this.lblPrecioUnitario.Size = new System.Drawing.Size(52, 13);
            this.lblPrecioUnitario.TabIndex = 3;
            this.lblPrecioUnitario.Text = "Precio : 0";
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Location = new System.Drawing.Point(5, 99);
            this.lblSubtotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(61, 13);
            this.lblSubtotal.TabIndex = 4;
            this.lblSubtotal.Text = "Subtotal : 0";
            // 
            // btnRegistrarVenta
            // 
            this.btnRegistrarVenta.Location = new System.Drawing.Point(430, 27);
            this.btnRegistrarVenta.Margin = new System.Windows.Forms.Padding(2);
            this.btnRegistrarVenta.Name = "btnRegistrarVenta";
            this.btnRegistrarVenta.Size = new System.Drawing.Size(95, 23);
            this.btnRegistrarVenta.TabIndex = 5;
            this.btnRegistrarVenta.Text = "Registrar Venta";
            this.btnRegistrarVenta.UseVisualStyleBackColor = true;
            this.btnRegistrarVenta.Click += new System.EventHandler(this.btnRegistrarVenta_Click);
            // 
            // dgvResumenVentas
            // 
            this.dgvResumenVentas.AllowUserToAddRows = false;
            this.dgvResumenVentas.AllowUserToDeleteRows = false;
            this.dgvResumenVentas.AllowUserToResizeColumns = false;
            this.dgvResumenVentas.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.OldLace;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvResumenVentas.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvResumenVentas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResumenVentas.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvResumenVentas.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvResumenVentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResumenVentas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvResumenVentas.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResumenVentas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvResumenVentas.ColumnHeadersHeight = 30;
            this.dgvResumenVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvResumenVentas.Cursor = System.Windows.Forms.Cursors.Default;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.PapayaWhip;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResumenVentas.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvResumenVentas.EnableHeadersVisualStyles = false;
            this.dgvResumenVentas.Location = new System.Drawing.Point(8, 114);
            this.dgvResumenVentas.Margin = new System.Windows.Forms.Padding(2);
            this.dgvResumenVentas.Name = "dgvResumenVentas";
            this.dgvResumenVentas.ReadOnly = true;
            this.dgvResumenVentas.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvResumenVentas.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResumenVentas.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvResumenVentas.RowHeadersVisible = false;
            this.dgvResumenVentas.RowHeadersWidth = 62;
            this.dgvResumenVentas.RowTemplate.Height = 28;
            this.dgvResumenVentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResumenVentas.Size = new System.Drawing.Size(517, 145);
            this.dgvResumenVentas.TabIndex = 6;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(5, 268);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(37, 13);
            this.lblTotal.TabIndex = 7;
            this.lblTotal.Text = "Total :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Registro de Ventas";
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(418, 90);
            this.btnAgregar.Margin = new System.Windows.Forms.Padding(2);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(104, 21);
            this.btnAgregar.TabIndex = 9;
            this.btnAgregar.Text = "Agregar Producto";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnGenerarReporte
            // 
            this.btnGenerarReporte.Location = new System.Drawing.Point(430, 268);
            this.btnGenerarReporte.Name = "btnGenerarReporte";
            this.btnGenerarReporte.Size = new System.Drawing.Size(95, 23);
            this.btnGenerarReporte.TabIndex = 10;
            this.btnGenerarReporte.Text = "Generar Reporte";
            this.btnGenerarReporte.UseVisualStyleBackColor = true;
            this.btnGenerarReporte.Click += new System.EventHandler(this.btnGenerarReporte_Click);
            // 
            // btnAnularVenta
            // 
            this.btnAnularVenta.Location = new System.Drawing.Point(323, 268);
            this.btnAnularVenta.Name = "btnAnularVenta";
            this.btnAnularVenta.Size = new System.Drawing.Size(90, 23);
            this.btnAnularVenta.TabIndex = 12;
            this.btnAnularVenta.Text = "Anular Venta";
            this.btnAnularVenta.UseVisualStyleBackColor = true;
            this.btnAnularVenta.Click += new System.EventHandler(this.btnAnularVenta_Click);
            // 
            // btnEditProducto
            // 
            this.btnEditProducto.Location = new System.Drawing.Point(321, 90);
            this.btnEditProducto.Name = "btnEditProducto";
            this.btnEditProducto.Size = new System.Drawing.Size(92, 23);
            this.btnEditProducto.TabIndex = 13;
            this.btnEditProducto.Text = "Editar Producto";
            this.btnEditProducto.UseVisualStyleBackColor = true;
            this.btnEditProducto.Click += new System.EventHandler(this.btnEditProducto_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(211, 90);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(104, 23);
            this.btnEliminar.TabIndex = 14;
            this.btnEliminar.Text = "Eliminar Producto";
            this.btnEliminar.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(533, 292);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnEditProducto);
            this.Controls.Add(this.btnAnularVenta);
            this.Controls.Add(this.btnGenerarReporte);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvResumenVentas);
            this.Controls.Add(this.btnRegistrarVenta);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblPrecioUnitario);
            this.Controls.Add(this.lblStockDisponible);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.txtNombreProducto);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "LucyBell";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumenVentas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNombreProducto;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblStockDisponible;
        private System.Windows.Forms.Label lblPrecioUnitario;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Button btnRegistrarVenta;
        private System.Windows.Forms.DataGridView dgvResumenVentas;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnGenerarReporte;
        private System.Windows.Forms.Button btnAnularVenta;
        private System.Windows.Forms.Button btnEditProducto;
        private System.Windows.Forms.Button btnEliminar;
    }
}

