using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BackEnd.Modelos
{
    public class Producto
    {
        public int id_producto { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }


    }
}