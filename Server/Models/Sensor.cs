using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class Sensor
    {
        public virtual int ID_REG { get; set; }
        public int ID_SENSOR { get; set; }
        public double MEDICION { get; set; }
        public DateTime FECHAYHORA { get; set; }
    }
}
