using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class Usuario:Register
    {
        [Required]
        public string ROL { get; set; }
        [Required]
        public string NOMBRE { get; set; }
        [Required]
        [EmailAddress]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CORREO { get; set; }
        public string DOC_TYPE { get; set; }
        [Required]
        public string DOCUMENTO { get; set; }
        [Required]
        public string PASSWORD { get; set; }

    }
}