using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ApiCalendarBackend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            // Configuración y servicios de API web
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            var formatter = GlobalConfiguration.Configuration.Formatters.Where(f => f is System.Net.Http.Formatting.JsonMediaTypeFormatter).FirstOrDefault();
            if (!formatter.SupportedMediaTypes.Any(mt => mt.MediaType == "text/plain"))
                formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));


            //Ignorar self-references
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
               Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            //Ignorar valores null
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;


            //CORS

            config.EnableCors();

           
           

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
