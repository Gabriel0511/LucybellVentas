﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Modelos
{
    public class Producto
    {
        public string Nombre { get; set; }
        public int Stock { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
