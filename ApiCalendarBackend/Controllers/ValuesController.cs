using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace ApiCalendarBackend.Controllers
{
    public class JsonTextWriterEx : JsonTextWriter
    {
        public string NewLine { get; set; }

        public JsonTextWriterEx(TextWriter textWriter) : base(textWriter)
        {
            NewLine = Environment.NewLine;
        }

        protected override void WriteIndent()
        {
            if (Formatting == Formatting.Indented)
            {
                WriteWhitespace(NewLine);
                int currentIndentCount = Top * Indentation;
                for (int i = 0; i < currentIndentCount; i++)
                    WriteIndentSpace();
            }
        }
    }

    public class ValuesController : ApiController
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

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
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
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
        // GET api/values
        public IEnumerable<string> Get()
        {
            return megafunction();
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
