using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using RestSharp;

namespace BusBoard.ConsoleApp
{
    class Program
    {
        // Create a new RestClient and RestRequest
        static readonly RestSharp.RestClient Client = new RestClient("https://api.tfl.gov.uk/");

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Look up bus stop closest to location
            // string busStopId = GetBusStopId(postcode);

            // Display the next 5 buses from stop
            GetNextFiveFromStop();
        }

        static void GetNextFiveFromStop()
        {
            IRestResponse<List<ArrivalsForStop>> response;
            string busStopId;
            do
            {
                busStopId = AskForPostcode();
                response = SendRequest(busStopId);
            } while (!CheckStopExists(response));

            DisplayFirstFive(response, busStopId);

            QuitProgram(0);
        }

        static string AskForPostcode()
        {
            //  Ask user for postcode
            Console.WriteLine("What is your postcode?");
            var postcode = Console.ReadLine();
            return postcode;
        }

        static bool CheckStopExists(IRestResponse<List<ArrivalsForStop>> response)
        {
            // Check the requested bus stop exists
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Bus stop id not found.");
                return false;
            }
            return true;
        }

        static void DisplayFirstFive(IRestResponse<List<ArrivalsForStop>> response, string busStopId)
        {
            // Sort the returned arrivals by expected arrival time
            List<ArrivalsForStop> sortedList = response.Data.OrderBy(o => o.ExpectedArrival).ToList();

            // Display the next upcoming buses
            Console.WriteLine("The next 5 buses at stop {0} are:", busStopId);
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(sortedList[i].LineName);
                Console.WriteLine(sortedList[i].ExpectedArrival);
            }
        }

        static void QuitProgram(int exitcode)
        {
            // Quit program
            Console.WriteLine("Quit the program by pressing enter.");
            Console.ReadLine();
            Environment.Exit(exitcode);
        }

        static IRestResponse<List<ArrivalsForStop>> SendRequest(string busStopId)
        {
            // Generate request for inputted bus stop
            var request = new RestRequest(string.Format("StopPoint/{0}/Arrivals", busStopId), Method.GET);
            request.RequestFormat = DataFormat.Json;

            // Sent the request to the web service and store the response
            var response = Client.Execute<List<ArrivalsForStop>>(request);
            return response;
        }

        /*
        static string GetBusStopId(string postcode)
        {
            var busStopId;
    
    
    
            return busStopId;
        }
        */
    }

    public class ArrivalsForStop
    {
        public string LineName { get; set; }
        public DateTime ExpectedArrival { get; set; }
    }
}
