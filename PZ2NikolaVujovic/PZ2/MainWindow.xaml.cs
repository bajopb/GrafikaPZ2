using PZ2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Dictionary<long, GeometryModel3D> pom = new Dictionary<long, GeometryModel3D>();
        private static Dictionary<LineEntity, List<GeometryModel3D>> pom2 = new Dictionary<LineEntity, List<GeometryModel3D>>();

        private const double najmanjaLat = 45.2325;
        private const double najvecaLat = 45.277031;
        private const double najmanjaLon = 19.793909;
        private const double najvecaLon = 19.894459;
        private double noviX, noviY;
        public List<PowerEntity> listaElemenataIzXML = new List<PowerEntity>();
        public List<LineEntity> listaVodova = new List<LineEntity>();
        public List<SwitchEntity> listaSviceva = new List<SwitchEntity>();
        List<GeometryModel3D> SwitchGeometryModels = new List<GeometryModel3D>();
        List<GeometryModel3D> NodeGeometryModels = new List<GeometryModel3D>();
        List<GeometryModel3D> SubstationGeometryModels = new List<GeometryModel3D>();
        Dictionary<GeometryModel3D, PowerEntity> elementi = new Dictionary<GeometryModel3D, PowerEntity>();
        private static Dictionary<System.Windows.Point, int> elementiNaIstojPoz = new Dictionary<System.Windows.Point, int>();
        private static Dictionary<long, PowerEntity> IDs = new Dictionary<long, PowerEntity>();
        private static Dictionary<long, int> connections = new Dictionary<long, int>();
        public List<SubstationEntity> substations = new List<SubstationEntity>();
        public List<NodeEntity> nodes = new List<NodeEntity>();
        public List<SwitchEntity> switches = new List<SwitchEntity>();
        public List<LineEntity> lines = new List<LineEntity>();
        public MainWindow()
        {
            InitializeComponent();
            UcitajXML();
            Crtanje();
        }
        public void Crtanje()
        {
            foreach (var sub in substations)
            {
                DrawElement(sub, Colors.Red, 1);
            }

            foreach (var node in nodes)
            {
                DrawElement(node, Colors.Green, 2);
            }

            foreach (var s in switches)
            {
                DrawElement(s, Colors.Purple, 3);
            }

            foreach (var line in lines)
            {

                DrawLine(line);
            }
        }
        public void DrawElement(PowerEntity entity, Color c, int i)
        {
            MeshGeometry3D meshGeometry3D = KreirajKocku(entity.Y, entity.X, entity.Id);
            DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(c));

            GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);
            geometryModel3D.SetValue(TagProperty, entity);
            elementi.Add(geometryModel3D, entity);
            if (i == 1)
                SubstationGeometryModels.Add(geometryModel3D);
            else if (i == 2)
                NodeGeometryModels.Add(geometryModel3D);
            else if (i == 3)
                SwitchGeometryModels.Add(geometryModel3D);

            pom.Add(entity.Id, geometryModel3D);
            model3dGroup.Children.Add(geometryModel3D);
        }
        private static MeshGeometry3D KreirajKocku(double Longitude, double Latitude, long EntityID)
        {
            var point = CreatePoint(Longitude, Latitude);

            int brojElem = 0;

            if (elementiNaIstojPoz.ContainsKey(point))
            {
                elementiNaIstojPoz[point]++;
                brojElem = elementiNaIstojPoz[point];
            }
            else
            {
                elementiNaIstojPoz[point] = 1;
                brojElem = 1;
            }

            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(point.X - 4, point.Y - 4, (brojElem) * 10 - 10));
            points.Add(new Point3D(point.X + 4, point.Y - 4, (brojElem) * 10 - 10));
            points.Add(new Point3D(point.X + 4, point.Y - 4, (brojElem) * 10));
            points.Add(new Point3D(point.X - 4, point.Y - 4, (brojElem) * 10));

            points.Add(new Point3D(point.X - 4, point.Y + 4, (brojElem) * 10));
            points.Add(new Point3D(point.X + 4, point.Y + 4, (brojElem) * 10));
            points.Add(new Point3D(point.X + 4, point.Y + 4, (brojElem) * 10 - 10));
            points.Add(new Point3D(point.X - 4, point.Y + 4, (brojElem) * 10 - 10));


            meshGeometry3D.Positions = new Point3DCollection(points);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(3);

            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(5);

            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(6);

            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(3);

            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(6);

            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(1);

            return meshGeometry3D;
        }
    
        private static System.Windows.Point CreatePoint(double lon, double lat)
        {
            double Lon = (najvecaLon - najmanjaLon) / 1175;
            double Lat = (najvecaLat - najmanjaLat) / 775;

            double x = Math.Round((lon - najmanjaLon) / Lon) - 587.5;
            double y = Math.Round((lat - najmanjaLat) / Lat) - 387.5;

            x = x - x % 8;
            y = y - y % 8;

            return new System.Windows.Point(x, y);
        }
        public void DrawLine(LineEntity line)
            {
                System.Windows.Point point1 = new System.Windows.Point();
                System.Windows.Point point2 = new System.Windows.Point();
                var br = 1;
                System.Windows.Point Startpoint = new System.Windows.Point();
                System.Windows.Point Endpoint = new System.Windows.Point();
                if (IDs.ContainsKey(line.FirstEnd) && IDs.ContainsKey(line.SecondEnd))
                {
                    Startpoint = CreatePoint(IDs[line.FirstEnd].Y, IDs[line.FirstEnd].X);
                    Endpoint = CreatePoint(IDs[line.SecondEnd].Y, IDs[line.SecondEnd].X);
                }
                foreach (var point in line.Vertices)
                {
                    if (br == 1)
                    {
                        point1 = CreatePoint(point.Y, point.X);
                        model3dGroup.Children.Add(createLineGeometryModel3D(Startpoint, point1, line));
                        br++;
                        continue;
                    }
                    else if (br == 2)
                    {
                        point2 = CreatePoint(point.Y, point.X);
                        model3dGroup.Children.Add(createLineGeometryModel3D(point1, point2, line));
                        point1 = point2;
                    }
                }
                model3dGroup.Children.Add(createLineGeometryModel3D(point2, Endpoint, line));
            }
        private static GeometryModel3D createLineGeometryModel3D(System.Windows.Point point1, System.Windows.Point point2, LineEntity line)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            List<Point3D> points = new List<Point3D>();

            points.Add(new Point3D(point1.X, point1.Y, 0));
            points.Add(new Point3D(point1.X, point1.Y, 6));
            points.Add(new Point3D(point1.X + 6, point1.Y, 6));
            points.Add(new Point3D(point1.X + 6, point1.Y, 0));

            points.Add(new Point3D(point2.X, point2.Y, 0));
            points.Add(new Point3D(point2.X, point2.Y, 6));
            points.Add(new Point3D(point2.X + 6, point2.Y, 6));
            points.Add(new Point3D(point2.X + 6, point2.Y, 0));



            meshGeometry3D.Positions = new Point3DCollection(points);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(1);


            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(4);


            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(5);




            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(0);


            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(0);


            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(1);



            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(1);


            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(6);


            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(6);

            DiffuseMaterial diffuseMaterial;
            if (line.ConductorMaterial == "Steel")
            {
                diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
            }
            else if (line.ConductorMaterial == "Copper")
            {
                diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.DarkGray));
            }
            else if (line.ConductorMaterial == "Acsr")
            {
                diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
            }
            else
            {
                diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightGray));
            }





            GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);
            geometryModel3D.SetValue(TagProperty, line);
            pom2[line].Add(geometryModel3D);



            return geometryModel3D;
        }
        private void UcitajXML() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            XmlNodeList nodeListLine = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode node in nodeListSubstation)
            {

                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out double x, out double y);
                if (x <= najmanjaLat || x >= najvecaLat || y <= najmanjaLon || y >= najvecaLon)
                {
                    continue;
                }
                else
                {
                    SubstationEntity substationEntity = new SubstationEntity();
                    substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                    substationEntity.Name = node.SelectSingleNode("Name").InnerText;
                    substationEntity.X = x;
                    substationEntity.Y = y;
                    substations.Add(substationEntity);
                    IDs.Add(substationEntity.Id, substationEntity);
                    connections.Add(substationEntity.Id, 0);
                }
            }

            foreach (XmlNode node in nodeListNode)
            {
                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out double x, out double y);
                if (x <= najmanjaLat || x >= najvecaLat || y <= najmanjaLon || y >= najvecaLon)
                {
                    continue;
                }
                else
                {
                    NodeEntity nodeEntity = new NodeEntity();
                    nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                    nodeEntity.Name = node.SelectSingleNode("Name").InnerText;
                    nodeEntity.X = x;
                    nodeEntity.Y = y;
                    nodes.Add(nodeEntity);
                    IDs.Add(nodeEntity.Id, nodeEntity);

                    connections.Add(nodeEntity.Id, 0);
                }
            }

            foreach (XmlNode node in nodeListSwitch)
            {
                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out double x, out double y);

                if (x <= najmanjaLat || x >= najvecaLat || y <= najmanjaLon || y >= najvecaLon)
                {
                    continue;
                }
                else
                {
                    SwitchEntity switchEntity = new SwitchEntity();
                    switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                    switchEntity.Name = node.SelectSingleNode("Name").InnerText;
                    switchEntity.Status = node.SelectSingleNode("Status").InnerText;
                    switchEntity.X = x;
                    switchEntity.Y = y;
                    switches.Add(switchEntity);
                    IDs.Add(switchEntity.Id, switchEntity);
                    connections.Add(switchEntity.Id, 0);
                }
            }

            foreach (XmlNode node in nodeListLine)
            {
                LineEntity lineEntity = new LineEntity();

                lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                lineEntity.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                    lineEntity.IsUnderground = true;
                else
                    lineEntity.IsUnderground = false;
                lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText, CultureInfo.InvariantCulture);
                lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText, CultureInfo.InvariantCulture);
                lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText, CultureInfo.InvariantCulture);
                lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText, CultureInfo.InvariantCulture);
                lineEntity.Vertices = new List<Model.Point>();
                foreach (XmlNode n in node.ChildNodes[9].ChildNodes)
                {
                    ToLatLon(double.Parse(n.SelectSingleNode("X")?.InnerText ?? string.Empty, CultureInfo.InvariantCulture), double.Parse(n.SelectSingleNode("Y")?.InnerText ?? string.Empty, CultureInfo.InvariantCulture), 34, out double x, out double y);
                    lineEntity.Vertices.Add(new Model.Point() { X = x, Y = y });

                }

                if (IDs.ContainsKey(lineEntity.SecondEnd) && IDs.ContainsKey(lineEntity.FirstEnd))
                {
                    lines.Add(lineEntity);
                    connections[lineEntity.SecondEnd]++;
                    connections[lineEntity.FirstEnd]++;
                    pom2.Add(lineEntity, new List<GeometryModel3D>());
                }



            }
        }

        private void ViewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void ViewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

    }
}
