using Newtonsoft.Json;

namespace LocationLookup.Models
{
    public class Point
    {
        [JsonProperty("type")]
        public string Type => "Point";
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        public Point()
        {

        }

        public Point(double latitude, double longitude)
        {
            Coordinates = new double[] { latitude, longitude };
        }
    }
}
