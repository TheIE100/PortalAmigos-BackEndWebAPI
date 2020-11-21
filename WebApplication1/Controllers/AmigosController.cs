using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    //NOTA: WEV API SOLAMENTE PERMITE UN METODO GET,POST,PUT,DELETE POR CONTROLADOR SI PONES MAS DE UNO TOMA EL PRIMERO QUE SE CREO..
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

        [HttpPost]
        public ClassicResponse InsertarAmigo(Amigo amigoJson) {
            var respuesta = new ClassicResponse();
            try
            {
                var usuario = db.Users.Find(User.Identity.GetUserId());
                if (db.Amigos.Any(amigo => amigo.Nombre == amigoJson.Nombre))
                {
                    respuesta.Estatus = 300;
                    respuesta.Mensaje = "Username Repetido";
                }
                else
                {
                    db.Amigos.Add(amigoJson);
                    usuario.Amigos.Add(amigoJson);
                    db.SaveChanges();
                }
            }
            catch(Exception e) {
                respuesta.Estatus = 500;
                respuesta.Mensaje = $"Error en el servidor: {e}";
            }
            return respuesta;
        }

        public class ActualizarAmigoInput { 
            public Amigo AmigoRecibido { get; set; }
            public string IdentificadorRegistro { get; set; }
        }
        [HttpPut]
        public ClassicResponse ActualizarAmigo([FromBody] ActualizarAmigoInput input) {
            var amigoCliente = input.AmigoRecibido;
            var respuesta = new ClassicResponse();
            try
            {
                if (db.Amigos.Any(amigo => amigo.Nombre == amigoCliente.Nombre 
                && amigo.Nombre != input.IdentificadorRegistro))
                {
                    respuesta.Estatus = 300;
                    respuesta.Mensaje = "Username Repetido";
                }
                else
                {
                    var amigoDb = db.Amigos.FirstOrDefault(a => a.Nombre == input.IdentificadorRegistro);
                    amigoDb.Nombre = amigoCliente.Nombre;
                    amigoDb.LigaTwitch = amigoCliente.LigaTwitch;
                    amigoDb.Imagen = amigoCliente.Imagen;
                    db.Entry(amigoDb).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                respuesta.Estatus = 500;
                respuesta.Mensaje = $"Error en el servidor: {e}";
            }
            return respuesta;

        }


        [HttpDelete]
        public ClassicResponse BorrarAmigo(string identificadorRegistro)
        {
            var respuesta = new ClassicResponse();
            try
            {
                var amigoDb = db.Amigos.FirstOrDefault(a => a.Nombre == identificadorRegistro);
                db.Entry(amigoDb).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                respuesta.Estatus = 500;
                respuesta.Mensaje = $"Error en el servidor: {e}";
            }
            return respuesta;

        }



    }
}
