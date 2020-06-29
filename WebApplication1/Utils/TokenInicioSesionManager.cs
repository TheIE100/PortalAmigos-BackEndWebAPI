

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using Microsoft.Owin;

namespace WebApplication1.Utils
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static IdentityResult CrearUsuarioMaestro()
        {
            var user = new ApplicationUser
            {
                UserName = "i_Emmanuel",
                Email = "i_emmanuel95@hotmail.com"
            };
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            ApplicationUserManager _userManager = new ApplicationUserManager(store);
            var manger = _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var resultado = manger.Create(user, "Twitch123.");
            return resultado;

        }
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure la lógica de validación de nombres de usuario
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure la lógica de validación de contraseñas
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configurar valores predeterminados para bloqueo de usuario
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registre proveedores de autenticación en dos fases. Esta aplicación usa los pasos Teléfono y Correo electrónico para recibir un código para comprobar el usuario
            // Puede escribir su propio proveedor y conectarlo aquí.
            manager.RegisterTwoFactorProvider("Código telefónico", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Su código de seguridad es {0}"
            });
            manager.RegisterTwoFactorProvider("Código de correo electrónico", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Código de seguridad",
                BodyFormat = "Su código de seguridad es {0}"
            });
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
    public class TokenInicioSesionManager : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public TokenInicioSesionManager(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        //1.
        /// <summary>
        /// Recibe los argumentos recibidos en el query string para realizar la validación de este (PRIMER PASO ANTES QUE TODO)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
               
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        //2.
        /// <summary>
        /// RECIBE LA PETICIÓN YA CUANDO FUE VALIDADA EN 'ValidateClientAuthentication' CUANDO EL USUARIO manda CON UN QUERY STRING! (USUARIO Y CONTRASEÑA)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            //string androidId = context.OwinContext.Get<string>("android_id");
            if (user == null)
            {
                context.SetError("ContraseniaCorreoInvalido", "The user name or password is incorrect."); //mensaje de error, luciría así --> {  "error": "ContraseniaCorreoInvalido","error_description": "The user name or password is incorrect."  }
                return;
            }
            else
            { //logró inicar sesión correctamente
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket); //aqui se genera el token tras banbalinas
                context.Request.Context.Authentication.SignIn(cookiesIdentity);

            }
        }


        //4.
        /// <summary>
        /// Crea propiedades que más adelante se van agregar en el método "TokenEndpoint" 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //5.
        /// <summary>
        /// Este método recibe un contexto y lo que hace es
        /// agregar al json las propiedades del diccionario de parametro que va mandar 
        /// como respuesta en caso de inicio de sesión exitoso y ajustar propiedades del token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            if (context.TokenIssued)
            {
                // client information
                var accessExpiration = DateTimeOffset.Now.AddSeconds(3600 * 24); //token de 1 dia
                context.Properties.ExpiresUtc = accessExpiration;
            }


            return Task.FromResult<object>(null);
        }

        //6.
        /// <summary>
        /// Este método se llama antes de enviar la respuesta al cliente acorde a la documentación de Microsoft.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            return base.TokenEndpointResponse(context);
        }
    }
}