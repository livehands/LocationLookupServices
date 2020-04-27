using LocationLookup.Models;
using LocationLookup.Models.BingModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LocationLookup.Functions
{
    public static class FindByZip
    {
        [FunctionName("FindByZip")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "zip/{zipCode}")] HttpRequest req,
            string zipCode,
            ILogger log)
        {
            log.LogInformation($"FindByZip: HTTP trigger Find By Zip {zipCode}");

            string BingMapsAPIKey = Environment.GetEnvironmentVariable("BingMapKey");

            string baseAddress = $"{req.Scheme}://{req.Host.Value}";
            string locationLookupUrl = baseAddress + "/api/custom/{0}/{1}";

            // Get the Lat/Long for the Zip Code from Bing
            string reverseLookupTemplate = $"http://dev.virtualearth.net/REST/v1/Locations/US/{HttpUtility.UrlEncode(zipCode)}?key={BingMapsAPIKey}";
            HttpClient client = new HttpClient();

            string locInfo = await client.GetStringAsync(reverseLookupTemplate);
            BingResponse bingresults = JsonConvert.DeserializeObject<BingResponse>(locInfo);
            WayPoint userLocation = null;

            if (bingresults.StatusCode == 200)
            {
                ResourceSet bingLocationSet = bingresults.ResourceSets.FirstOrDefault();
                Resource bingLocation = bingLocationSet.Resources.FirstOrDefault();
                if (bingLocation.Point.Coordinates.Length > 1)
                {
                    userLocation = new WayPoint(bingLocation.Point);
                }

                if (userLocation != null)
                {
                    // Get the locations closest to the Zip
                    var data = await client.GetStringAsync(string.Format(locationLookupUrl, userLocation.Latitude, userLocation.Longitude));
                    return new OkObjectResult(data);
                }
            }

            log.LogInformation($"FindByZip: Complted HTTP trigger Find By Zip {zipCode}");

            return new OkResult();
        }
    }
}
