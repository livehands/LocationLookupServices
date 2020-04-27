using LocationLookup.Helpers;
using LocationLookup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LocationLookup.Functions
{
    public static class LoadLocations
    {
        [FunctionName("LoadLocations")]
        public static async Task Run([BlobTrigger("mylocations/{name}", Connection = "BlobConnectionString")]string myBlob, [CosmosDB(
                databaseName: Constants.CosmosDbName,
                collectionName: Constants.MyLocationsCollection,
                ConnectionStringSetting = "AzureCosmosDBConnectionString")]IAsyncCollector<Hospital> document,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            List<Hospital> locations = JsonConvert.DeserializeObject<List<Hospital>>(myBlob);

            string BingMapsAPIKey = Environment.GetEnvironmentVariable("BingMapKey");

            HttpClient client = new HttpClient();
            int i = 0;
            foreach (Hospital h in locations)
            {
                try
                {
                    // Get Lat/Long from Bing
                    // Docs: https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-address
                    string bingReverseLookupTemplate = $"http://dev.virtualearth.net/REST/v1/Locations/US/{HttpUtility.UrlEncode(h.State)}/{HttpUtility.UrlEncode(h.Zip)}/{HttpUtility.UrlEncode(h.City)}/{HttpUtility.UrlEncode(h.Address)}?key={BingMapsAPIKey}";

                    string locInfo = await client.GetStringAsync(bingReverseLookupTemplate);
                    BingResponse bingresults = JsonConvert.DeserializeObject<BingResponse>(locInfo);

                    if (bingresults.StatusCode == 200)
                    {
                        ResourceSet bingLocationSet = bingresults.ResourceSets.FirstOrDefault();
                        Resource bingLocation = bingLocationSet.Resources.FirstOrDefault();
                        if (bingLocation.Point.Coordinates.Length > 1)
                        {
                            h.Location = new Point(bingLocation.Point.Coordinates[0], bingLocation.Point.Coordinates[1]);
                        }

                        //TODO: Get the Map for hte location.
                    }

                    await document.AddAsync(h);
                    log.LogInformation($"C# Blob trigger function Processed blob\n Name:{h.Name} \n FacilityID: {h.FacilityId}");
                    i++;
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Exception Adding item to collection!");
                }
            }

            log.LogInformation($"C# Blob trigger function ALL DONE Processed {i} items");
        }
    }
}
