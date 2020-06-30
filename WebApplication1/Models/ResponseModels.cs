using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ClassicResponse { 
        public int Estatus { get; set; }
        public string Mensaje { get; set; }

        public ClassicResponse() {
            this.Estatus = 200;
            this.Mensaje = "Ok";
        }
    }

    public class DescargaAmigosResponse : ClassicResponse { 
        public List<Amigo> ListaAmigos { get; set; }

        public DescargaAmigosResponse() : base() {
            this.ListaAmigos = new List<Amigo>();
        }
    
    }
}