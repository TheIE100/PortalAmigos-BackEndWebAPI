using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AmigosController : ApiController
    {

        [HttpGet]
        public DescargaAmigosResponse DescargaAmigos() {
            var respuesta = new DescargaAmigosResponse();
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var usuario = db.Users.Find(User.Identity.GetUserId());
                    respuesta.ListaAmigos = new List<Amigo>(usuario.Amigos);
                }
            }
            catch(Exception e) {
                respuesta.Estatus = 500;
                respuesta.Mensaje = $"Error en el servidor: {e.ToString()}";
            }
            return respuesta;
        } 

    }
}
