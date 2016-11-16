
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

namespace CalendarQuickstart
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Google Calendar Test";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //CreateTest(service);
            //DeleteTest(service);

             
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            //request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var e in events.Items)
                {
                    string when = e.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = e.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", e.Summary, when);
//                    if(e.Summary == "Test")
                    {
                        service.Events.Delete("primary", e.Id).Execute();
                        Console.WriteLine("Deleted {0} ({1})", e.Summary, when);
                    }
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
            Console.Read();

        }

        static void CreateTest(CalendarService service)
        {
            Event newEvent = new Event()
            {
                Summary = "Test",
                Start = new EventDateTime()
                {
                    DateTime = new DateTime(2016, 11, 16, 9, 0, 0),
                    TimeZone = "Asia/Singapore",
                },
                End = new EventDateTime()
                {
                    DateTime = new DateTime(2016, 11, 16, 10, 0, 0),
                    TimeZone = "Asia/Singapore",
                },
            };
            service.Events.Insert(newEvent, "primary").Execute();
            Console.WriteLine("Event created: {0}", newEvent.HtmlLink);
        }

        static void DeleteTest(CalendarService service)
        {
            service.Events.Delete("primary", "eventID").Execute();
            Console.WriteLine("Event deleted.");
        }
    }
}