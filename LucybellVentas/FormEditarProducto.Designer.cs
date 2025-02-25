namespace LucybellVentas
{
    partial class FormEditarProducto
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditarProducto));
            this.lblEditarProducto = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.txtStock = new System.Windows.Forms.TextBox();
            this.btnGuardarCambios = new System.Windows.Forms.Button();
            this.lblStockP = new System.Windows.Forms.Label();
            this.lblPrecioP = new System.Windows.Forms.Label();
            this.lblNombreP = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEditarProducto
            // 
            this.lblEditarProducto.AutoSize = true;
            this.lblEditarProducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditarProducto.Location = new System.Drawing.Point(103, 9);
            this.lblEditarProducto.Name = "lblEditarProducto";
            this.lblEditarProducto.Size = new System.Drawing.Size(216, 31);
            this.lblEditarProducto.TabIndex = 0;
            this.lblEditarProducto.Text = "Editar Producto";
            // 
            // txtNombre
            // 
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(12, 69);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(400, 28);
            this.txtNombre.TabIndex = 1;
            // 
            // txtPrecio
            // 
            this.txtPrecio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPrecio.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrecio.Location = new System.Drawing.Point(12, 139);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(400, 28);
            this.txtPrecio.TabIndex = 2;
            // 
            // txtStock
            // 
            this.txtStock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStock.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStock.Location = new System.Drawing.Point(12, 217);
            this.txtStock.Name = "txtStock";
            this.txtStock.Size = new System.Drawing.Size(400, 28);
            this.txtStock.TabIndex = 3;
            // 
            // btnGuardarCambios
            // 
            this.btnGuardarCambios.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.btnGuardarCambios.Location = new System.Drawing.Point(12, 275);
            this.btnGuardarCambios.Name = "btnGuardarCambios";
            this.btnGuardarCambios.Size = new System.Drawing.Size(395, 44);
            this.btnGuardarCambios.TabIndex = 4;
            this.btnGuardarCambios.Text = "Guardar cambios";
            this.btnGuardarCambios.UseVisualStyleBackColor = true;
            this.btnGuardarCambios.Click += new System.EventHandler(this.btnGuardarCambios_Click);
            // 
            // lblStockP
            // 
            this.lblStockP.AutoSize = true;
            this.lblStockP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockP.Location = new System.Drawing.Point(13, 194);
            this.lblStockP.Name = "lblStockP";
            this.lblStockP.Size = new System.Drawing.Size(58, 20);
            this.lblStockP.TabIndex = 12;
            this.lblStockP.Text = "Stock :";
            // 
            // lblPrecioP
            // 
            this.lblPrecioP.AutoSize = true;
            this.lblPrecioP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecioP.Location = new System.Drawing.Point(13, 116);
            this.lblPrecioP.Name = "lblPrecioP";
            this.lblPrecioP.Size = new System.Drawing.Size(61, 20);
            this.lblPrecioP.TabIndex = 11;
            this.lblPrecioP.Text = "Precio :";
            // 
            // lblNombreP
            // 
            this.lblNombreP.AutoSize = true;
            this.lblNombreP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreP.Location = new System.Drawing.Point(13, 46);
            this.lblNombreP.Name = "lblNombreP";
            this.lblNombreP.Size = new System.Drawing.Size(73, 20);
            this.lblNombreP.TabIndex = 10;
            this.lblNombreP.Text = "Nombre :";
            // 
            // FormEditarProducto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 331);
            this.Controls.Add(this.lblStockP);
            this.Controls.Add(this.lblPrecioP);
            this.Controls.Add(this.lblNombreP);
            this.Controls.Add(this.btnGuardarCambios);
            this.Controls.Add(this.txtStock);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblEditarProducto);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormEditarProducto";
            this.Text = "LucyBell - Editar Producto";
            this.Load += new System.EventHandler(this.FormEditarProducto_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEditarProducto;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.TextBox txtStock;
        private System.Windows.Forms.Button btnGuardarCambios;
        private System.Windows.Forms.Label lblStockP;
        private System.Windows.Forms.Label lblPrecioP;
        private System.Windows.Forms.Label lblNombreP;
    }
}