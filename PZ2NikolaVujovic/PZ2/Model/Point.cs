using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Model
{
    public class Point
    {
        private double x;
        private double y;
        private double latitude;
        private double longitude;

        public Point() { }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
    }
}
