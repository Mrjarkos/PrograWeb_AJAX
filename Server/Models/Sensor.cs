using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class Sensor: Register
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SERIAL { get; set; }
        [Required]
        public string PASSWORD { get; set; }
        public string TIPO { get; set; }
        public string DESCRIPCION { get; set; }
    }
}
