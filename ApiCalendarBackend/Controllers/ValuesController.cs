using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Text;
using System.Threading.Tasks;

namespace ApiCalendarBackend.Controllers
{
    public class TestController : ApiController
    {
        // Controller methods not shown...
    }
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly, GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Google Calendar amd Gmail API .NET Quickstart";

        string countdown(string string1, string string2)
        {
        
            var hoy = new DateTime();
            hoy = DateTime.Now;

            double dias = 0;
            double horas = 0;
            double minutos = 0;
            double segundos = 0;
            var fechat = "";
            DateTime date1 = new DateTime();
            if(string1!="")
            {
                System.Diagnostics.Debug.WriteLine("DEBUGEO " + string1);
                date1 = DateTime.ParseExact(string1, "yyyy-MM-dd HH:mm",
                                  System.Globalization.CultureInfo.CurrentCulture);
                fechat = string1;

            }
            if (string2 != "")
            {
                System.Diagnostics.Debug.WriteLine("DEBUGEO " + string2);
                date1 = DateTime.ParseExact(string2, "yyyy-MM-dd",
                                  System.Globalization.CultureInfo.CurrentCulture);

                fechat = string2;

            }



          var diferencia = date1 - hoy;
                dias = Math.Floor(diferencia.TotalDays);
               horas = Math.Floor(diferencia.TotalHours);
               minutos = (diferencia.Minutes);
                 segundos = (diferencia.Seconds);


            //  console.log(hoy + "-" + fechaevento);
            //fechat = "";
            if (segundos < 0)
                fechat = "En estos momentos :";
            if (minutos < 0)
                fechat = "En estos momentos :";
            if (horas < 0)
            {
                fechat = "En estos momentos :";
            }
         


            if (segundos > 0)
                    fechat = "En " + segundos + " segundos :";
                if (minutos > 0)
                    fechat = "En " + minutos + " minutos :";
                if (horas > 0)
                {
                        fechat = "En " + horas + " horas :";
                }
                if (dias > 0)
                fechat = "En " + dias + " Dias :";
                

                
                return fechat;

        }

        List<Threads> obtenerCorreos()
        {
            List<Threads> correos = new List<Threads>();
            UserCredential credential;

            using (var stream =
                new FileStream(HttpContext.Current.Server.MapPath("~/credentials2.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = HttpContext.Current.Server.MapPath("~/token2.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            List<String> xd =new List<String>();
            xd.Add("0");
            xd.Add("1");
            // Define parameters of request
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List("me");

            // List labels.
            var labels = request.Execute().Threads;
            //Console.WriteLine("Labels:");
            if (labels != null && labels.Count > 0)
            {
                foreach (var labelItem in labels)
                {

                    //labelItem.ToString();
                    Threads correo = new Threads();
                    correo.desc = labelItem.Snippet;
                    
                     
                    correos.Add(correo);
                    //correos.Add
                 //   Console.WriteLine("{0}", labelItem.Name);
                }
            }
            else
            {
                Threads correo = new Threads();
                correo.desc = "no hay correos";
                correos.Add(correo);
                //   Console.WriteLine("No labels found.");
            }
            return correos;
        }

        List<Event> megafunction()
        {
            CultureInfo ci = new CultureInfo("es-PE");
            ci.DateTimeFormat.ShortDatePattern = "yyyy'-'MM'-'dd";

            ci.DateTimeFormat.LongTimePattern = "hh':'mm";

            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            
            
            List<Event> megaCadena = new List<Event>();
            UserCredential credential;

            using (var stream =
                new FileStream(HttpContext.Current.Server.MapPath("~/credentials.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = HttpContext.Current.Server.MapPath("~/token.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
       //         Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();


            //  Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    
                  
                    var dt = DateTime.Now;
                    string s = dt.ToString();

                    string when = eventItem.Start.DateTime.ToString();
                    
                    if (String.IsNullOrEmpty(when))
                    {
                        when = countdown("", eventItem.Start.Date);

                    }
                    else
                    {
                        when = countdown(when, "");

                    }

                    //        Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    Event even = new Event();
                    even.date = when;
                    even.desc = eventItem.Summary;
                    megaCadena.Add(even);

                }


                //megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(events.Items);
                // megaCadena = megaCadena.Replace("\"", " ");
                // megaCadena = megaCadena.Substring(1, megaCadena.Length); ;
     
              
            }
            else
            {
                Event even = new Event();
                even.date = "";
                even.desc = "no hay eventos";
                megaCadena.Add(even);
           //     Console.WriteLine("No upcoming events found.");
            }
      //      Console.Read();
            return megaCadena;
        }

        public class Event
        {
            public String desc { get; set; }
            public String date { get; set; }
        }

        public class Threads
        {
            public String desc { get; set; }
        }
        public class RootObject
        {
            public List<Threads> labels { get; set; }
            public List<Event> events { get; set; }
        }
        // GET api/values
        public RootObject Get()
        {


              RootObject obj = new RootObject();
            //  List<Event> events = new List<Event>();
            // List<Labels> labels = new List<Labels>();


            obj.events = megafunction();
            obj.labels = obtenerCorreos();
         //      String megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
           //   RootObject objf = JsonConvert.DeserializeObject<RootObject>(megaCadena);
            
           //String megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(megafunction());
           
          
            return obj; 
        }

        // GET api/values/5
        public Event Get(int id)
        {
            

            return megafunction().ElementAt(id); 
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
