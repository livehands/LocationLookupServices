using System;
using System.Collections.Generic;
using System.Text;

namespace LocationLookup.Models.BingModels
{
    public class Resource
    {
        public string Name { get; set; }
        public Point Point { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public string DistanceUnit { get; set; }
        public double TravelDistance { get; set; }
    }
}
