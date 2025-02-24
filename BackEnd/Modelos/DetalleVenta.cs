using BackEnd.Modelos;

namespace BackEnd.Modelos
{
    public class DetalleVenta
    {
        public int id_detalle { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal total { get; set; }

        // Llaves foráneas
        public int id_producto { get; set; }
        public int id_venta { get; set; }

        // Propiedades de navegación (opcional)
        public Producto Producto { get; set; }
        public Venta Venta { get; set; }
    }
}

