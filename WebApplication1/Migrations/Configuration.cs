namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication1.Utils;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApplication1.Models.ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.UserName == "i_Emmanuel")) {
                ApplicationUserManager.CrearUsuarioMaestro();
            }
            context.Amigos.AddOrUpdate(a => a.Nombre,
                   new Models.Amigo {
                        Nombre="Aleegons",
                        Imagen= "https://static-cdn.jtvnw.net/jtv_user_pictures/9c79bf4f-12fc-4c23-9b3d-da5aac423b18-profile_image-300x300.png",
                        LigaTwitch= "https://www.twitch.tv/Aleegons"
                   },
                   new Models.Amigo
                   {
                        Nombre = "watertd12",
                        Imagen = "https://static-cdn.jtvnw.net/jtv_user_pictures/60d81439-9c03-4b7b-b3bf-76781c5c74d2-profile_image-300x300.png",
                        LigaTwitch = "https://www.twitch.tv/watertd12"
                   }
            );
        }
    }
}
