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
        SerialPort SerialPort = new SerialPort();
        string SerialComPort;
        string[] SerialPorts = SerialPort.GetPortNames();

        static Boolean Error_Received = false;
        static byte Read_State = 0;
        static byte OffSet = 0;
        static byte DataSize = 0;
        static byte CheckSum = 0;
        static byte Command;
        static byte[] InBuffer = new byte[300];

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

        string GPSLAT = "0";
        string GPSLONG = "0";

        Boolean PushLocation = false;

        double NumSat;
        double VBatt;
        double HDOP = 99.99;
        double Altitude;

        int Heading = 0;
        int ReadPitch = 0;

        byte GmapFrameMode = 0;

        GMapOverlay MarkersOverlay = new GMapOverlay("Markers");
        GMapOverlay GmapPolygons = new GMapOverlay("Poligonos");

        private List<PointLatLng> WPCoordinates;
        private List<PointLatLng> WPCoordinates2;

        static GMapOverlay GmapRoutes;
        static GMapRoute GMapTack;
        List<PointLatLng> LatLngPoints = new List<PointLatLng>();
        PointLatLng GPS_Position;
        GMapOverlay GmapPositions;

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

        byte ArmDisarm = 0;

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

        double[] PushedLatitude = new double[10];
        double[] PushedLongitude = new double[10];
        byte[] PushedComboBox = new byte[30];
        Boolean ParamsPushed = false;
        Boolean PrintArea2 = false;
        byte CountWP2 = 0;
        PointLatLng GPS_Position2;
        byte CountToBlock = 0;
        Boolean BlockPushParams = false;

        int CoG = 0;
        Int32 Crosstrack = 0;

        int WPRadius = 200;

        new static bool MouseDown = false;
        static bool MouseDraging = false;
        GMapMarkerRect CurrentRectMarker = null;
        GMarkerGoogle CurrentMarker;
        PointLatLng Start;
        GMapMarker Center;
        static GMapOverlay GMOverlayLiveData;

        Form WaitUart = Program.WaitUart;

        public WayPoint()
        {
            InitializeComponent();
            MyGMap.PolygonsEnabled = true;
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
            MyGMap.ShowCenter = false;
            MyGMap.Manager.Mode = AccessMode.ServerAndCache;
            //MyGMap.MapProvider = GMapProviders.BingSatelliteMap;
            //MyGMap.MapProvider = GMapProviders.BingHybridMap;
            MyGMap.MapProvider = GMapProviders.GoogleSatelliteMap;
            MyGMap.Zoom = 2;
            GmapPositions = new GMapOverlay("GmapPositions");
            MyGMap.Overlays.Add(GmapPositions);
            GmapPositions.Markers.Clear();
            GmapRoutes = new GMapOverlay("GMapRoutes");
            MyGMap.Overlays.Add(GmapRoutes);
            Pen penRoute = new Pen(Color.Purple, 5);
            GMapTack = new GMapRoute(LatLngPoints, "GMapTrack");
            GMapTack.Stroke = penRoute;
            GmapRoutes.Routes.Add(GMapTack);
            CurrentMarker = new GMarkerGoogle(MyGMap.Position, GMarkerGoogleType.red);
            Center = new GMarkerGoogle(MyGMap.Position, GMarkerGoogleType.white_small);
            GMOverlayLiveData = new GMapOverlay("LiveData");
            MyGMap.Overlays.Add(GMOverlayLiveData);
            GMOverlayLiveData.Markers.Clear();
            trackBar1.Value = 2;
            trackBar1.Minimum = 2;
            trackBar1.Maximum = 20;
            label41.Parent = MyGMap;
            label41.BackColor = Color.Blue;
            label41.ForeColor = Color.White;
            label46.Parent = MyGMap;
            label46.BackColor = Color.Blue;
            label46.ForeColor = Color.White;
            label47.Parent = MyGMap;
            label47.BackColor = Color.Blue;
            label47.ForeColor = Color.White;
            label48.Parent = MyGMap;
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

        private void gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (ModifierKeys != Keys.Control) return;
            if (e.Button == MouseButtons.Left)
            {
                if (!InvalidWP)
                {
                    CountWP++;
                    var WayPoint = MyGMap.FromLocalToLatLng(e.X, e.Y);
                    WPLat = WayPoint.Lat;
                    WPLon = WayPoint.Lng;
                    PointLatLng ActualPoint = new PointLatLng(WPLat, WPLon);
                    GMapMarker GMarker = new CreateGMapMarker(ActualPoint, Convert.ToByte(CountWP))
                    {
                        Tag = Convert.ToString(CountWP)
                    };
                    GMapMarkerRect MarkerRect = new GMapMarkerRect(ActualPoint);
                    {
                        MarkerRect.InnerMarker = GMarker;
                        MarkerRect.WPRadius = (int)WPRadius / 10;
                        MarkerRect.MainMap = MyGMap;
                        MarkerRect.Color = Color.White;
                    }
                    MarkersOverlay.Markers.Add(GMarker);
                    MarkersOverlay.Markers.Add(MarkerRect);
                    MyGMap.Overlays.Add(MarkersOverlay);
                    MyGMap.UpdateMarkerLocalPosition(GMarker);
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
                MyGMap.Position = new PointLatLng(Convert.ToDouble(InitialLat / 10000000.0), Convert.ToDouble(InitialLong / 10000000.0));
                MyGMap.Zoom = 18;
                trackBar1.Value = 18;
                timer1.Enabled = false;
                if (timer1.Enabled == false) DebugMap = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;
            SendWayPoints(SerialPort);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            SerialComPort = Convert.ToString(comboBox13.SelectedItem);
            try
            {
                Program.WaitUart.Show();
                SerialPort.PortName = SerialComPort;
                SerialPort.BaudRate = 115200;
                SerialPort.DataBits = 8;
                SerialPort.Parity = Parity.None;
                SerialPort.StopBits = StopBits.One;
                SerialPort.Open();

                for (Int32 i = 0; i < 300; i++)
                {
                    WaitUart.Refresh();
                    Thread.Sleep(10);
                }

                if (!PingTest.PingNetwork("pingtest.com"))
                {
                    MyGMap.Manager.Mode = AccessMode.CacheOnly;
                    MessageBox.Show("Você está sem internet,o mapa irá funcinar em modo cache,partes do mapa não carregados antes com internet podem falhar", "Checagem de conexão com a internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (SerialPort.IsOpen == true)
                {
                    comboBox13.Enabled = false;
                    pictureBox1.Image = Properties.Resources.Conectado;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("A conexão falhou!Acho que a Porta Serial está aberta em outro programa,verifique se ela não está aberta na tela principal do GCS.");
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
            CountWP2 = 0;
            ParamsPushed = false;
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
            byte CheckState;

            if (SerialPort.IsOpen == false) return;

            while (SerialPort.BytesToRead > 0)
            {
                CheckState = (byte)SerialPort.ReadByte();

                switch (Read_State)
                {
                    case 0:
                        Read_State = (CheckState == 0x4a) ? (byte)1 : (byte)0;
                        break;

                    case 1:
                        Read_State = (CheckState == 0x43) ? (byte)2 : (byte)0;
                        break;

                    case 2:
                        if (CheckState == 0x46)
                        {
                            Read_State = 3;
                        }
                        else if (CheckState == 0x21)
                        {
                            Read_State = 6;
                        }
                        else
                        {
                            Read_State = 0;
                        }
                        break;

                    case 3:
                    case 6:
                        Error_Received = (Read_State == 6);
                        DataSize = CheckState;
                        OffSet = 0;
                        CheckSum = 0;
                        CheckSum ^= CheckState;
                        Read_State = 4;
                        if (DataSize > 100)
                        {
                            Read_State = 0;
                        }

                        break;

                    case 4:
                        Command = CheckState;
                        CheckSum ^= CheckState;
                        Read_State = 5;
                        break;

                    case 5:
                        if (OffSet < DataSize)
                        {
                            CheckSum ^= CheckState;
                            InBuffer[OffSet++] = CheckState;
                        }
                        else
                        {
                            if (CheckSum == CheckState)
                            {
                                if (!Error_Received)
                                {
                                    Serial_Parse(Command);
                                }
                            }
                            Read_State = 0;
                        }
                        break;
                }
            }
        }

        private void Serial_Parse(byte Command)
        {
            int ptr;
            switch (Command)
            {

                case 1:
                    ptr = 0;
                    if (!ParamsPushed)
                    {
                        PushedLatitude[0] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[1] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[2] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[3] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[4] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[5] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[6] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[7] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[8] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLatitude[9] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[0] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[1] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[2] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[3] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[4] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[5] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[6] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[7] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[8] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                        PushedLongitude[9] = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr) / 1e7); ptr += 4;
                    }
                    break;

                case 2:
                    ptr = 0;
                    if (!ParamsPushed)
                    {
                        PushedComboBox[0] = (byte)InBuffer[ptr++];
                        PushedComboBox[1] = (byte)InBuffer[ptr++];
                        PushedComboBox[2] = (byte)InBuffer[ptr++];
                        PushedComboBox[3] = (byte)InBuffer[ptr++];
                        PushedComboBox[4] = (byte)InBuffer[ptr++];
                        PushedComboBox[5] = (byte)InBuffer[ptr++];
                        PushedComboBox[6] = (byte)InBuffer[ptr++];
                        PushedComboBox[7] = (byte)InBuffer[ptr++];
                        PushedComboBox[8] = (byte)InBuffer[ptr++];
                        PushedComboBox[9] = (byte)InBuffer[ptr++];
                        PushedComboBox[10] = (byte)InBuffer[ptr++];
                        PushedComboBox[11] = (byte)InBuffer[ptr++];
                        PushedComboBox[12] = (byte)InBuffer[ptr++];
                        PushedComboBox[13] = (byte)InBuffer[ptr++];
                        PushedComboBox[14] = (byte)InBuffer[ptr++];
                        PushedComboBox[15] = (byte)InBuffer[ptr++];
                        PushedComboBox[16] = (byte)InBuffer[ptr++];
                        PushedComboBox[17] = (byte)InBuffer[ptr++];
                        PushedComboBox[18] = (byte)InBuffer[ptr++];
                        PushedComboBox[19] = (byte)InBuffer[ptr++];
                    }
                    if (!BlockPushParams) ParamsPushed = true;
                    break;

                case 7:
                    ptr = 0;
                    ReadPitch = BitConverter.ToInt16(InBuffer, ptr); ptr += 4;
                    Heading = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ptr += 25;
                    NumSat = (byte)InBuffer[ptr++];
                    GPSLAT = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    GPSLONG = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    InitialLat = double.Parse(GPSLAT);
                    InitialLong = double.Parse(GPSLONG);
                    if (NumSat >= 5 && InitialLat != 0 && InitialLong != 0) PushLocation = true;
                    ptr += 8;
                    Altitude = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr)) / 100; ptr += 4;
                    ptr += 1;
                    VBatt = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    ptr += 1;
                    ArmDisarm = (byte)InBuffer[ptr++];
                    HDOP = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    ptr += 7;
                    GmapFrameMode = (byte)InBuffer[ptr++];
                    ptr += 6;
                    CoG = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Crosstrack = BitConverter.ToInt16(InBuffer, ptr); ptr += 4;
                    break;

            }
        }

        private void Serial_Write_To_FC(int Command)
        {
            byte CheckSum = 0;
            byte[] Buffer;
            Buffer = new byte[10];
            Buffer[0] = (byte)0x4a;
            Buffer[1] = (byte)0x43;
            Buffer[2] = (byte)0x3c;
            Buffer[3] = (byte)0;
            CheckSum ^= Buffer[3];
            Buffer[4] = (byte)Command;
            CheckSum ^= Buffer[4];
            Buffer[5] = (byte)CheckSum;
            SerialPort.Write(Buffer, 0, 6);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            WPCoordinates2 = new List<PointLatLng>();
            if (SerialPort.IsOpen)
            {
                Program.WaitUart.Close();
                if (SerialPort.BytesToRead == 0)
                {
                    if (!ParamsPushed)
                    {
                        Serial_Write_To_FC(1);
                        Serial_Write_To_FC(2);
                    }

                    Serial_Write_To_FC(7);

                    if (ParamsPushed)
                    {
                        if (PushedLatitude[0] != 0 && PushedLongitude[0] != 0 && !PrintArea2 && CountWP2 == 0)
                        {
                            label19.Text = Convert.ToString(PushedLatitude[0]);
                            label18.Text = Convert.ToString(PushedLongitude[0]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[0];
                            GPS_Position2.Lng = PushedLongitude[0];
                            MyGMap.Zoom = 17;
                            trackBar1.Value = 17;
                        }

                        if (PushedLatitude[1] != 0 && PushedLongitude[1] != 0 && !PrintArea2 && CountWP2 == 1)
                        {
                            label20.Text = Convert.ToString(PushedLatitude[1]);
                            label16.Text = Convert.ToString(PushedLongitude[1]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[1];
                            GPS_Position2.Lng = PushedLongitude[1];
                            Dist1 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[0], WPCoordinates2[1]) * 1000;
                            double Dist1Float = Convert.ToDouble(Convert.ToInt32(Dist1)) / 1000;
                            if (Dist1 >= 1000) label42.Text = "Distância entre P1 - P2: " + Dist1Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label42.Text = "Distância entre P1 - P2: " + Convert.ToInt32(Dist1) + "M";
                        }

                        if (PushedLatitude[2] != 0 && PushedLongitude[2] != 0 && !PrintArea2 && CountWP2 == 2)
                        {
                            label21.Text = Convert.ToString(PushedLatitude[2]);
                            label22.Text = Convert.ToString(PushedLongitude[2]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[2];
                            GPS_Position2.Lng = PushedLongitude[2];
                            Dist2 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[1], WPCoordinates2[2]) * 1000;
                            double Dist2Float = Convert.ToDouble(Convert.ToInt32(Dist2)) / 1000;
                            if (Dist2 >= 1000) label43.Text = "Distância entre P2 - P3: " + Dist2Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label43.Text = "Distância entre P2 - P3: " + Convert.ToInt32(Dist2) + "M";
                        }

                        if (PushedLatitude[3] != 0 && PushedLongitude[3] != 0 && !PrintArea2 && CountWP2 == 3)
                        {
                            label37.Text = Convert.ToString(PushedLatitude[3]);
                            label35.Text = Convert.ToString(PushedLongitude[3]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[3];
                            GPS_Position2.Lng = PushedLongitude[3];
                            Dist3 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[2], WPCoordinates2[3]) * 1000;
                            double Dist3Float = Convert.ToDouble(Convert.ToInt32(Dist3)) / 1000;
                            if (Dist3 >= 1000) label44.Text = "Distância entre P3 - P4: " + Dist3Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label44.Text = "Distância entre P3 - P4: " + Convert.ToInt32(Dist3) + "M";
                        }

                        if (PushedLatitude[4] != 0 && PushedLongitude[4] != 0 && !PrintArea2 && CountWP2 == 4)
                        {
                            label30.Text = Convert.ToString(PushedLatitude[4]);
                            label26.Text = Convert.ToString(PushedLongitude[4]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[4];
                            GPS_Position2.Lng = PushedLongitude[4];
                            Dist4 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[3], WPCoordinates2[4]) * 1000;
                            double Dist4Float = Convert.ToDouble(Convert.ToInt32(Dist4)) / 1000;
                            if (Dist4 >= 1000) label45.Text = "Distância entre P4 - P5: " + Dist4Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label45.Text = "Distância entre P4 - P5: " + Convert.ToInt32(Dist4) + "M";
                        }

                        if (PushedLatitude[5] != 0 && PushedLongitude[5] != 0 && !PrintArea2 && CountWP2 == 5)
                        {
                            label28.Text = Convert.ToString(PushedLatitude[5]);
                            label24.Text = Convert.ToString(PushedLongitude[5]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[5];
                            GPS_Position2.Lng = PushedLongitude[5];
                            Dist5 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[4], WPCoordinates2[5]) * 1000;
                            double Dist5Float = Convert.ToDouble(Convert.ToInt32(Dist5)) / 1000;
                            if (Dist5 >= 1000) label40.Text = "Distância entre P5 - P6: " + Dist5Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label40.Text = "Distância entre P5 - P6: " + Convert.ToInt32(Dist5) + "M";
                        }

                        if (PushedLatitude[6] != 0 && PushedLongitude[6] != 0 && !PrintArea2 && CountWP2 == 6)
                        {
                            label61.Text = Convert.ToString(PushedLatitude[6]);
                            label57.Text = Convert.ToString(PushedLongitude[6]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[6];
                            GPS_Position2.Lng = PushedLongitude[6];
                            Dist6 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[5], WPCoordinates2[6]) * 1000;
                            double Dist6Float = Convert.ToDouble(Convert.ToInt32(Dist6)) / 1000;
                            if (Dist6 >= 1000) label78.Text = "Distância entre P6 - P7: " + Dist6Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label78.Text = "Distância entre P6 - P7: " + Convert.ToInt32(Dist6) + "M";
                        }

                        if (PushedLatitude[7] != 0 && PushedLongitude[7] != 0 && !PrintArea2 && CountWP2 == 7)
                        {
                            label70.Text = Convert.ToString(PushedLatitude[7]);
                            label63.Text = Convert.ToString(PushedLongitude[7]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[7];
                            GPS_Position2.Lng = PushedLongitude[7];
                            Dist7 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[6], WPCoordinates2[7]) * 1000;
                            double Dist7Float = Convert.ToDouble(Convert.ToInt32(Dist7)) / 1000;
                            if (Dist7 >= 1000) label77.Text = "Distância entre P7 - P8: " + Dist7Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label77.Text = "Distância entre P7 - P8: " + Convert.ToInt32(Dist7) + "M";
                        }

                        if (PushedLatitude[8] != 0 && PushedLongitude[8] != 0 && !PrintArea2 && CountWP2 == 8)
                        {
                            label68.Text = Convert.ToString(PushedLatitude[8]);
                            label59.Text = Convert.ToString(PushedLongitude[8]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[8]), Convert.ToDouble(PushedLongitude[8])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[8];
                            GPS_Position2.Lng = PushedLongitude[8];
                            Dist8 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[7], WPCoordinates2[8]) * 1000;
                            double Dist8Float = Convert.ToDouble(Convert.ToInt32(Dist8)) / 1000;
                            if (Dist8 >= 1000) label76.Text = "Distância entre P8 - P9: " + Dist8Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label76.Text = "Distância entre P8 - P9: " + Convert.ToInt32(Dist8) + "M";
                        }

                        if (PushedLatitude[9] != 0 && PushedLongitude[9] != 0 && !PrintArea2 && CountWP2 == 9)
                        {
                            label66.Text = Convert.ToString(PushedLatitude[9]);
                            label55.Text = Convert.ToString(PushedLongitude[9]);
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[8]), Convert.ToDouble(PushedLongitude[8])));
                            WPCoordinates2.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[9]), Convert.ToDouble(PushedLongitude[9])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[9];
                            GPS_Position2.Lng = PushedLongitude[9];
                            Dist9 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates2[8], WPCoordinates2[9]) * 1000;
                            double Dist9Float = Convert.ToDouble(Convert.ToInt32(Dist9)) / 1000;
                            if (Dist9 >= 1000) label75.Text = "Distância entre P9 - P10: " + Dist9Float.ToString(new CultureInfo("en-US")) + "KM";
                            else label75.Text = "Distância entre P9 - P10: " + Convert.ToInt32(Dist9) + "M";
                        }

                        if (PrintArea2)
                        {
                            CountWP2++;
                            GMapMarker GMarker = new CreateGMapMarker(GPS_Position2, Convert.ToByte(CountWP2))
                            {
                                Tag = Convert.ToString(CountWP2)
                            };
                            GMapMarkerRect MarkerRect = new GMapMarkerRect(GPS_Position2);
                            {
                                MarkerRect.InnerMarker = GMarker;
                                MarkerRect.WPRadius = (int)WPRadius / 10;
                                MarkerRect.MainMap = MyGMap;
                                MarkerRect.Color = Color.White;
                            }
                            MarkersOverlay.Markers.Add(GMarker);
                            MarkersOverlay.Markers.Add(MarkerRect);
                            MyGMap.Overlays.Add(MarkersOverlay);
                            MyGMap.UpdateMarkerLocalPosition(GMarker);
                            GmapPolygons.Polygons.Clear();
                            GMapRoute FirstPointTrace = new GMapRoute("FirstPointTrace");
                            FirstPointTrace.Clear();
                            GmapPolygons.Clear();
                            FirstPointTrace.Stroke = new Pen(Color.Violet, 4);
                            FirstPointTrace.Stroke.DashStyle = DashStyle.Dash;
                            if (CountWP2 > 2)
                            {
                                FirstPointTrace.Points.Add(WPCoordinates2[0]);
                                FirstPointTrace.Points.Add(WPCoordinates2[WPCoordinates2.Count - 1]);
                            }
                            GMapRoute WPLineRoute = new GMapRoute("WPLineRoute");
                            WPLineRoute.Stroke = new Pen(Color.Green, 4);
                            WPLineRoute.Stroke.DashStyle = DashStyle.Custom;
                            for (int a = 0; a < WPCoordinates2.Count; a++) WPLineRoute.Points.Add(WPCoordinates2[a]);
                            GmapPolygons.Routes.Add(FirstPointTrace);
                            GmapPolygons.Routes.Add(WPLineRoute);
                            MyGMap.Overlays.Add(GmapPolygons);
                            PrintArea2 = false;
                        }
                        comboBox1.SelectedIndex = PushedComboBox[0];
                        comboBox4.SelectedIndex = PushedComboBox[1];
                        comboBox6.SelectedIndex = PushedComboBox[2];
                        comboBox8.SelectedIndex = PushedComboBox[3];
                        comboBox10.SelectedIndex = PushedComboBox[4];
                        comboBox12.SelectedIndex = PushedComboBox[5];
                        comboBox15.SelectedIndex = PushedComboBox[6];
                        comboBox17.SelectedIndex = PushedComboBox[7];
                        comboBox19.SelectedIndex = PushedComboBox[8];
                        comboBox21.SelectedIndex = PushedComboBox[9];
                        comboBox2.SelectedIndex = (PushedComboBox[10] > 0) ? PushedComboBox[10] - 1 : 0;
                        comboBox3.SelectedIndex = (PushedComboBox[11] > 0) ? PushedComboBox[11] - 1 : 0;
                        comboBox5.SelectedIndex = (PushedComboBox[12] > 0) ? PushedComboBox[12] - 1 : 0;
                        comboBox7.SelectedIndex = (PushedComboBox[13] > 0) ? PushedComboBox[13] - 1 : 0;
                        comboBox9.SelectedIndex = (PushedComboBox[14] > 0) ? PushedComboBox[14] - 1 : 0;
                        comboBox11.SelectedIndex = (PushedComboBox[15] > 0) ? PushedComboBox[15] - 1 : 0;
                        comboBox14.SelectedIndex = (PushedComboBox[16] > 0) ? PushedComboBox[16] - 1 : 0;
                        comboBox16.SelectedIndex = (PushedComboBox[17] > 0) ? PushedComboBox[17] - 1 : 0;
                        comboBox18.SelectedIndex = (PushedComboBox[18] > 0) ? PushedComboBox[18] - 1 : 0;
                        comboBox20.SelectedIndex = (PushedComboBox[19] > 0) ? PushedComboBox[19] - 1 : 0;
                        CountToBlock++;
                        if (CountToBlock > 80)
                        {
                            BlockPushParams = true;
                            ParamsPushed = false;
                            CountToBlock = 151;
                        }
                    }
                }
            }

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
                Dist1 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[0], WPCoordinates[1]) * 1000;
                double Dist1Float = Convert.ToDouble(Convert.ToInt32(Dist1)) / 1000;
                if (Dist1 >= 1000) label42.Text = "Distância entre P1 - P2: " + Dist1Float.ToString(new CultureInfo("en-US")) + "KM";
                else label42.Text = "Distância entre P1 - P2: " + Convert.ToInt32(Dist1) + "M";
            }

            if (CountWP == 3)
            {
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect1), Convert.ToDouble(WPLonVect1)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect2), Convert.ToDouble(WPLonVect2)));
                WPCoordinates.Add(new PointLatLng(Convert.ToDouble(WPLatVect3), Convert.ToDouble(WPLonVect3)));
                Dist2 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[1], WPCoordinates[2]) * 1000;
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
                Dist3 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[2], WPCoordinates[3]) * 1000;
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
                Dist4 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[3], WPCoordinates[4]) * 1000;
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
                Dist5 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[4], WPCoordinates[5]) * 1000;
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
                Dist6 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[5], WPCoordinates[6]) * 1000;
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
                Dist7 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[6], WPCoordinates[7]) * 1000;
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
                Dist8 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[7], WPCoordinates[8]) * 1000;
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
                Dist9 = MyGMap.MapProvider.Projection.GetDistance(WPCoordinates[8], WPCoordinates[9]) * 1000;
                double Dist9Float = Convert.ToDouble(Convert.ToInt32(Dist9)) / 1000;
                if (Dist9 >= 1000) label75.Text = "Distância entre P9 - P10: " + Dist9Float.ToString(new CultureInfo("en-US")) + "KM";
                else label75.Text = "Distância entre P9 - P10: " + Convert.ToInt32(Dist9) + "M";
            }

            if (PrintArea)
            {
                GMapRoute FirstPointTrace = new GMapRoute("FirstPointTrace");
                FirstPointTrace.Clear();
                GmapPolygons.Clear();
                FirstPointTrace.Stroke = new Pen(Color.Violet, 4);
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
                MyGMap.Overlays.Add(GmapPolygons);
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
            if (PushLocation) MyGMap.Zoom = trackBar1.Value;
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
            GPS_Position.Lat = double.Parse(GPSLAT) / 10000000;
            GPS_Position.Lng = double.Parse(GPSLONG) / 10000000;
            if (NumSat >= 5 && GPS_Position.Lat != 0 && GPS_Position.Lng != 0 &&
                PrevLatitude != GPS_Position.Lat && PrevLongitude != GPS_Position.Lng)
            {
                GmapPositions.Markers.Clear();
                PrevLatitude = GPS_Position.Lat;
                PrevLongitude = GPS_Position.Lng;
                byte TrackLength = 200;
                if (GMapTack.Points.Count > TrackLength) GMapTack.Points.RemoveRange(0, GMapTack.Points.Count - TrackLength);
                if (GmapFrameMode == 0)
                {
                    GmapPositions.Markers.Add(new GMapMarkerQuad(GPS_Position, Heading, CoG, Crosstrack));
                }
                else if (GmapFrameMode == 1)
                {
                    GmapPositions.Markers.Add(new GMapMarkerHexaX(GPS_Position, Heading, CoG, Crosstrack));
                }
                else if (GmapFrameMode == 2)
                {
                    GmapPositions.Markers.Add(new GMapMarkerHexaI(GPS_Position, Heading, CoG, Crosstrack));
                }
                else if (GmapFrameMode == 3 || GmapFrameMode == 4 || GmapFrameMode == 5)
                {
                    int ExpoValue = 0;
                    int AttitudePitch = ReadPitch / 10;
                    if (AttitudePitch >= 10 && AttitudePitch < 25) ExpoValue = 150;
                    if (AttitudePitch >= 25) ExpoValue = 50;
                    if (AttitudePitch <= -10 && AttitudePitch > -25) ExpoValue = -150;
                    if (AttitudePitch <= -25) ExpoValue = -50;
                    GmapPositions.Markers.Add(new GMapMarkerAero(GPS_Position, Heading, CoG, Crosstrack, ExpoValue));
                }
                if (ArmDisarm == 1) GMapTack.Points.Add(GPS_Position);
                MyGMap.Position = GPS_Position;
                MyGMap.Invalidate();
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
            WPLatVect1 = 0;
            WPLonVect1 = 0;
            WPLatVect2 = 0;
            WPLonVect2 = 0;
            WPLatVect3 = 0;
            WPLonVect3 = 0;
            WPLatVect4 = 0;
            WPLonVect4 = 0;
            WPLatVect5 = 0;
            WPLonVect5 = 0;
            WPLatVect6 = 0;
            WPLonVect6 = 0;
            WPLatVect7 = 0;
            WPLonVect7 = 0;
            WPLatVect8 = 0;
            WPLonVect8 = 0;
            WPLatVect9 = 0;
            WPLonVect9 = 0;
            WPLatVect10 = 0;
            WPLonVect10 = 0;
            GPSHoldTimed1 = 1;
            GPSHoldTimed2 = 1;
            GPSHoldTimed3 = 1;
            GPSHoldTimed4 = 1;
            GPSHoldTimed5 = 1;
            GPSHoldTimed6 = 1;
            GPSHoldTimed7 = 1;
            GPSHoldTimed8 = 1;
            GPSHoldTimed9 = 1;
            GPSHoldTimed10 = 1;
            CountToBlock = 0;
            BlockPushParams = false;
            comboBox1.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox12.SelectedIndex = 0;
            comboBox15.SelectedIndex = 0;
            comboBox17.SelectedIndex = 0;
            comboBox19.SelectedIndex = 0;
            comboBox21.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            comboBox11.SelectedIndex = 0;
            comboBox14.SelectedIndex = 0;
            comboBox16.SelectedIndex = 0;
            comboBox18.SelectedIndex = 0;
            comboBox20.SelectedIndex = 0;
            if (SerialPort.IsOpen == false) return;
            Serial_Write_To_FC(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;
            Serial_Write_To_FC(4);
        }

        public void SendWayPoints(SerialPort SerialPort)
        {

            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;

            if (SerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                WayPoint1Latitude = Convert.ToInt32(WPLatVect1 * 1e7);
                WayPoint1Longitude = Convert.ToInt32(WPLonVect1 * 1e7);
                //ENVIA O PRIMEIRO WAYPOINT SE DISPONIVEL
                SendBuffer[VectorPointer++] = (byte)0x4a;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x3c;
                SendBuffer[VectorPointer++] = 55;
                SendBuffer[VectorPointer++] = (byte)5;
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

                WayPoint2Latitude = Convert.ToInt32(WPLatVect2 * 1e7);
                WayPoint2Longitude = Convert.ToInt32(WPLonVect2 * 1e7);
                //ENVIA O SEGUNDO WAYPOINT SE DISPONIVEL
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

                WayPoint3Latitude = Convert.ToInt32(WPLatVect3 * 1e7);
                WayPoint3Longitude = Convert.ToInt32(WPLonVect3 * 1e7);
                //ENVIA O TERCEIRO WAYPOINT SE DISPONIVEL
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

                WayPoint4Latitude = Convert.ToInt32(WPLatVect4 * 1e7);
                WayPoint4Longitude = Convert.ToInt32(WPLonVect4 * 1e7);
                //ENVIA O QUARTO WAYPOINT SE DISPONIVEL
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

                WayPoint5Latitude = Convert.ToInt32(WPLatVect5 * 1e7);
                WayPoint5Longitude = Convert.ToInt32(WPLonVect5 * 1e7);
                //ENVIA O QUINTO WAYPOINT SE DISPONIVEL
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
                SerialPort.Write(SendBuffer, 0, VectorPointer);

                VectorPointer = 0;
                CheckAllBuffers = 0;

                WayPoint6Latitude = Convert.ToInt32(WPLatVect6 * 1e7);
                WayPoint6Longitude = Convert.ToInt32(WPLonVect6 * 1e7);
                //ENVIA O SEXTO WAYPOINT SE DISPONIVEL
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x3c;
                SendBuffer[VectorPointer++] = 55;
                SendBuffer[VectorPointer++] = (byte)6;
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

                WayPoint7Latitude = Convert.ToInt32(WPLatVect7 * 1e7);
                WayPoint7Longitude = Convert.ToInt32(WPLonVect7 * 1e7);
                //ENVIA O SETIMO WAYPOINT SE DISPONIVEL
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

                WayPoint8Latitude = Convert.ToInt32(WPLatVect8 * 1e7);
                WayPoint8Longitude = Convert.ToInt32(WPLonVect8 * 1e7);
                //ENVIA O OITAVO WAYPOINT SE DISPONIVEL
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

                WayPoint9Latitude = Convert.ToInt32(WPLatVect9 * 1e7);
                WayPoint9Longitude = Convert.ToInt32(WPLonVect9 * 1e7);
                //ENVIA O NONO WAYPOINT SE DISPONIVEL
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

                WayPoint10Latitude = Convert.ToInt32(WPLatVect10 * 1e7);
                WayPoint10Longitude = Convert.ToInt32(WPLonVect10 * 1e7);
                //ENVIA O NONO WAYPOINT SE DISPONIVEL
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
                SerialPort.Write(SendBuffer, 0, VectorPointer);
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

        private void MyGmap_OnMarkerEnter(GMapMarker Item)
        {
            if (!MouseDown)
            {
                if (Item is GMapMarkerRect)
                {
                    GMapMarkerRect MarkerRect = Item as GMapMarkerRect;
                    MarkerRect.Pen.Color = Color.Red;
                    MyGMap.Invalidate(false);
                    CurrentRectMarker = MarkerRect;
                }
            }
        }

        private void MyGmap_OnMarkerLeave(GMapMarker Item)
        {
            if (!MouseDown)
            {
                if (Item is GMapMarkerRect)
                {
                    CurrentRectMarker = null;
                    GMapMarkerRect MarkerRect = Item as GMapMarkerRect;
                    MarkerRect.Pen.Color = Color.White;
                    MyGMap.Invalidate(false);
                }
            }
        }

        private void MyGmap_MouseMove(object sender, MouseEventArgs e)
        {
            PointLatLng Point = MyGMap.FromLocalToLatLng(e.X, e.Y);
            CurrentMarker.Position = Point;
            if (ModifierKeys == Keys.Shift)
            {
                MyGMap.DragButton = MouseButtons.Left;
                return;
            }
            else MyGMap.DragButton = MouseButtons.Right;
            if (e.Button == MouseButtons.Left && MouseDown && ModifierKeys == Keys.None)
            {
                MouseDraging = true;
                if (CurrentRectMarker == null)
                {
                    double LatitudeDiff = Start.Lat - Point.Lat;
                    double LongitudeDiff = Start.Lng - Point.Lng;
                    MyGMap.Position = new PointLatLng(Center.Position.Lat + LatitudeDiff, Center.Position.Lng + LongitudeDiff);
                }
                else
                {
                    PointLatLng NewPoint = MyGMap.FromLocalToLatLng(e.X, e.Y);
                    if (CurrentMarker.IsVisible)
                    {
                        CurrentMarker.Position = NewPoint;
                    }
                    CurrentRectMarker.Position = NewPoint;

                    if (CurrentRectMarker.InnerMarker != null)
                    {
                        CurrentRectMarker.InnerMarker.Position = NewPoint;
                    }
                }
            }
        }

        private void MyGmap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            }
            if (MouseDown)
            {
                if (e.Button == MouseButtons.Left)
                {
                    MouseDown = false;
                }
                if (MouseDraging)
                {
                    if (CurrentRectMarker != null)
                    {
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "1")
                        {
                            label19.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label18.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect1 = CurrentMarker.Position.Lat;
                            WPLonVect1 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "2")
                        {
                            label20.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label16.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect2 = CurrentMarker.Position.Lat;
                            WPLonVect2 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "3")
                        {
                            label21.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label22.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect3 = CurrentMarker.Position.Lat;
                            WPLonVect3 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "4")
                        {
                            label37.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label35.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect4 = CurrentMarker.Position.Lat;
                            WPLonVect4 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "5")
                        {
                            label30.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label26.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect5 = CurrentMarker.Position.Lat;
                            WPLonVect5 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "6")
                        {
                            label28.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label24.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect6 = CurrentMarker.Position.Lat;
                            WPLonVect6 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "7")
                        {
                            label61.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label57.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect7 = CurrentMarker.Position.Lat;
                            WPLonVect7 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "8")
                        {
                            label70.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label63.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect8 = CurrentMarker.Position.Lat;
                            WPLonVect8 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "9")
                        {
                            label68.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label59.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect9 = CurrentMarker.Position.Lat;
                            WPLonVect9 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                        if (CurrentRectMarker.InnerMarker.Tag.ToString() == "10")
                        {
                            label66.Text = Convert.ToString(CurrentMarker.Position.Lat);
                            label55.Text = Convert.ToString(CurrentMarker.Position.Lng);
                            WPLatVect10 = CurrentMarker.Position.Lat;
                            WPLonVect10 = CurrentMarker.Position.Lng;
                            PrintArea = true;
                        }
                    }
                }
            }
            MouseDraging = false;
        }

        private void MyGmap_MouseDown(object sender, MouseEventArgs e)
        {
            Start = MyGMap.FromLocalToLatLng(e.X, e.Y);
            if (e.Button == MouseButtons.Left && ModifierKeys != Keys.Alt)
            {
                MouseDown = true;
                MouseDraging = false;
                if (CurrentMarker.IsVisible)
                {
                    CurrentMarker.Position = MyGMap.FromLocalToLatLng(e.X, e.Y);
                }
            }
        }

        private void limparMapaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GMapTack.Points.Clear();
        }

        private void DecolarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Você realmente deseja realizar uma decolagem automática?",
                      "Decolagem automática", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Serial_Write_To_FC(?);
            }
        }

        private void pousarAquiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Você realmente deseja realizar um pouso automático?",
                    "Pouso automático", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Serial_Write_To_FC(?);
            }
        }

        private void voeParaCáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Você realmente deseja voar para esse ponto?",
                                "Avanço automático de ponto", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Serial_Write_To_FC(?);
            }
        }

        private void tirarFotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Serial_Write_To_FC(?);
        }
    }
}

