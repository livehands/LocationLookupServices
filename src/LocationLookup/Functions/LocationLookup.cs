using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LocationLookup.Models;
using System.Collections.Generic;
using LocationLookup.Helpers;

namespace LocationLookup.Functions
{
    public static class LocationLookup
    {
        [FunctionName("LocationLookup")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "nearest/{latitude}/{longitude}")] HttpRequest req,
            [CosmosDB(Constants.CosmosDbName,
                      Constants.MyLocationsCollection,
                      CreateIfNotExists = true,
                      ConnectionStringSetting = "AzureCosmosDBConnectionString",
            SqlQuery ="SELECT * FROM locations hospital WHERE ST_DISTANCE(hospital.location, {{ 'type': 'Point', 'coordinates':[ {latitude},{longitude}]}}) < 16000"
            )] IEnumerable<Hospital> hospitals,
            double latitude,
            double longitude,
            ILogger log)
        {
            List<Location> locationResults = new List<Location>();

            try
            {
                WayPoint userLocation = new WayPoint
                {
                    Longitude = longitude,
                    Latitude = latitude
                };

                BingHelper bh = new BingHelper();

                string mapUrl;
                string distance;

                foreach (Hospital h in hospitals)
                {

                    WayPoint wp = new WayPoint
                    {
                        Latitude = h.Location.Coordinates[0],
                        Longitude = h.Location.Coordinates[1]
                    };

                    mapUrl = await bh.GetMapImageUrl(userLocation, wp);
                    distance = await bh.GetRouteDistance(userLocation, wp);

                    Location l = new Location()
                    {
                        Address = h.Address,
                        Name = h.Name,
                        Telephone = h.PhoneNumber,
                        Point = h.Location,
                        MapUri = mapUrl,
                        Distance = distance
                    };

                    locationResults.Add(l);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            return new OkObjectResult(locationResults);
        }
    }
}
