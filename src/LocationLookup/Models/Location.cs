using Newtonsoft.Json;

namespace LocationLookup.Models
{
    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("point")]
        public Point Point { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("mapUri")]
        public string MapUri { get; set; }

        [JsonProperty("distance")]
        public string Distance { get; set; }

        [JsonProperty("distanceNumber")]
        public double DistanceNumber { get; set; }
    }
}
