using System;
using System.Collections.Generic;
using System.Device.Location;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class Reporte : Register
    {
        [Required]
        public string SERIALSENSOR { get; set; }
        [Required]
        public double MEDICION { get; set; }
        [Required]
        public double LATITUD { get; set; }
        [Required]
        public double LONGITUD { get; set; }
        [Required]
        public DateTime DATEREPORTED { get; set; }
    }
}