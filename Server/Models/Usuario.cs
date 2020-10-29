using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ejemplo1.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ID { get; set; }
        [Required]
        public string ROL { get; set; }
        [Required]
        public string NOMBRE { get; set; }
        [Required]
        [EmailAddress]
        public string CORREO { get; set; }
        [Required]
        public string DOC_TYPE { get; set; }
        [Required]
        public string DOCUMENTO { get; set; }
        [Required]
        public string PASSWORD { get; set; }

    }
}