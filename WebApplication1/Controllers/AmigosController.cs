using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AmigosController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public JsonResult<DescargaAmigosResponse> DescargaAmigos() {
            var respuesta = new DescargaAmigosResponse();
            try
            {
                var usuario = db.Users.Find(User.Identity.GetUserId());
                respuesta.ListaAmigos = new List<Amigo>(usuario.Amigos);
            }
            catch (Exception e)
            {
                respuesta.Estatus = 500;
                respuesta.Mensaje = $"Error en el servidor: {e.ToString()}";
            }
            return Json(respuesta);
        } 

    }
}
