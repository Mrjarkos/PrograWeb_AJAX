using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class JsonTable
    {

            public List<Models.Sensor> Registros;
            public int PaginaActual { get; set; }
            public int TotalPaginas { get; set; }


    }
}