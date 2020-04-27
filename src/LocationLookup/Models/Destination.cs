using Newtonsoft.Json;

namespace LocationLookup.Models
{
    public class Destination
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("location")]
        public Point Location { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("mapUri")]
        public string MapUri { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("distance")]
        public string Distance { get; set; }
    }
}
