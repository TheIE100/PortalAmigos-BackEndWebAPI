using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        /// <summary>
        /// Método que sirve para realizar alguna modificación general del sistema cuando se comienza la
        /// ejecucion de un request (1 paso antes).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //COLOCANDO EL IDIOMA ESPAÑOL PARA QUE EN LAS VALIDACIONES SE MUESTRE EN ESTE IDIOMA
            CultureInfo culture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;


            // Cobfigyuracion de request para aplicaciones extenar - ejemplo> ANGUKAR 
          //  HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Origin", "*");  //porque ya lo pusimos en el en el web config

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {


                HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Methods", "*");
                // If any http headers are shown in preflight error in browser console add them below
                HttpContext.Current.Response.Headers.Set("Access-Control-Allow-Headers", "*");
                HttpContext.Current.Response.End();
            }

        }
    }
}
