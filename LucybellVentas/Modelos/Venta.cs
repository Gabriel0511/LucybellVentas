using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucybellVentas.Modelos
{
    public class Venta
    {
        public DateTime FechaHora { get; set; }
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVenta { get; set; }
    }
}
