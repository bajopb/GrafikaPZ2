using PZ2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public double noviX, noviY;
        // donji levi ugao
        public const double minLongitude = 19.793909;
        public const double minLatitude = 45.2325;
        // gornji desni 
        public const double maxLongitude = 19.894459;
        public const double maxLatitude = 45.277031;
        // liste za iscitavanje iz xmla
        public List<LineEntity> listaVodova = new List<LineEntity>();



        public List<NodeEntity> listaNodova = new List<NodeEntity>();
        public List<SubstationEntity> listaSubstationa = new List<SubstationEntity>();
        public List<SwitchEntity> listaSwitcheva = new List<SwitchEntity>();
        // za testiranje 
        public List<NodeEntity> listaNodovaProba = new List<NodeEntity>();
        public List<SubstationEntity> listaSubstationaProba = new List<SubstationEntity>();
        public List<SwitchEntity> listaSwitchevaProba = new List<SwitchEntity>();

        // za dodatni zadatak
        //public List<GeometryModel3D> SwitchGeometryModel = new List<GeometryModel3D>();
        //public List<GeometryModel3D> NodeGeometryModel = new List<GeometryModel3D>();
        //public List<GeometryModel3D> SubstationsGeometryModel = new List<GeometryModel3D>();
        public List<GeometryModel3D> steelVodovi = new List<GeometryModel3D>();
        public List<GeometryModel3D> copperVodovi = new List<GeometryModel3D>();
        public List<GeometryModel3D> acsrVodovi = new List<GeometryModel3D>();


        //
        private static Dictionary<long, PowerEntity> collectionOfNodesID = new Dictionary<long, PowerEntity>();
        // da vidim da li se nalazi na istom mestu 
        private static Dictionary<System.Windows.Point, int> numberOfEntityOnPoint = new Dictionary<System.Windows.Point, int>();
        //menjanje boje switcheva
        Dictionary<GeometryModel3D, long> allEntities = new Dictionary<GeometryModel3D, long>();

        Dictionary<GeometryModel3D, long> allVodes = new Dictionary<GeometryModel3D, long>();
        //tooltip
        System.Windows.Point mousePositionForToolTip = new System.Windows.Point();
        ToolTip toolTip = new ToolTip();
        private GeometryModel3D hitgeo;
        Dictionary<GeometryModel3D, PowerEntity> geometryAndEntyty = new Dictionary<GeometryModel3D, PowerEntity>();
        Dictionary<GeometryModel3D, LineEntity> geometryAndVod = new Dictionary<GeometryModel3D, LineEntity>();
        //skroll za zoom 
        private int zoomMax = 50;
        private int zoomCurent = 30;
        int currentZoom = 1;
        int maxZoom = 20;
        int minZoom = -5;
        //pan
        private System.Windows.Point start = new System.Windows.Point();
        private System.Windows.Point diffOffset = new System.Windows.Point();
        private bool MiddleClicked = false;
        DoubleAnimation doubleAnimation = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(15));
        //neaktivan deo 
        static Dictionary<long, SwitchEntity> switches = new Dictionary<long, SwitchEntity>();

        static List<string> materijali = new List<string>();
        static Dictionary<int, int> otpornosti = new Dictionary<int, int>();
        //rotiranje
        private System.Windows.Point startPosition = new System.Windows.Point();
        public List<PowerEntity> allXmlElements = new List<PowerEntity>();
        public List<LineEntity> allLines = new List<LineEntity>();

        public MainWindow()
        {
            InitializeComponent();

            ParseXml();
            crtajElemente();
            crtajVodove();
        }

        private void crtajVodove() {
            foreach (var line in listaVodova)
            {
                System.Windows.Point point1 = new System.Windows.Point();
                System.Windows.Point point2 = new System.Windows.Point();
                var temp = 1;
                System.Windows.Point Startpoint = new System.Windows.Point();
                System.Windows.Point Endpoint = new System.Windows.Point();
                GeometryModel3D vod = new GeometryModel3D();
                GeometryModel3D vodDeo1 = new GeometryModel3D();
                GeometryModel3D vodDeo2 = new GeometryModel3D();
                GeometryModel3D vodDeo3 = new GeometryModel3D();
                DiffuseMaterial boja = new DiffuseMaterial();

                if (collectionOfNodesID.ContainsKey(line.FirstEnd) && collectionOfNodesID.ContainsKey(line.SecondEnd))
                {
                    Startpoint = CreatePoint(collectionOfNodesID[line.FirstEnd].Longitude, collectionOfNodesID[line.FirstEnd].Latitude);
                    Endpoint = CreatePoint(collectionOfNodesID[line.SecondEnd].Longitude, collectionOfNodesID[line.SecondEnd].Latitude);

                }
                else
                {
                    continue;
                }

                // na osnovu materijala 
                if (line.ConductorMaterial == "Steel")
                {
                    boja = new DiffuseMaterial(System.Windows.Media.Brushes.Green);
                }
                else if (line.ConductorMaterial == "Acsr")
                {
                    boja = new DiffuseMaterial(System.Windows.Media.Brushes.Red);
                }
                else if (line.ConductorMaterial == "Copper")
                {
                    boja = new DiffuseMaterial(System.Windows.Media.Brushes.Black);
                }

                foreach (var point in line.Vertices)
                {
                    if (temp == 1)
                    {
                        point1 = CreatePoint(point.Longitude, point.Latitude);
                        vodDeo1 = createLineGeometryModel3D(Startpoint, point1, boja);
                        model3dGroup.Children.Add(vodDeo1);
                        geometryAndVod.Add(vodDeo1, line);
                        temp++;
                        continue;
                    }
                    else if (temp == 2)
                    {
                        point2 = CreatePoint(point.Longitude, point.Latitude);
                        vodDeo2 = createLineGeometryModel3D(point1, point2, boja);
                        model3dGroup.Children.Add(vodDeo2);
                        geometryAndVod.Add(vodDeo2, line);
                        point1 = point2;
                    }

                    if (line.ConductorMaterial == "Steel")
                    {
                        //steelVodovi.Add(vod);
                        steelVodovi.Add(vodDeo1);
                        steelVodovi.Add(vodDeo2);
                    }
                    else if (line.ConductorMaterial == "Acsr")
                    {
                        //acsrVodovi.Add(vod);
                        acsrVodovi.Add(vodDeo1);
                        acsrVodovi.Add(vodDeo2);
                    }
                    else if (line.ConductorMaterial == "Copper")
                    {
                        //copperVodovi.Add(vod);
                        copperVodovi.Add(vodDeo1);
                        copperVodovi.Add(vodDeo2);
                    }

                }


                //geometryAndVod.Add(vodDeo1, line);
                //geometryAndVod.Add(vodDeo2, line);

                vod = createLineGeometryModel3D(point2, Endpoint, boja);

                allVodes.Add(vod, line.Id);
                geometryAndVod.Add(vod, line);


                if (line.ConductorMaterial == "Steel")
                {
                    steelVodovi.Add(vod);
                    //steelVodovi.Add(vodDeo1);
                    //steelVodovi.Add(vodDeo2);
                }
                else if (line.ConductorMaterial == "Acsr")
                {
                    acsrVodovi.Add(vod);
                    //acsrVodovi.Add(vodDeo1);
                    //acsrVodovi.Add(vodDeo2);
                }
                else if (line.ConductorMaterial == "Copper")
                {
                    copperVodovi.Add(vod);
                    //copperVodovi.Add(vodDeo1);
                    //copperVodovi.Add(vodDeo2);
                }


                model3dGroup.Children.Add(vod);

                allEntities.Add(vod, line.Id);
            }
        }

        private void ParseXml()
        {
            double longit = 0; // izlaz iz ToLatLon funkcije 
            double latid = 0;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");
            XmlNodeList nodeList;
            List<SubstationEntity> subis = new List<SubstationEntity>();
            List<NodeEntity> nobis = new List<NodeEntity>();
            List<SwitchEntity> switchis = new List<SwitchEntity>();
            List<LineEntity> linis = new List<LineEntity>();

            var filename = "Geographic.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = System.IO.Path.Combine(currentDirectory, filename);
            StringBuilder result = new StringBuilder();
            XDocument xdoc = XDocument.Load(filename);

            #region substations
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {

                SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);


                subis.Add(sub); // dodat odmah iz xml
                allXmlElements.Add(sub);
            }

            for (int i = 0; i < subis.Count(); i++)
            {
                listaSubstationaProba.Add(subis[i]);
                var item = subis[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid >= minLatitude && latid <= maxLatitude && longit >= minLongitude && longit <= maxLongitude)
                {
                    subis[i].Latitude = latid;
                    subis[i].Longitude = longit;
                    listaSubstationa.Add(subis[i]);
                    collectionOfNodesID.Add(subis[i].Id, subis[i]);
                }

            }
            #endregion

            #region nodes
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {

                NodeEntity nodeobj = new NodeEntity();
                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                nobis.Add(nodeobj);
                allXmlElements.Add(nodeobj);

            }
            for (int i = 0; i < nobis.Count(); i++)
            {
                listaNodovaProba.Add(nobis[i]);
                var item = nobis[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid >= minLatitude && latid <= maxLatitude && longit >= minLongitude && longit <= maxLongitude)
                {
                    nobis[i].Latitude = latid;
                    nobis[i].Longitude = longit;
                    listaNodova.Add(nobis[i]);
                    collectionOfNodesID.Add(nobis[i].Id, nobis[i]);
                }

            }
            #endregion

            #region switches
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchobj = new SwitchEntity();
                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                switchis.Add(switchobj);
                switches.Add(switchobj.Id, switchobj);

            }

            for (int i = 0; i < switchis.Count(); i++)
            {
                listaSwitchevaProba.Add(switchis[i]);
                var item = switchis[i];
                ToLatLon(item.X, item.Y, 34, out latid, out longit);
                if (latid >= minLatitude && latid <= maxLatitude && longit >= minLongitude && longit <= maxLongitude)
                {
                    switchis[i].Latitude = latid;
                    switchis[i].Longitude = longit;
                    listaSwitcheva.Add(switchis[i]);
                    collectionOfNodesID.Add(switchis[i].Id, switchis[i]);
                }


            }
            #endregion

            #region lines

            var lines = xdoc.Descendants("LineEntity")
                     .Select(line => new LineEntity
                     {
                         Id = (long)line.Element("Id"),
                         Name = (string)line.Element("Name"),
                         ConductorMaterial = (string)line.Element("ConductorMaterial"),
                         IsUnderground = (bool)line.Element("IsUnderground"),
                         R = (float)line.Element("R"),
                         FirstEnd = (long)line.Element("FirstEnd"),
                         SecondEnd = (long)line.Element("SecondEnd"),
                         LineType = (string)line.Element("LineType"),
                         ThermalConstantHeat = (long)line.Element("ThermalConstantHeat"),
                         Vertices = line.Element("Vertices").Descendants("Point").Select(p => new Model.Point
                         {
                             X = (double)p.Element("X"),
                             Y = (double)p.Element("Y"),
                         }).ToList()
                     }).ToList();

            for (int i = 0; i < lines.Count(); i++)
            {
                if (collectionOfNodesID.ContainsKey(lines[i].SecondEnd) && collectionOfNodesID.ContainsKey(lines[i].FirstEnd))
                {
                    var line = lines[i];
                    foreach (var point in line.Vertices)
                    {

                        ToLatLon(point.X, point.Y, 34, out latid, out longit);
                        point.Latitude = latid;
                        point.Longitude = longit;

                    }

                    if (!materijali.Contains(line.ConductorMaterial))
                        materijali.Add(line.ConductorMaterial);

                    listaVodova.Add(line);
                    allLines.Add(line);
                }
            }

            otpornosti.Add(0, 0);
            otpornosti.Add(1, 0);
            otpornosti.Add(2, 0);

            foreach (var line in lines)
            {
                if (line.R < 1)
                {   //crvena
                    otpornosti[0]++;

                }
                else if (line.R >= 1 && line.R < 2)
                {   //narandzasta
                    otpornosti[1]++;

                }
                else if (line.R >= 2)
                {   //zuta
                    otpornosti[2]++;
                }
            }

            #endregion

        }


        private void crtajElemente() {
            foreach (var node in listaNodova)
            {

                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(node.Longitude, node.Latitude, node.Id);
                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.AliceBlue));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);

                RotateTransform3D rotateTransform3D = new RotateTransform3D();
                //rotateTransform3D.Rotation = myAngleRotation;

                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);

                geometryModel3D.Transform = myTransform3DGroup;

                geometryAndEntyty.Add(geometryModel3D, node);
                //NodeGeometryModel.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);

                allEntities.Add(geometryModel3D, node.Id);
            }
            foreach (var sub in listaSubstationa)
            {
                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(sub.Longitude, sub.Latitude, sub.Id);
                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.DarkOrange));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);

                RotateTransform3D rotateTransform3D = new RotateTransform3D();
                //rotateTransform3D.Rotation = myAngleRotation;

                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);

                geometryModel3D.Transform = myTransform3DGroup;

                geometryAndEntyty.Add(geometryModel3D, sub);
                //SubstationsGeometryModel.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);

                allEntities.Add(geometryModel3D, sub.Id);
            }
            foreach (var sw in listaSwitcheva)
            {
                MeshGeometry3D meshGeometry3D = createCubeMeshGeometry(sw.Longitude, sw.Latitude, sw.Id);
                DiffuseMaterial diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Violet));

                GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, diffuseMaterial);

                RotateTransform3D rotateTransform3D = new RotateTransform3D();
                //rotateTransform3D.Rotation = myAngleRotation;

                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);

                geometryModel3D.Transform = myTransform3DGroup;

                geometryAndEntyty.Add(geometryModel3D, sw);
                //SwitchGeometryModel.Add(geometryModel3D);
                model3dGroup.Children.Add(geometryModel3D);

                allEntities.Add(geometryModel3D, sw.Id);
            }
        }


        public static MeshGeometry3D createCubeMeshGeometry(double longitude, double latitude, long entityID)
        {
            var point = CreatePoint(longitude, latitude);
            int numEntity = 0;

            if (numberOfEntityOnPoint.ContainsKey(point))
            {
                numberOfEntityOnPoint[point]++;
                numEntity = numberOfEntityOnPoint[point];
            }
            else
            {
                numberOfEntityOnPoint[point] = 1;
                numEntity = 1;
            }

            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

            List<Point3D> pointovi = new List<Point3D>();
            // kocka ima 8 tacaka
            pointovi.Add(new Point3D(point.X - 4, point.Y - 4, numEntity * 10 - 10));
            pointovi.Add(new Point3D(point.X + 4, point.Y - 4, numEntity * 10 - 10));
            pointovi.Add(new Point3D(point.X + 4, point.Y - 4, numEntity * 10));
            pointovi.Add(new Point3D(point.X - 4, point.Y - 4, numEntity * 10));
            //dole
            pointovi.Add(new Point3D(point.X - 4, point.Y + 4, numEntity * 10));
            pointovi.Add(new Point3D(point.X + 4, point.Y + 4, numEntity * 10));
            pointovi.Add(new Point3D(point.X + 4, point.Y + 4, numEntity * 10 - 10));
            pointovi.Add(new Point3D(point.X - 4, point.Y + 4, numEntity * 10 - 10));

            meshGeometry3D.Positions = new Point3DCollection(pointovi);
            // stranice
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

            // dodaj donju stanicu 

            return meshGeometry3D;
        }



        public static System.Windows.Point CreatePoint(double longitude, double latitude)
        {
            // vel canvasa 2200x2200
            double vrednostJednogLongitude = (maxLongitude - minLongitude) / 1175; // width slike mape
            double vrednostJednogLatitude = (maxLatitude - minLatitude) / 775; // height slike mape 

            // koliko stane u rastojanje izmedju ttrenutne i minimalne 
            double x = Math.Round((longitude - minLongitude) / vrednostJednogLongitude) - 587.5; // pozicija na xamlu
            double y = Math.Round((latitude - minLatitude) / vrednostJednogLatitude) - 387.5;

            // zaokruzi na prvi broj deljiv sa 10 
            x = x - x % 8; // rastojanje izmedju dva susedna x
            y = y - y % 8; // rastojanje izmedju dva susedna y 

            System.Windows.Point point = new System.Windows.Point();
            point.X = x;
            point.Y = y;

            return point;
        }



        public static GeometryModel3D createLineGeometryModel3D(System.Windows.Point point1, System.Windows.Point point2, DiffuseMaterial boja)
        {
            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(point1.X - 1, point1.Y - 1, 0)); //0
            points.Add(new Point3D(point1.X + 1, point1.Y + 1, 0)); //1
            points.Add(new Point3D(point1.X - 1, point1.Y - 1, 10)); //2
            points.Add(new Point3D(point1.X + 1, point1.Y + 1, 10)); //3

            points.Add(new Point3D(point2.X - 1, point2.Y - 1, 0)); //4
            points.Add(new Point3D(point2.X + 1, point2.Y + 1, 0)); //5
            points.Add(new Point3D(point2.X - 1, point2.Y - 1, 10)); //6
            points.Add(new Point3D(point2.X + 1, point2.Y + 1, 10)); //7


            meshGeometry3D.Positions = new Point3DCollection(points);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(0); // back

            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(4); // left

            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(5); //front

            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(1); // right

            meshGeometry3D.TriangleIndices.Add(7);
            meshGeometry3D.TriangleIndices.Add(3);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(2);
            meshGeometry3D.TriangleIndices.Add(6);
            meshGeometry3D.TriangleIndices.Add(7); // top

            meshGeometry3D.TriangleIndices.Add(1);
            meshGeometry3D.TriangleIndices.Add(5);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(4);
            meshGeometry3D.TriangleIndices.Add(0);
            meshGeometry3D.TriangleIndices.Add(1); // bottom

            GeometryModel3D geometryModel3D = new GeometryModel3D(meshGeometry3D, boja);

            RotateTransform3D rotateTransform3D = new RotateTransform3D();
            //rotateTransform3D.Rotation = myAngleRotation;

            Transform3DGroup myTransform3DGroup = new Transform3DGroup();
            myTransform3DGroup.Children.Add(rotateTransform3D);

            geometryModel3D.Transform = myTransform3DGroup;

            return geometryModel3D;
        }

        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewPort.CaptureMouse();
            start = e.GetPosition(ViewPort);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetZ;

            //hit testing
            System.Windows.Point mouseposition = e.GetPosition(ViewPort);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D     
            hitgeo = null;
            VisualTreeHelper.HitTest(ViewPort, null, HTResult, pointparams);

        }

        

        private void ViewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewPort.ReleaseMouseCapture();
        }
        private int zoom = 20;
        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var p = PointFromScreen(e.GetPosition(window));
            skaliranje.CenterX = p.X;
            skaliranje.CenterY = p.Y;
            skaliranje.CenterZ = 0;

            if ((e.Delta > 0 && zoom < 60) || (e.Delta <= 0 && zoom > -60))
            {
                zoom += e.Delta > 0 ? 1 : -1;
                skaliranje.ScaleX += e.Delta > 0 ? 0.1 : -0.1;
                skaliranje.ScaleY += e.Delta > 0 ? 0.1 : -0.1;
                skaliranje.ScaleZ += e.Delta > 0 ? 0.1 : -0.1;
            }
        }



        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)    
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                DiffuseMaterial darkSide = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Colors.Red));
                bool gasit = false;

                #region Kada se klikne entitet - ToolTip

                foreach (KeyValuePair<GeometryModel3D, PowerEntity> model in geometryAndEntyty)
                {
                    if (model.Key == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;

                        if (model.Value is NodeEntity)
                        {
                            toolTip.Content = "\tNode entity:\n\nID: " + model.Value.Id.ToString() + "\nName: " + model.Value.Name + "\nType: " + model.Value.GetType().Name;

                        }
                        else if (model.Value is SubstationEntity)
                        {
                            toolTip.Content = "\tSubstation entity:\n\nID: " + model.Value.Id.ToString() + "\nName: " + model.Value.Name + "\nType: " + model.Value.GetType().Name;

                        }
                        else if (model.Value is SwitchEntity)
                        {
                            toolTip.Content = "\tSwitch entity:\n\nID: " + model.Value.Id.ToString() + "\nName: " + model.Value.Name + "\nType: " + model.Value.GetType().Name;

                        }
                        toolTip.Height = 100;
                        toolTip.IsOpen = true;

                        ToolTipService.SetPlacement(ViewPort, PlacementMode.Mouse);
                    }
                }
                #endregion

                #region Kada se klikne na vod
                //firstEnd i SecondEnd su ID-jevi entiteta

                if (geometryAndVod.ContainsKey((GeometryModel3D)rayResult.ModelHit))
                {
                    hitgeo = (GeometryModel3D)rayResult.ModelHit;
                    gasit = true;

                    var prviEntitet = geometryAndVod[(GeometryModel3D)rayResult.ModelHit].FirstEnd;
                    var drugiEntitet = geometryAndVod[(GeometryModel3D)rayResult.ModelHit].SecondEnd;
                    toolTip.Content = "\tLine entity:\n\nID: " + geometryAndVod[(GeometryModel3D)rayResult.ModelHit].Id.ToString() + "\nName: " + geometryAndVod[(GeometryModel3D)rayResult.ModelHit].Name + "\nType: " + geometryAndVod[(GeometryModel3D)rayResult.ModelHit].GetType().Name+"\nR:" + geometryAndVod[(GeometryModel3D)rayResult.ModelHit].R;

                    toolTip.Height = 100;
                    toolTip.IsOpen = true;

                    ToolTipService.SetPlacement(ViewPort, PlacementMode.Mouse);

                    foreach (KeyValuePair<GeometryModel3D, PowerEntity> ent1 in geometryAndEntyty)
                    {
                        var prethodna = ((DiffuseMaterial)((GeometryModel3D)ent1.Key).Material).Brush;

                        if (ent1.Value.Id == prviEntitet || ent1.Value.Id == drugiEntitet)
                        {
                            ((DiffuseMaterial)((GeometryModel3D)ent1.Key).Material).Brush = Brushes.Red;

                        }
                    }
                }


                #endregion

                if (!gasit)
                {
                    hitgeo = null;
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var line in listaVodova)
            {
                foreach (var s in listaSwitcheva)
                {
                    if (s.Id.Equals(line.FirstEnd))
                    {
                        if (s.Status == "Open")
                        {
                            model3dGroup.Children.Remove(allEntities.FirstOrDefault(item=>item.Value==s.Id).Key);

                        }
                    }
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, long> entity in allEntities)
            {
                if (model3dGroup.Children.Contains(entity.Key) == false)
                {
                    model3dGroup.Children.Add(entity.Key);
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            foreach (SwitchEntity s in listaSwitcheva)
            {
                if (s.Status=="Open")
                {
                    
                    allEntities.FirstOrDefault(x => x.Value == s.Id).Key.Material = new DiffuseMaterial(System.Windows.Media.Brushes.Green);
                }
                else
                {
                    allEntities.FirstOrDefault(x => x.Value == s.Id).Key.Material = new DiffuseMaterial(System.Windows.Media.Brushes.Red);
                }
            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            foreach (SwitchEntity s in listaSwitcheva)
            {
                allEntities.FirstOrDefault(x => x.Value == s.Id).Key.Material = new DiffuseMaterial(System.Windows.Media.Brushes.Violet);
            }
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            foreach (var kvp in geometryAndVod)
            {
                GeometryModel3D geometryModel = kvp.Key;
                LineEntity lineEntity = kvp.Value;

                if (lineEntity.R < 1)
                {
                    DiffuseMaterial material = (DiffuseMaterial)geometryModel.Material;
                    material.Brush = System.Windows.Media.Brushes.Red;
                }
                else if (lineEntity.R >= 1 && lineEntity.R < 2)
                {
                    DiffuseMaterial material = (DiffuseMaterial)geometryModel.Material;
                    material.Brush = System.Windows.Media.Brushes.Orange;
                }
                else if (lineEntity.R > 2)
                {
                    DiffuseMaterial material = (DiffuseMaterial)geometryModel.Material;
                    material.Brush = System.Windows.Media.Brushes.Yellow;
                }
            }
        }


        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {
            foreach (LineEntity line in listaVodova)
            {
                foreach (KeyValuePair<GeometryModel3D, long> kvp in allVodes)
                {
                    GeometryModel3D geometryModel = kvp.Key;
                    long id = kvp.Value;

                    if (id == line.Id)
                    {
                        DiffuseMaterial material = (DiffuseMaterial)geometryModel.Material;
                        if (line.ConductorMaterial == "Steel")
                            material.Brush = System.Windows.Media.Brushes.DarkGreen;
                        else if (line.ConductorMaterial == "Acsr")
                            material.Brush = System.Windows.Media.Brushes.DarkRed;
                        else if (line.ConductorMaterial == "Copper")
                            material.Brush = System.Windows.Media.Brushes.Black;
                    }
                }
            }

        }

        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {


            var end = e.GetPosition(this);
            var offsetX = start.X - end.X;
            var offsetY = start.Y - end.Y;
            var translateX = (offsetX * 100) / Width;
            var translateY = -(offsetY * 100) / Height;

            if (ViewPort.IsMouseCaptured)
            {
                
                    translacija.OffsetX = diffOffset.X + (translateX / (100 * skaliranje.ScaleX)) * 1000;
                    translacija.OffsetY = diffOffset.Y + (translateY / (100 * skaliranje.ScaleY)) * 1000;

               
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var angleX = (rotateX.Angle + translateY) % 360;
                rotateY.Angle = (rotateY.Angle + -translateX) % 360;
                if (-65 < angleX && angleX < 65)
                {
                    rotateX.Angle = angleX;
                }
                start = end;
            }
        }

       

      

        

        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> vod in geometryAndVod)
            {
                if (vod.Value.R<1)
                {
                    model3dGroup.Children.Remove(vod.Key);
                    Console.WriteLine("removed");
                }
            }
        }

        private void CheckBox_Checked_4(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> vod in geometryAndVod)
            {
                if (vod.Value.R <= 2 && vod.Value.R >= 1)
                {
                    model3dGroup.Children.Remove(vod.Key);
                    Console.WriteLine("removed");
                }
            }
            
        }

        private void CheckBox_Checked_5(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> vod in geometryAndVod)
            {
                if (vod.Value.R > 2)
                {
                    model3dGroup.Children.Remove(vod.Key);
                    Console.WriteLine("removed");
                }
            }
        }

        private void CheckBox_Unchecked_3(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> entity in geometryAndVod)
            {
                if (model3dGroup.Children.Contains(entity.Key) == false)
                {
                    model3dGroup.Children.Add(entity.Key);
                }
            }
        }

        private void CheckBox_Unchecked_4(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> entity in geometryAndVod)
            {
                if (model3dGroup.Children.Contains(entity.Key) == false)
                {
                    model3dGroup.Children.Add(entity.Key);
                }
            }
        }

        private void CheckBox_Unchecked_5(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<GeometryModel3D, LineEntity> entity in geometryAndVod)
            {
                if (model3dGroup.Children.Contains(entity.Key) == false)
                {
                    model3dGroup.Children.Add(entity.Key);
                }
            }
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
