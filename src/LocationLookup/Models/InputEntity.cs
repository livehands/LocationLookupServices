using Newtonsoft.Json;

namespace LocationLookup.Models
{
    /// Used as the model to load the Cosmos DB.  Has just hte basics about the Destinations that will be compared against.
    /// This can be any model you need just update the LoadLocations function to use this type.
    public class InputEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("Zip")]
        public string Zip { get; set; }
        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("location")]
        public Point Location { get; set; }

    }
}
