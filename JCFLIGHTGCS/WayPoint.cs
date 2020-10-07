using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.IO.Ports;
using System.Threading;
using System.Globalization;
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic;

namespace JCFLIGHTGCS
{
    public partial class WayPoint : Form
    {
        //SERIAL RX TX
        SerialPort SerialPort = new SerialPort();
        string SerialComPort, RxString;
        string[] ArduinoData = null;
        string[] SerialPorts = SerialPort.GetPortNames();

        int CountWP;
        double WPLat;
        double WPLon;

        double Dist1;
        double Dist2;
        double Dist3;
        double Dist4;
        double Dist5;
        double Dist6;
        double Dist7;
        double Dist8;
        double Dist9;

        double WPLatVect1;
        double WPLonVect1;

        double WPLatVect2;
        double WPLonVect2;

        double WPLatVect3;
        double WPLonVect3;

        double WPLatVect4;
        double WPLonVect4;

        double WPLatVect5;
        double WPLonVect5;

        double WPLatVect6;
        double WPLonVect6;

        double WPLatVect7;
        double WPLonVect7;

        double WPLatVect8;
        double WPLonVect8;

        double WPLatVect9;
        double WPLonVect9;

        double WPLatVect10;
        double WPLonVect10;

        int DebugMap;
        Boolean InvalidWP = false;
        Boolean PrintArea = false;

        double InitialLat;
        double InitialLong;

        Int32 Begin = 0;

        string GPSLAT = "0";
        string GPSLONG = "0";

        Boolean PushLocation = false;

        Int32 FrameType = 0;

        double NumSat;
        double VBatt;
        double HDOP = 99.99;
        double Altitude;

        int Heading = 0;

        byte GmapFrameMode = 0;

        GMapOverlay MarkersOverlay = new GMapOverlay("Makers");
        GMapOverlay GmapPolygons = new GMapOverlay("Poligonos");

        private List<PointLatLng> WPCoordinates;
        private List<PointLatLng> DistanceValid;

        static GMapOverlay GmapRoutes;
        static GMapRoute GMapTack;
        List<PointLatLng> LatLngPoints = new List<PointLatLng>();
        PointLatLng GPS_Position;
        GMapOverlay GmapPositions;

        const int KEY_WP1 = 1;
        const int KEY_WP2 = 2;
        const int KEY_WP3 = 3;
        const int KEY_WP4 = 4;
        const int KEY_WP5 = 5;
        const int KEY_WP6 = 6;
        const int KEY_WP7 = 7;
        const int KEY_WP8 = 8;
        const int KEY_WP9 = 9;
        const int KEY_WP10 = 10;
        const int KEY_WPReset = 11;
        const int KEY_WPSave = 12;

        Int32 WayPoint1Latitude;
        Int32 WayPoint1Longitude;

        Int32 WayPoint2Latitude;
        Int32 WayPoint2Longitude;

        Int32 WayPoint3Latitude;
        Int32 WayPoint3Longitude;

        Int32 WayPoint4Latitude;
        Int32 WayPoint4Longitude;

        Int32 WayPoint5Latitude;
        Int32 WayPoint5Longitude;

        Int32 WayPoint6Latitude;
        Int32 WayPoint6Longitude;

        Int32 WayPoint7Latitude;
        Int32 WayPoint7Longitude;

        Int32 WayPoint8Latitude;
        Int32 WayPoint8Longitude;

        Int32 WayPoint9Latitude;
        Int32 WayPoint9Longitude;

        Int32 WayPoint10Latitude;
        Int32 WayPoint10Longitude;

        Boolean ArmDisarm = false;

        double PrevLatitude;
        double PrevLongitude;

        byte GPSHoldTimed1 = 1;
        byte GPSHoldTimed2 = 1;
        byte GPSHoldTimed3 = 1;
        byte GPSHoldTimed4 = 1;
        byte GPSHoldTimed5 = 1;
        byte GPSHoldTimed6 = 1;
        byte GPSHoldTimed7 = 1;
        byte GPSHoldTimed8 = 1;
        byte GPSHoldTimed9 = 1;
        byte GPSHoldTimed10 = 1;

        public WayPoint()
        {
            InitializeComponent();

            MyGmap.PolygonsEnabled = true;

            //SERIAL
            SerialPort.DataBits = 8;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.Handshake = Handshake.None;
            SerialPort.DtrEnable = false;
            SerialPort.ReadBufferSize = 4096;
            foreach (string PortsName in SerialPorts) comboBox13.Items.Add(PortsName);
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived_1);

            pictureBox1.Image = Properties.Resources.Desconectado;

            MyGmap.ShowCenter = false;
            MyGmap.Manager.Mode = AccessMode.ServerAndCache;
            //MyGmap.MapProvider = GMapProviders.BingSatelliteMap;
            //MyGmap.MapProvider = GMapProviders.BingHybridMap;
            MyGmap.MapProvider = GMapProviders.GoogleSatelliteMap;
            MyGmap.Zoom = 2;

            GmapPositions = new GMapOverlay("GmapPositions");
            MyGmap.Overlays.Add(GmapPositions);

            GmapPositions.Markers.Clear();

            GmapRoutes = new GMapOverlay("GMapRoutes");
            MyGmap.Overlays.Add(GmapRoutes);

            Pen penRoute = new Pen(Color.Purple, 2);

            GMapTack = new GMapRoute(LatLngPoints, "GMapTrack");
            GMapTack.Stroke = penRoute;
            GmapRoutes.Routes.Add(GMapTack);

            trackBar1.Value = 2;
            trackBar1.Minimum = 2;
            trackBar1.Maximum = 20;

            label41.Parent = MyGmap;
            label41.BackColor = Color.Blue;
            label41.ForeColor = Color.White;

            label46.Parent = MyGmap;
            label46.BackColor = Color.Blue;
            label46.ForeColor = Color.White;

            label47.Parent = MyGmap;
            label47.BackColor = Color.Blue;
            label47.ForeColor = Color.White;

            label48.Parent = MyGmap;
            label48.BackColor = Color.Blue;
            label48.ForeColor = Color.White;

            comboBox1.MouseWheel += new MouseEventHandler(comboBox1_MouseWheel);
            comboBox2.MouseWheel += new MouseEventHandler(comboBox2_MouseWheel);
            comboBox3.MouseWheel += new MouseEventHandler(comboBox3_MouseWheel);
            comboBox4.MouseWheel += new MouseEventHandler(comboBox4_MouseWheel);
            comboBox5.MouseWheel += new MouseEventHandler(comboBox5_MouseWheel);
            comboBox6.MouseWheel += new MouseEventHandler(comboBox6_MouseWheel);
            comboBox7.MouseWheel += new MouseEventHandler(comboBox7_MouseWheel);
            comboBox8.MouseWheel += new MouseEventHandler(comboBox8_MouseWheel);
            comboBox9.MouseWheel += new MouseEventHandler(comboBox9_MouseWheel);
            comboBox10.MouseWheel += new MouseEventHandler(comboBox10_MouseWheel);
            comboBox11.MouseWheel += new MouseEventHandler(comboBox11_MouseWheel);
            comboBox12.MouseWheel += new MouseEventHandler(comboBox12_MouseWheel);
            comboBox13.MouseWheel += new MouseEventHandler(comboBox13_MouseWheel);
            comboBox14.MouseWheel += new MouseEventHandler(comboBox14_MouseWheel);
            comboBox15.MouseWheel += new MouseEventHandler(comboBox15_MouseWheel);
            comboBox16.MouseWheel += new MouseEventHandler(comboBox16_MouseWheel);
            comboBox17.MouseWheel += new MouseEventHandler(comboBox17_MouseWheel);
            comboBox18.MouseWheel += new MouseEventHandler(comboBox18_MouseWheel);
            comboBox19.MouseWheel += new MouseEventHandler(comboBox19_MouseWheel);
            comboBox20.MouseWheel += new MouseEventHandler(comboBox20_MouseWheel);
            comboBox21.MouseWheel += new MouseEventHandler(comboBox21_MouseWheel);
            trackBar1.MouseWheel += new MouseEventHandler(trackBar1_MouseWheel);
        }

        Boolean ThisPointEquals = false;

        private void MyGmap_MouseMove(object sender, MouseEventArgs e)
        {
            //PREVINE CRIAR 2 WP NA MESMA COORDENADA
            PointLatLng MousePoint = MyGmap.FromLocalToLatLng(e.X, e.Y);

            ThisPointEquals = true;

            if (MousePoint.Lat == WPLatVect1) return;
            if (MousePoint.Lng == WPLonVect1) return;

            if (MousePoint.Lat == WPLatVect2) return;
            if (MousePoint.Lng == WPLonVect2) return;

            if (MousePoint.Lat == WPLatVect3) return;
            if (MousePoint.Lng == WPLonVect3) return;

            if (MousePoint.Lat == WPLatVect4) return;
            if (MousePoint.Lng == WPLonVect4) return;

            if (MousePoint.Lat == WPLatVect5) return;
            if (MousePoint.Lng == WPLonVect5) return;

            if (MousePoint.Lat == WPLatVect6) return;
            if (MousePoint.Lng == WPLonVect6) return;

            if (MousePoint.Lat == WPLatVect7) return;
            if (MousePoint.Lng == WPLonVect7) return;

            if (MousePoint.Lat == WPLatVect8) return;
            if (MousePoint.Lng == WPLonVect8) return;

            if (MousePoint.Lat == WPLatVect9) return;
            if (MousePoint.Lng == WPLonVect9) return;

            if (MousePoint.Lat == WPLatVect10) return;
            if (MousePoint.Lng == WPLonVect10) return;

            ThisPointEquals = false;
        }

        private void gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (ThisPointEquals) return;
            if (e.Button == MouseButtons.Left)
            {
                if (!InvalidWP)
                {
                    CountWP++;
                    var WayPoint = MyGmap.FromLocalToLatLng(e.X, e.Y);
                    WPLat = WayPoint.Lat;
                    WPLon = WayPoint.Lng;
                    var GMarker = new GMarkerGoogle(WayPoint, GMarkerGoogleType.orange);
                    GMarker.ToolTip = new GMapToolTip(GMarker);
                    GMarker.ToolTipMode = MarkerTooltipMode.Always;
                    GMarker.ToolTipText = $"WP { CountWP}";
                    MarkersOverlay.Markers.Add(GMarker);
                    MyGmap.Overlays.Add(MarkersOverlay);
                    MyGmap.UpdateMarkerLocalPosition(GMarker);
                    GmapPolygons.Polygons.Clear();
                    PrintArea = true;
                }

                if (CountWP == 1)
                {
                    label19.Text = Convert.ToString(WPLat);
                    label18.Text = Convert.ToString(WPLon);
                    WPLatVect1 = WPLat;
                    WPLonVect1 = WPLon;
                }
                if (CountWP == 2)
                {
                    label20.Text = Convert.ToString(WPLat);
                    label16.Text = Convert.ToString(WPLon);
                    WPLatVect2 = WPLat;
                    WPLonVect2 = WPLon;
                }
                if (CountWP == 3)
                {
                    label21.Text = Convert.ToString(WPLat);
                    label22.Text = Convert.ToString(WPLon);
                    WPLatVect3 = WPLat;
                    WPLonVect3 = WPLon;
                }
                if (CountWP == 4)
                {
                    label37.Text = Convert.ToString(WPLat);
                    label35.Text = Convert.ToString(WPLon);
                    WPLatVect4 = WPLat;
                    WPLonVect4 = WPLon;
                }
                if (CountWP == 5)
                {
                    label30.Text = Convert.ToString(WPLat);
                    label26.Text = Convert.ToString(WPLon);
                    WPLatVect5 = WPLat;
                    WPLonVect5 = WPLon;
                }
                if (CountWP == 6)
                {
                    label28.Text = Convert.ToString(WPLat);
                    label24.Text = Convert.ToString(WPLon);
                    WPLatVect6 = WPLat;
                    WPLonVect6 = WPLon;
                }
                if (CountWP == 7)
                {
                    label61.Text = Convert.ToString(WPLat);
                    label57.Text = Convert.ToString(WPLon);
                    WPLatVect7 = WPLat;
                    WPLonVect7 = WPLon;
                }
                if (CountWP == 8)
                {
                    label70.Text = Convert.ToString(WPLat);
                    label63.Text = Convert.ToString(WPLon);
                    WPLatVect8 = WPLat;
                    WPLonVect8 = WPLon;
                }
                if (CountWP == 9)
                {
                    label68.Text = Convert.ToString(WPLat);
                    label59.Text = Convert.ToString(WPLon);
                    WPLatVect9 = WPLat;
                    WPLonVect9 = WPLon;
                }
                if (CountWP == 10)
                {
                    label66.Text = Convert.ToString(WPLat);
                    label55.Text = Convert.ToString(WPLon);
                    WPLatVect10 = WPLat;
                    WPLonVect10 = WPLon;
                }
                if (CountWP > 9)
                {
                    MessageBox.Show("Número Maximo de WayPoints Atingido.");
                    InvalidWP = true;
                    CountWP = 10;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DebugMap < 300 && PushLocation)
            {
                DebugMap++;
                MyGmap.Position = new PointLatLng(Convert.ToDouble(InitialLat / 10000000.0), Convert.ToDouble(InitialLong / 10000000.0));
                MyGmap.Zoom = 18;
                trackBar1.Value = 18;
                timer1.Enabled = false;
                if (timer1.Enabled == false) DebugMap = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;
            SerialPort.Close();
            serialPort1.PortName = SerialComPort;
            serialPort1.BaudRate = 115200;
            serialPort1.Open();
            for (int i = 0; i < 300; i++)
            {
                SendWayPoints(serialPort1);
                Thread.Sleep(1);
            }
            serialPort1.Close();
            SerialPort.PortName = SerialComPort;
            SerialPort.BaudRate = 115200;
            SerialPort.DataBits = 8;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.Open();
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean Ignore = false;
            SerialComPort = Convert.ToString(comboBox13.SelectedItem);
            try
            {
                SerialPort.Close();
                serialPort1.PortName = SerialComPort;
                serialPort1.BaudRate = 115200;
                serialPort1.Open();
                for (Int32 i = 0; i < 100; i++)
                {
                    if (Ignore == false)
                    {
                        serialPort1.WriteLine("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
                        Thread.Sleep(1);
                    }
                    if (i == 99)
                    {
                        Ignore = true;
                        serialPort1.WriteLine("RUN LOOP WAYPOINT");
                    }
                }
                serialPort1.Close();
                SerialPort.PortName = SerialComPort;
                SerialPort.BaudRate = 115200;
                SerialPort.DataBits = 8;
                SerialPort.Parity = Parity.None;
                SerialPort.StopBits = StopBits.One;
                SerialPort.Open();
                if (SerialPort.IsOpen == true)
                {
                    comboBox13.Enabled = false;
                    pictureBox1.Image = Properties.Resources.Conectado;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("A conexão falhou!Acho que a Porta Serial está aberta em outro programa,verifique se não está aberta na tela principal do GCS.");
            }
        }

        private void comboBox13_Click(object sender, EventArgs e)
        {
            comboBox13.Items.Clear();
            SerialPorts = SerialPort.GetPortNames();
            foreach (string PortsName in SerialPorts) comboBox13.Items.Add(PortsName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox13.Enabled = true;
            comboBox13.Text = "Selecione";
            SerialPort.Close();
            if (SerialPort.IsOpen == false)
            {
                pictureBox1.Image = Properties.Resources.Desconectado;
            }
        }

        private void serialPort1_DataReceived_1(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (SerialPort.IsOpen == false) return;
                RxString = SerialPort.ReadLine();
                ArduinoData = RxString.Split(',');
                if (RxString == "") return;
                if (RxString == "Debug Serial Monitor Para JCFlightGCS\r") return;
                if (RxString == "POR FAVOR,EXECUTE A TELEMETRIA NO PROGRAMA JCFLIGHTGCS\r") return;
                if (ArduinoData[0] == "\r") return;
                if (ArduinoData[0] == null) return;
                if (ArduinoData[0] == "0") return;
                Begin = Convert.ToInt32(ArduinoData[0]);
                if (Begin == 10)
                {
                    NumSat = Convert.ToDouble(ArduinoData[1]);
                    GPSLAT = Convert.ToString(ArduinoData[2]);
                    GPSLONG = Convert.ToString(ArduinoData[3]);
                    InitialLat = double.Parse(GPSLAT);
                    InitialLong = double.Parse(GPSLONG);
                    if (NumSat >= 5 && InitialLat != 0 && InitialLong != 0) PushLocation = true;
                    VBatt = Convert.ToDouble(ArduinoData[4]) / 100;
                    HDOP = Convert.ToDouble(ArduinoData[5]) / 100;
                    Altitude = Convert.ToDouble(ArduinoData[6]) / 100;
                    Heading = Convert.ToInt32(ArduinoData[7]);
                    FrameType = Convert.ToInt32(ArduinoData[8]);
                    if (FrameType == 1)
                    {
                        GmapFrameMode = 1;
                        ArmDisarm = false;
                    }

                    if (FrameType == 2)
                    {
                        GmapFrameMode = 1;
                        ArmDisarm = true;
                    }

                    if (FrameType == 3)
                    {
                        GmapFrameMode = 2;
                        ArmDisarm = false;
                    }

                    if (FrameType == 4)
                    {
                        GmapFrameMode = 2;
                        ArmDisarm = true;
                    }

                    if (FrameType == 5)
                    {
                        GmapFrameMode = 3;
                        ArmDisarm = false;
                    }

                    if (FrameType == 6)
                    {
                        GmapFrameMode = 3;
                        ArmDisarm = true;
                    }

                    if (FrameType == 7)
                    {
                        GmapFrameMode = 4;
                        ArmDisarm = false;
                    }

                    if (FrameType == 8)
                    {
                        GmapFrameMode = 4;
                        ArmDisarm = true;
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            WPCoordinates = new List<PointLatLng>();

            label41.Text = Convert.ToString("Número de Satélites:" + NumSat.ToString(new CultureInfo("en-US")));
            label47.Text = Convert.ToString("HDOP:" + HDOP.ToString(new CultureInfo("en-US")));
            label48.Text = Convert.ToString("Altitude:" + Altitude.ToString(new CultureInfo("en-US")) + " M");

            if (VBatt > 6)
            {
                label46.Text = Convert.ToString("Tensão da Bateria:" + Convert.ToString(VBatt) + "V");
            }
            else
            {
                label46.Text = Convert.ToString("Tensão da Bateria:00.00V");
            }

            if (CountWP == 1)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
            }

            if (CountWP == 2)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                Dist1 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[0], WPCoordinates[1]) * 1000;
                double Dist1Float = Convert.ToDouble(Convert.ToInt32(Dist1)) / 1000;
                if (Dist1 >= 1000) label42.Text = "Distância entre P1 - P2: " + Dist1Float.ToString(new CultureInfo("en-US")) + "KM";
                else label42.Text = "Distância entre P1 - P2: " + Convert.ToInt32(Dist1) + "M";
            }

            if (CountWP == 3)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                Dist2 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[1], WPCoordinates[2]) * 1000;
                double Dist2Float = Convert.ToDouble(Convert.ToInt32(Dist2)) / 1000;
                if (Dist2 >= 1000) label43.Text = "Distância entre P2 - P3: " + Dist2Float.ToString(new CultureInfo("en-US")) + "KM";
                else label43.Text = "Distância entre P2 - P3: " + Convert.ToInt32(Dist2) + "M";
            }

            if (CountWP == 4)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                Dist3 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[2], WPCoordinates[3]) * 1000;
                double Dist3Float = Convert.ToDouble(Convert.ToInt32(Dist3)) / 1000;
                if (Dist3 >= 1000) label44.Text = "Distância entre P3 - P4: " + Dist3Float.ToString(new CultureInfo("en-US")) + "KM";
                else label44.Text = "Distância entre P3 - P4: " + Convert.ToInt32(Dist3) + "M";
            }

            if (CountWP == 5)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                Dist4 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[3], WPCoordinates[4]) * 1000;
                double Dist4Float = Convert.ToDouble(Convert.ToInt32(Dist4)) / 1000;
                if (Dist4 >= 1000) label45.Text = "Distância entre P4 - P5: " + Dist4Float.ToString(new CultureInfo("en-US")) + "KM";
                else label45.Text = "Distância entre P4 - P5: " + Convert.ToInt32(Dist4) + "M";
            }

            if (CountWP == 6)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect6), Convert.ToDouble(WPLonVect6)));
                Dist5 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[4], WPCoordinates[5]) * 1000;
                double Dist5Float = Convert.ToDouble(Convert.ToInt32(Dist5)) / 1000;
                if (Dist5 >= 1000) label40.Text = "Distância entre P5 - P6: " + Dist5Float.ToString(new CultureInfo("en-US")) + "KM";
                else label40.Text = "Distância entre P5 - P6: " + Convert.ToInt32(Dist5) + "M";
            }

            if (CountWP == 7)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect6), Convert.ToDouble(WPLonVect6)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect7), Convert.ToDouble(WPLonVect7)));
                Dist6 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[5], WPCoordinates[6]) * 1000;
                double Dist6Float = Convert.ToDouble(Convert.ToInt32(Dist6)) / 1000;
                if (Dist6 >= 1000) label78.Text = "Distância entre P6 - P7: " + Dist6Float.ToString(new CultureInfo("en-US")) + "KM";
                else label78.Text = "Distância entre P6 - P7: " + Convert.ToInt32(Dist6) + "M";
            }

            if (CountWP == 8)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect6), Convert.ToDouble(WPLonVect6)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect7), Convert.ToDouble(WPLonVect7)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect8), Convert.ToDouble(WPLonVect8)));
                Dist7 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[6], WPCoordinates[7]) * 1000;
                double Dist7Float = Convert.ToDouble(Convert.ToInt32(Dist7)) / 1000;
                if (Dist7 >= 1000) label77.Text = "Distância entre P7 - P8: " + Dist7Float.ToString(new CultureInfo("en-US")) + "KM";
                else label77.Text = "Distância entre P7 - P8: " + Convert.ToInt32(Dist7) + "M";
            }

            if (CountWP == 9)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect6), Convert.ToDouble(WPLonVect6)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect7), Convert.ToDouble(WPLonVect7)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect8), Convert.ToDouble(WPLonVect8)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect9), Convert.ToDouble(WPLonVect9)));
                Dist8 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[7], WPCoordinates[8]) * 1000;
                double Dist8Float = Convert.ToDouble(Convert.ToInt32(Dist8)) / 1000;
                if (Dist8 >= 1000) label76.Text = "Distância entre P8 - P9: " + Dist8Float.ToString(new CultureInfo("en-US")) + "KM";
                else label76.Text = "Distância entre P8 - P9: " + Convert.ToInt32(Dist8) + "M";
            }

            if (CountWP == 10)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect4), Convert.ToDouble(WPLonVect4)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect5), Convert.ToDouble(WPLonVect5)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect6), Convert.ToDouble(WPLonVect6)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect7), Convert.ToDouble(WPLonVect7)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect8), Convert.ToDouble(WPLonVect8)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect9), Convert.ToDouble(WPLonVect9)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect10), Convert.ToDouble(WPLonVect10)));
                Dist9 = MyGmap.MapProvider.Projection.GetDistance(WPCoordinates[8], WPCoordinates[9]) * 1000;
                double Dist9Float = Convert.ToDouble(Convert.ToInt32(Dist9)) / 1000;
                if (Dist9 >= 1000) label75.Text = "Distância entre P9 - P10: " + Dist9Float.ToString(new CultureInfo("en-US")) + "KM";
                else label75.Text = "Distância entre P9 - P10: " + Convert.ToInt32(Dist9) + "M";
            }

            if (PrintArea)
            {
                GMapRoute FirstPointTrace = new GMapRoute("FirstPointTrace");
                FirstPointTrace.Clear();
                GmapPolygons.Clear();
                FirstPointTrace.Stroke = new Pen(Color.Orange, 4);
                FirstPointTrace.Stroke.DashStyle = DashStyle.Dash;

                if (CountWP > 2)
                {
                    FirstPointTrace.Points.Add(WPCoordinates[0]);
                    FirstPointTrace.Points.Add(WPCoordinates[WPCoordinates.Count - 1]);
                }

                GMapRoute WPLineRoute = new GMapRoute("WPLineRoute");
                WPLineRoute.Stroke = new Pen(Color.Green, 4);
                WPLineRoute.Stroke.DashStyle = DashStyle.Custom;

                for (int a = 0; a < WPCoordinates.Count; a++) WPLineRoute.Points.Add(WPCoordinates[a]);

                GmapPolygons.Routes.Add(FirstPointTrace);
                GmapPolygons.Routes.Add(WPLineRoute);

                MyGmap.Overlays.Add(GmapPolygons);

                PrintArea = false;
            }
            if (SerialPort.IsOpen)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
            if (PushLocation) MyGmap.Zoom = trackBar1.Value;
        }

        private void WayPoint_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SerialPort.Close();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            DistanceValid = new List<PointLatLng>();
            GPS_Position.Lat = double.Parse(GPSLAT) / 10000000;
            GPS_Position.Lng = double.Parse(GPSLONG) / 10000000;
            if (NumSat >= 5 && GPS_Position.Lat != 0 && GPS_Position.Lng != 0 &&
                PrevLatitude != GPS_Position.Lat && PrevLongitude != GPS_Position.Lng)
            {

                DistanceValid.Add(new PointLatLng(Convert.ToDouble(PrevLatitude), Convert.ToDouble(PrevLongitude)));
                DistanceValid.Add(new PointLatLng(Convert.ToDouble(GPS_Position.Lat), Convert.ToDouble(GPS_Position.Lng)));
                double Distance = MyGmap.MapProvider.Projection.GetDistance(DistanceValid[0], DistanceValid[1]) * 1000;
                double Dist1Float = Convert.ToDouble(Convert.ToInt32(Distance));
                if (Dist1Float <= 1) return; //DISTANCIA ENTRE O PONTO ANTERIOR E O ATUAL É MENOR QUE 1M?SIM...SAIA DA FUNÇÃO

                GmapPositions.Markers.Clear();

                PrevLatitude = GPS_Position.Lat;
                PrevLongitude = GPS_Position.Lng;

                if (GmapFrameMode == 1)
                {
                    GmapPositions.Markers.Add(new GMapMarkerQuad(GPS_Position, Heading, 0, 0));
                }
                else if (GmapFrameMode == 2)
                {
                    GmapPositions.Markers.Add(new GMapMarkerHexaX(GPS_Position, Heading, 0, 0));
                }
                else if (GmapFrameMode == 3)
                {
                    GmapPositions.Markers.Add(new GMapMarkerHexaI(GPS_Position, Heading, 0, 0));
                }
                else if (GmapFrameMode == 4)
                {
                    GmapPositions.Markers.Add(new GMapMarkerAero(GPS_Position, Heading, 0, 0));
                }
                if (ArmDisarm)
                {
                    GMapTack.Points.Add(GPS_Position);
                    MyGmap.Position = GPS_Position;
                }
                MyGmap.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MarkersOverlay.Markers.Clear();
            GmapPolygons.Polygons.Clear();
            GmapPolygons.Clear();
            CountWP = 0;
            WPLat = 0;
            WPLon = 0;
            InvalidWP = false;
            label19.Text = Convert.ToString(0);
            label18.Text = Convert.ToString(0);
            label20.Text = Convert.ToString(0);
            label16.Text = Convert.ToString(0);
            label21.Text = Convert.ToString(0);
            label22.Text = Convert.ToString(0);
            label37.Text = Convert.ToString(0);
            label35.Text = Convert.ToString(0);
            label30.Text = Convert.ToString(0);
            label26.Text = Convert.ToString(0);
            label28.Text = Convert.ToString(0);
            label24.Text = Convert.ToString(0);
            label61.Text = Convert.ToString(0);
            label57.Text = Convert.ToString(0);
            label70.Text = Convert.ToString(0);
            label63.Text = Convert.ToString(0);
            label68.Text = Convert.ToString(0);
            label59.Text = Convert.ToString(0);
            label66.Text = Convert.ToString(0);
            label55.Text = Convert.ToString(0);
            label42.Text = Convert.ToString("Distância entre P1 - P2: 0M");
            label43.Text = Convert.ToString("Distância entre P2 - P3: 0M");
            label44.Text = Convert.ToString("Distância entre P3 - P4: 0M");
            label45.Text = Convert.ToString("Distância entre P4 - P5: 0M");
            label40.Text = Convert.ToString("Distância entre P5 - P6: 0M");
            label78.Text = Convert.ToString("Distância entre P6 - P7: 0M");
            label77.Text = Convert.ToString("Distância entre P7 - P8: 0M");
            label76.Text = Convert.ToString("Distância entre P8 - P9: 0M");
            label75.Text = Convert.ToString("Distância entre P9 - P10: 0M");
            SendReset(SerialPort);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendSave(SerialPort);
        }

        public void SendReset(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_WPReset;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void SendSave(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_WPSave;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void SendWayPoints(SerialPort serialSerialPort)
        {

            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;

            if (serialSerialPort.IsOpen)
            {
                if (WPLatVect1 != 0 && WPLonVect1 != 0)
                {
                    WayPoint1Latitude = Convert.ToInt32(WPLatVect1 * 1e7);
                    WayPoint1Longitude = Convert.ToInt32(WPLonVect1 * 1e7);
                    //ENVIA O PRIMEIRO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    //COORDENADAS LATITUDE E LONGITUDE
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP1;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint1Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox1.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox1.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox1.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox1.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox1.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox1.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox1.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox1.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox1.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox1.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox1.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox1.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox1.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox1.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox1.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox1.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox1.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox1.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox1.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox1.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox1.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox1.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox1.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox1.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox1.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox1.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox1.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox2.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox2.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox2.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox2.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox2.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed1);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect2 != 0 && WPLonVect2 != 0)
                {
                    WayPoint2Latitude = Convert.ToInt32(WPLatVect2 * 1e7);
                    WayPoint2Longitude = Convert.ToInt32(WPLonVect2 * 1e7);
                    //ENVIA O SEGUNDO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP2;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint2Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox4.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox4.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox4.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox4.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox4.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox4.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox4.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox4.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox4.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox4.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox4.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox4.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox4.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox4.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox4.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox4.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox4.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox4.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox4.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox4.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox4.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox4.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox4.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox4.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox4.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox4.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox4.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox3.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox3.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox3.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox3.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox3.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed2);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect3 != 0 && WPLonVect3 != 0)
                {
                    WayPoint3Latitude = Convert.ToInt32(WPLatVect3 * 1e7);
                    WayPoint3Longitude = Convert.ToInt32(WPLonVect3 * 1e7);
                    //ENVIA O TERCEIRO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP3;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint3Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox6.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox6.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox6.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox6.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox6.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox6.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox6.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox6.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox6.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox6.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox6.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox6.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox6.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox6.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox6.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox6.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox6.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox6.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox6.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox6.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox6.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox6.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox6.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox6.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox6.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox6.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox6.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox5.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox5.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox5.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox5.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox5.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed3);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect4 != 0 && WPLonVect4 != 0)
                {
                    WayPoint4Latitude = Convert.ToInt32(WPLatVect4 * 1e7);
                    WayPoint4Longitude = Convert.ToInt32(WPLonVect4 * 1e7);
                    //ENVIA O QUARTO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP4;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint4Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox8.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox8.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox8.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox8.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox8.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox8.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox8.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox8.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox8.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox8.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox8.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox8.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox8.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox8.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox8.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox8.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox8.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox8.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox8.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox8.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox8.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox8.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox8.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox8.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox8.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox8.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox8.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox7.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox7.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox7.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox7.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox7.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed4);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect5 != 0 && WPLonVect5 != 0)
                {
                    WayPoint5Latitude = Convert.ToInt32(WPLatVect5 * 1e7);
                    WayPoint5Longitude = Convert.ToInt32(WPLonVect5 * 1e7);
                    //ENVIA O QUINTO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP5;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint5Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox10.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox10.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox10.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox10.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox10.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox10.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox10.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox10.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox10.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox10.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox10.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox10.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox10.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox10.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox10.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox10.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox10.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox10.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox10.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox10.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox10.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox10.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox10.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox10.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox10.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox10.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox10.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox9.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox9.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox9.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox9.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox9.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed5);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect6 != 0 && WPLonVect6 != 0)
                {
                    WayPoint6Latitude = Convert.ToInt32(WPLatVect6 * 1e7);
                    WayPoint6Longitude = Convert.ToInt32(WPLonVect6 * 1e7);
                    //ENVIA O SEXTO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP6;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint6Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox12.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox12.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox12.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox12.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox12.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox12.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox12.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox12.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox12.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox12.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox12.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox12.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox12.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox12.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox12.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox12.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox12.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox12.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox12.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox12.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox12.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox12.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox12.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox12.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox12.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox12.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox12.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox11.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox11.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox11.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox11.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox11.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed6);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect7 != 0 && WPLonVect7 != 0)
                {
                    WayPoint7Latitude = Convert.ToInt32(WPLatVect7 * 1e7);
                    WayPoint7Longitude = Convert.ToInt32(WPLonVect7 * 1e7);
                    //ENVIA O SETIMO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP7;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint7Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox15.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox15.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox15.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox15.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox15.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox15.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox15.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox15.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox15.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox15.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox15.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox15.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox15.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox15.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox15.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox15.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox15.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox15.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox15.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox15.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox15.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox15.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox15.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox15.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox15.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox15.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox15.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox14.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox14.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox14.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox14.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox14.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed7);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect8 != 0 && WPLonVect8 != 0)
                {
                    WayPoint8Latitude = Convert.ToInt32(WPLatVect8 * 1e7);
                    WayPoint8Longitude = Convert.ToInt32(WPLonVect8 * 1e7);
                    //ENVIA O OITAVO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP8;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint8Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox17.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox17.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox17.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox17.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox17.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox17.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox17.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox17.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox17.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox17.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox17.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox17.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox17.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox17.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox17.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox17.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox17.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox17.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox17.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox17.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox17.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox17.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox17.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox17.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox17.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox17.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox17.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox16.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox16.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox16.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox16.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox16.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed8);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect9 != 0 && WPLonVect9 != 0)
                {
                    WayPoint9Latitude = Convert.ToInt32(WPLatVect9 * 1e7);
                    WayPoint9Longitude = Convert.ToInt32(WPLonVect9 * 1e7);
                    //ENVIA O NONO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP9;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint9Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox19.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox19.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox19.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox19.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox19.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox19.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox19.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox19.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox19.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox19.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox19.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox19.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox19.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox19.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox19.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox19.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox19.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox19.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox19.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox19.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox19.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox19.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox19.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox19.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox19.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox19.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox19.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox18.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox18.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox18.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox18.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox18.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed9);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }

                if (WPLatVect10 != 0 && WPLonVect10 != 0)
                {
                    WayPoint10Latitude = Convert.ToInt32(WPLatVect10 * 1e7);
                    WayPoint10Longitude = Convert.ToInt32(WPLonVect10 * 1e7);
                    //ENVIA O NONO WAYPOINT SE DISPONIVEL
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4A;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x5D;
                    SendBuffer[VectorPointer++] = 11;
                    SendBuffer[VectorPointer++] = (byte)KEY_WP10;
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Latitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Latitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Latitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Latitude >> 24);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Longitude);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Longitude >> 8);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Longitude >> 16);
                    SendBuffer[VectorPointer++] = (byte)(WayPoint10Longitude >> 24);
                    //ALTITUDE DE SUBIDA
                    if (Convert.ToString(comboBox21.SelectedItem) == "10M") SendBuffer[VectorPointer++] = (byte)(0);
                    if (Convert.ToString(comboBox21.SelectedItem) == "15M") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox21.SelectedItem) == "20M") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox21.SelectedItem) == "25M") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox21.SelectedItem) == "30M") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox21.SelectedItem) == "35M") SendBuffer[VectorPointer++] = (byte)(5);
                    if (Convert.ToString(comboBox21.SelectedItem) == "40M") SendBuffer[VectorPointer++] = (byte)(6);
                    if (Convert.ToString(comboBox21.SelectedItem) == "45M") SendBuffer[VectorPointer++] = (byte)(7);
                    if (Convert.ToString(comboBox21.SelectedItem) == "50M") SendBuffer[VectorPointer++] = (byte)(8);
                    if (Convert.ToString(comboBox21.SelectedItem) == "55M") SendBuffer[VectorPointer++] = (byte)(9);
                    if (Convert.ToString(comboBox21.SelectedItem) == "60M") SendBuffer[VectorPointer++] = (byte)(10);
                    if (Convert.ToString(comboBox21.SelectedItem) == "65M") SendBuffer[VectorPointer++] = (byte)(11);
                    if (Convert.ToString(comboBox21.SelectedItem) == "70M") SendBuffer[VectorPointer++] = (byte)(12);
                    if (Convert.ToString(comboBox21.SelectedItem) == "75M") SendBuffer[VectorPointer++] = (byte)(13);
                    if (Convert.ToString(comboBox21.SelectedItem) == "80M") SendBuffer[VectorPointer++] = (byte)(14);
                    if (Convert.ToString(comboBox21.SelectedItem) == "85M") SendBuffer[VectorPointer++] = (byte)(15);
                    if (Convert.ToString(comboBox21.SelectedItem) == "90M") SendBuffer[VectorPointer++] = (byte)(16);
                    if (Convert.ToString(comboBox21.SelectedItem) == "95M") SendBuffer[VectorPointer++] = (byte)(17);
                    if (Convert.ToString(comboBox21.SelectedItem) == "100M") SendBuffer[VectorPointer++] = (byte)(18);
                    if (Convert.ToString(comboBox21.SelectedItem) == "105M") SendBuffer[VectorPointer++] = (byte)(19);
                    if (Convert.ToString(comboBox21.SelectedItem) == "110M") SendBuffer[VectorPointer++] = (byte)(20);
                    if (Convert.ToString(comboBox21.SelectedItem) == "115M") SendBuffer[VectorPointer++] = (byte)(21);
                    if (Convert.ToString(comboBox21.SelectedItem) == "120M") SendBuffer[VectorPointer++] = (byte)(22);
                    if (Convert.ToString(comboBox21.SelectedItem) == "125M") SendBuffer[VectorPointer++] = (byte)(23);
                    if (Convert.ToString(comboBox21.SelectedItem) == "130M") SendBuffer[VectorPointer++] = (byte)(24);
                    if (Convert.ToString(comboBox21.SelectedItem) == "135M") SendBuffer[VectorPointer++] = (byte)(25);
                    if (Convert.ToString(comboBox21.SelectedItem) == "140M") SendBuffer[VectorPointer++] = (byte)(26);
                    //MODO DE VOO
                    if (Convert.ToString(comboBox20.SelectedItem) == "PROX.WP") SendBuffer[VectorPointer++] = (byte)(1);
                    if (Convert.ToString(comboBox20.SelectedItem) == "GPS-HOLD") SendBuffer[VectorPointer++] = (byte)(2);
                    if (Convert.ToString(comboBox20.SelectedItem) == "LAND") SendBuffer[VectorPointer++] = (byte)(3);
                    if (Convert.ToString(comboBox20.SelectedItem) == "RTH") SendBuffer[VectorPointer++] = (byte)(4);
                    if (Convert.ToString(comboBox20.SelectedItem) == "TAKEOFF") SendBuffer[VectorPointer++] = (byte)(5);
                    SendBuffer[VectorPointer++] = (byte)(GPSHoldTimed10);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    serialSerialPort.Write(SendBuffer, 0, VectorPointer);
                }
            }
        }

        Boolean MouseClickedComboBox2 = false;
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox2)
            {
                return;
            }

            if (comboBox2.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 1";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed1 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox2 = false;
        }

        private void comboBox2_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox2 = false;
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox2 = false;
        }

        private void comboBox2_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox2 = true;
        }

        Boolean MouseClickedComboBox3 = false;
        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox3)
            {
                return;
            }

            if (comboBox3.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 2";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed2 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox3_KeyUp_1(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox3 = false;
        }

        private void comboBox3_KeyDown_1(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox3 = false;
        }

        private void comboBox3_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox3 = false;
        }

        private void comboBox3_MouseDown_1(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox3 = true;
        }

        Boolean MouseClickedComboBox5 = false;
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox5)
            {
                return;
            }

            if (comboBox5.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 3";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed3 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox5_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox5 = false;
        }

        private void comboBox5_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox5 = false;
        }

        private void comboBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox5 = false;
        }

        private void comboBox5_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox5 = true;
        }

        Boolean MouseClickedComboBox7 = false;
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox7)
            {
                return;
            }

            if (comboBox7.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 4";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed4 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox7_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox7 = false;
        }

        private void comboBox7_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox7 = false;
        }

        private void comboBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox7 = false;
        }

        private void comboBox7_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox7 = true;
        }

        Boolean MouseClickedComboBox9 = false;
        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox9)
            {
                return;
            }

            if (comboBox9.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 5";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed5 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox9_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox9 = false;
        }

        private void comboBox9_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox9 = false;
        }

        private void comboBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox9 = false;
        }

        private void comboBox9_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox9 = true;
        }

        Boolean MouseClickedComboBox11 = false;
        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox11)
            {
                return;
            }

            if (comboBox11.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 6";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed6 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox11_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox11 = false;
        }

        private void comboBox11_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox11 = false;
        }

        private void comboBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox11 = false;
        }

        private void comboBox11_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox11 = true;
        }

        Boolean MouseClickedComboBox14 = false;
        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox14)
            {
                return;
            }

            if (comboBox14.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 7";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed7 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox14_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox14 = false;
        }

        private void comboBox14_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox14 = false;
        }

        private void comboBox14_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox14 = false;
        }

        private void comboBox14_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox14 = true;
        }

        Boolean MouseClickedComboBox16 = false;
        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox16)
            {
                return;
            }

            if (comboBox16.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 8";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed8 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox16_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox16 = false;
        }

        private void comboBox16_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox16 = false;
        }

        private void comboBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox16 = false;
        }

        private void comboBox16_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox16 = true;
        }

        Boolean MouseClickedComboBox18 = false;
        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox18)
            {
                return;
            }

            if (comboBox18.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 9";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed9 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox18_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox18 = false;
        }

        private void comboBox18_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox18 = false;
        }

        private void comboBox18_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox18 = false;
        }

        private void comboBox18_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox18 = true;
        }

        Boolean MouseClickedComboBox20 = false;
        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!MouseClickedComboBox20)
            {
                return;
            }

            if (comboBox20.Text == "GPS-HOLD")
            {
                string message, tittle, defaultValue;
                object MyValue;

                message = "Defina o tempo de GPS-HOLD nesse WP,após o estouro desse tempo a controladora irá para o proximo WP." +
                    "O valor do tempo é configuravel de 1 a 254 Segundos (Nunca coloque o tempo em Minutos,sempre em segundos)." +
                    Environment.NewLine + "1 Minuto:60 Segundos" +
                    Environment.NewLine + "2 Minutos:120 Segundos" +
                    Environment.NewLine + "3 Minutos:180 Segundos" +
                    Environment.NewLine + "4 Minutos:240 Segundos" +
                    Environment.NewLine + " ";

                tittle = "Timerização para o modo GPS-HOLD do WayPoint 10";

                defaultValue = "1";

                MyValue = Interaction.InputBox(message, tittle, defaultValue);

                if ((string)MyValue == "")
                {
                    MyValue = defaultValue;
                }
                else
                {
                    GPSHoldTimed10 = Convert.ToByte(MyValue);
                }
            }
        }

        private void comboBox20_KeyUp(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox20 = false;
        }

        private void comboBox20_KeyDown(object sender, KeyEventArgs e)
        {
            MouseClickedComboBox20 = false;
        }

        private void comboBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            MouseClickedComboBox20 = false;
        }

        private void comboBox20_MouseDown(object sender, MouseEventArgs e)
        {
            MouseClickedComboBox20 = true;
        }

        void comboBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox2_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox3_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox4_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox5_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox6_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox7_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox8_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox9_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox10_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox11_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox12_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox13_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox14_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox15_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox16_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox17_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox18_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox19_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox20_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void comboBox21_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void trackBar1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }
}
