using LocationLookup.Models;
using LocationLookup.Models.BingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocationLookup.Helpers
{
    public class BingHelper : ILocationApi
    {
        private string BingMapsAPIKey = Environment.GetEnvironmentVariable("BingMapKey");
        private HttpClient client;

        public BingHelper()
        {
            client = new HttpClient();
        }

        public async Task<IEnumerable<Destination>> GetItems(WayPoint userLocation, int numItems = 3)
        {
            List<Destination> locations = new List<Destination>();

            // Each Entity Type should be separated by comma ',' 
            // Type Ids are here: https://docs.microsoft.com/en-us/bingmaps/rest-services/common-parameters-and-types/type-identifiers/
            string entityTypes = "Hospitals";

            string bingEntitySearchTemplate = $"https://dev.virtualearth.net/REST/v1/LocalSearch/?type={entityTypes}&userLocation={userLocation.Coordinates}&key={BingMapsAPIKey}";

            string results = await client.GetStringAsync(bingEntitySearchTemplate);
            BingResponse bingresults = JsonConvert.DeserializeObject<BingResponse>(results);

            if (bingresults.StatusCode == 200)
            {
                ResourceSet bingLocationSet = bingresults.ResourceSets.FirstOrDefault();
                List<Resource> bingLocations = bingLocationSet.Resources.Take(numItems).ToList();

                Destination l;
                // Get the Map for the top 3 items
                foreach (Resource r in bingLocations)
                {
                    l = new Destination
                    {
                        Name = r.Name,
                        PhoneNumber = r.PhoneNumber,
                        Address = r.Address.FormattedAddress,
                        Location = r.Point,
                        Website = r.WebSite
                    };

                    locations.Add(l);
                }

            }

            return locations;
        }

        /// <summary>
        /// Get the Map with a highlighted route from Bing based on the two waypoints entered 
        /// and store in a public blob store for retrieval later.  Uses GUID to anonymize the file name.
        /// </summary>
        /// <param name="p1">WayPoint 1 which has a latitude & longitude, typically the users location</param>
        /// <param name="p2">WayPoint 2 which has a latitude & longitude, the destination location</param>
        /// <returns>Uri to the map stored in Blob storage</returns>
        public async Task<string> GetMapImageUrl(WayPoint p1, WayPoint p2)
        {
            string bingMapTemplate = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/Routes/?wp.0={0};26;1&wp.1={1};28;2&key={2}";

            //TODO: Ensure to deal with failures/errors.
            string requestedMapUri = string.Format(bingMapTemplate, p1.Coordinates, p2.Coordinates, BingMapsAPIKey);

            var map = await client.GetStreamAsync(requestedMapUri);

            //Upload file to the container
            AzureStorageConfig asc = new AzureStorageConfig
            {
                AccountKey = Environment.GetEnvironmentVariable("BlobAccountKey"),
                AccountName = Environment.GetEnvironmentVariable("BlobAccountName"),
                ImageContainer = Environment.GetEnvironmentVariable("BlobImageContainer")
            };

            string fileName = Guid.NewGuid().ToString() + ".png";

            string blobUrl = await StorageHelper.UploadFileToStorage(map, fileName, asc);

            //Write to the blob and get URI                        
            return blobUrl;
        }

        public async Task<string> GetRouteDistance(WayPoint p1, WayPoint p2)
        {
            string bingRouteTemplate = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&du=mi&key={2}";
            string requestedRouteUri = string.Format(bingRouteTemplate, p1.Coordinates, p2.Coordinates, BingMapsAPIKey);

            string routeData = await client.GetStringAsync(requestedRouteUri);

            BingResponse bingRouteResults = JsonConvert.DeserializeObject<BingResponse>(routeData);
            ResourceSet routeResourceSet = bingRouteResults.ResourceSets.FirstOrDefault();

            if (routeResourceSet != null)
            {
                Resource routeResource = routeResourceSet.Resources.FirstOrDefault();
                if (routeResource != null)
                {
                    return $"{routeResource.TravelDistance:0.0} mi.";
                }
            }

            return "Not Sure";
        }
    }
}
