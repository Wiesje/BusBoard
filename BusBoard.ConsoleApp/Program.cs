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
    static void Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        
        //  Ask user for relevant bus stop
        Console.WriteLine("What is the bus stop closest to you?");
        var busStopId = Console.ReadLine();
        
        // Create a new RestClient and RestRequest
        var client = new RestClient("https://api.tfl.gov.uk/");

        // Generate request for inputted bus stop
        var request = new RestRequest(string.Format("StopPoint/{0}/Arrivals", busStopId), Method.GET);
        request.RequestFormat = DataFormat.Json;
        
        // Sent the request to the web service and store the response
        var response = client.Execute<List<ArrivalsForStop>>(request);
        
        // Check the requested bus stop exists
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Bus stop id not found.");
            Console.WriteLine("Quit the program by pressing enter.");
            Console.ReadLine();
            Environment.Exit(1);
        }

        // Sort the returned arrivals by expected arrival time
        List<ArrivalsForStop> sortedList = response.Data.OrderBy(o => o.ExpectedArrival).ToList();

        // Display the next upcoming buses
        Console.WriteLine("The next 5 buses at stop {0} are:", busStopId);
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(sortedList[i].LineName);
            Console.WriteLine(sortedList[i].ExpectedArrival);
        }

        // Stop cmd from disappearing immediately
        Console.WriteLine("Quit the program by pressing enter.");
        Console.ReadLine();
    }
  }

  public class ArrivalsForStop
  {
    public string Id { get; set; }
    public int OperationType { get; set; }
    public string VehicleId { get; set; }
    public string NaptanId { get; set; }
    public string StationName { get; set; }
    public string LineId { get; set; }
    public string LineName { get; set; }
    public string PlatformName { get; set; }
    public string Direction { get; set; }
    public string Bearing { get; set; }
    public string DestinationNaptanId { get; set; }
    public string DestinationName { get; set; }
    public DateTime TimeStamp { get; set; }
    public int TimeToStation { get; set; }
    public string CurrentLocation { get; set; }
    public string Towards { get; set; }
    public DateTime ExpectedArrival { get; set; }
    public DateTime TimeToLive { get; set; }
    public string ModeName { get; set; }
    public Timing Timing { get; set; }

    }

  public class Timing
  {
    public string CountdownServerAdjustment { get; set; }
    public DateTime Source { get; set; }
    public DateTime Insert { get; set; }
    public DateTime Read { get; set; }
    public DateTime Sent { get; set; }
    public DateTime Received { get; set; }
    }
}
