using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PZ2.Model
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;
        private double latitude;
        private double longitude;
        private Model3D modelEntitet;

        public PowerEntity() { }

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
        public Model3D ModelEntitet { get => modelEntitet; set => modelEntitet = value; }
    }
}
