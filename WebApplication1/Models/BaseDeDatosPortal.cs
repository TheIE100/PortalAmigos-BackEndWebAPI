using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public class Amigo { 
        [Key]
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string LigaTwitch { get; set; }

        public List<ApplicationUser> Usuarios { get; set; }

    }

}