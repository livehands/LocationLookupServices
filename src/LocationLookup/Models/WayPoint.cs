using System;
using System.Collections.Generic;
using System.Text;

namespace LocationLookup.Models
{
    public class WayPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Coordinates => $"{Latitude},{Longitude}";
        public WayPoint()
        {

        }

        public WayPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public WayPoint(Point point)
        {
            Latitude = point.Coordinates[0];
            Longitude = point.Coordinates[1];
        }
    }
}
