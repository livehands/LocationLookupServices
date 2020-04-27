using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LocationLookup.Helpers;
using System.Collections.Generic;
using System.Linq;
using LocationLookup.Models;

namespace LocationLookup.Functions
{
    public static class LookupNoMap
    {
        [FunctionName("LookupNoMap")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "near-nomap/{latitude}/{longitude}/{distance:int=16000}")] HttpRequest req,
            [CosmosDB(Constants.CosmosDbName,
                      Constants.MyLocationsCollection,
                      CreateIfNotExists = true,
                      ConnectionStringSetting = "AzureCosmosDBConnectionString",
            SqlQuery ="SELECT * FROM locations l WHERE ST_DISTANCE(l.location, {{ 'type': 'Point', 'coordinates':[ {latitude},{longitude}]}}) < {distance}"
            )] IEnumerable<dynamic> destinations,
            ILogger log)
        {
            log.LogInformation("Location Lookup Started");
            int returnCount;

            try
            {
                string countParam = req.Query["count"];
                if (!string.IsNullOrWhiteSpace(countParam) && countParam.CompareTo("all") == 0)
                {
                    returnCount = destinations.Count();
                }
                else
                {
                    int.TryParse(countParam, out returnCount);

                    if (returnCount < 1)
                    {
                        returnCount = int.Parse(Environment.GetEnvironmentVariable("DefaultReturnCount"));
                    }
                }

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            log.LogInformation("Location Lookup Complete");
            return new OkObjectResult(destinations.Take(returnCount));
        }
    }
}
