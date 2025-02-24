using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BackEnd.Modelos
{
    public class Venta
    {
        public int id_venta { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}