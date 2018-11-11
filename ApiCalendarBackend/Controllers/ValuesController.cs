using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace ApiCalendarBackend.Controllers
{

    public class ValuesController : ApiController
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        string countdown(string fechaevento)
        {


         
            var hoy = new DateTime();
            hoy = DateTime.Now;

            double dias = 0;
            double horas = 0;
            double minutos = 0;
            double segundos = 0;
            
            var date1 = new DateTime();
            date1 = DateTime.ParseExact("2018-10-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
                                       System.Globalization.CultureInfo.InvariantCulture);

            double diferencia = (date1 - hoy).TotalDays / 1000;
            dias = Math.Floor(diferencia / 86400);
            diferencia = diferencia - (86400 * dias);
            horas = Math.Floor(diferencia / 3600);
            diferencia = diferencia - (3600 * horas);
            minutos = Math.Floor(diferencia / 60);
            diferencia = diferencia - (60 * minutos);
            segundos = Math.Floor(diferencia);



          //  console.log(hoy + "-" + fechaevento);
            var fechat = "";


            if (segundos > 0)
                fechat = "Quedan " + segundos + " segundos :";
            if (minutos > 0)
                fechat = "Quedan " + minutos + " minutos :";
            if (horas > 0)
            {
                if (date1.Hour > 12)
                    fechat = "Hoy a las " + date1.Hour + " PM :";
                else
                    fechat = "Hoy a las " + date1.Hour + " AM :";
            }
            if (dias > 0)
                fechat = "En " + dias + " Dias :";

            return fechat;


        }



        List<string> megafunction()
        {
            List<string> megaCadena = new List<string>();
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
                    string when = eventItem.Start.DateTime.ToString();
                    string whenconv = "";
                    if (String.IsNullOrEmpty(when))
                    {
                         when = eventItem.Start.Date;
                       // whenconv = countdown(when);

                    }
                    //        Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    megaCadena.Add(eventItem.Summary + " " + when);

                }


                //megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(events.Items);
                // megaCadena = megaCadena.Replace("\"", " ");
                // megaCadena = megaCadena.Substring(1, megaCadena.Length); ;
     
              
            }
            else
            {
                megaCadena.Add("no hay eventos");
           //     Console.WriteLine("No upcoming events found.");
            }
      //      Console.Read();
            return megaCadena;
        }


        public class RootObject
        {
            public List<String> events { get; set; }
        }
        // GET api/values
        public string Get()
        {
         
        
           RootObject obj = new RootObject();
            List<String> events = new List<String>();
            

            obj.events = megafunction();
            String megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
          //  RootObject objf = JsonConvert.DeserializeObject<RootObject>(megaCadena);

            return megaCadena;
        }

        // GET api/values/5
        public string Get(int id)
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
