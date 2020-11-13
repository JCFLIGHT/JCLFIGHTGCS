using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using ZedGraph;
using System.Runtime.InteropServices;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Globalization;

namespace JCFLIGHTGCS
{
    public partial class GCS : Form
    {
        SerialPort SerialPort = new SerialPort();
        string SerialComPort;
        string[] SerialPorts = SerialPort.GetPortNames();
        static bool Error_Received = false;
        static byte Read_State = 0;
        static byte OffSet = 0;
        static byte DataSize = 0;
        static byte CheckSum = 0;
        static byte Command;
        static byte[] InBuffer = new byte[300];
        static byte[] NumericConvert = new byte[15];

        int ThrottleData = 900;
        int YawData = 1000;
        int PitchData = 1000;
        int RollData = 1000;
        int Aux1Data = 1000;
        int Aux2Data = 1000;
        int Aux3Data = 1000;
        int Aux4Data = 1000;
        int Aux5Data = 1000;
        int Aux6Data = 1000;
        int Aux7Data = 1000;
        int Aux8Data = 1000;
        int ReadRoll = 2000;
        int ReadPitch = 0;
        int ReadCompass = 0;
        double ReadBarometer = 0;
        double BattVoltage = 0;
        double GPSLatPrev = 0;
        double GPSLonPrev = 0;
        double HDOP = 99.99;
        double Current = 0;
        double Watts = 0;
        double Declination = 00.00;
        double Temperature = 0;
        double HomePointDisctance;
        static double xTimeStamp = 0;
        byte CommandArmDisarm = 0;
        byte GPS_NumSat = 0;
        byte FailSafeDetect = 0;
        byte BattPercentage = 0;
        byte FlightMode = 0;
        byte FrameMode = 0;
        byte HomePointOK = 0;
        bool SmallCompass = false;
        bool SerialOpen = false;
        bool PidAndFiltersCommunicationOpen = false;
        bool AccNotCalibrated = false;
        string GPSLAT = "0";
        string GPSLONG = "0";
        string LatitudeHome = "0";
        string LongitudeHome = "0";

        static Scale aScale;
        static Scale bScale;
        static Scale cScale;
        static Scale dScale;
        static Scale eScale;
        static Scale fScale;

        GraphPane RollGraph;
        GraphPane PitchGraph;
        GraphPane CompassGraph;
        GraphPane BaroGraph;
        GraphPane TempGraph;
        GraphPane AccGraph;

        static RollingPointPairList RollToGraph;
        static RollingPointPairList PitchToGraph;
        static RollingPointPairList CompassToGraph;
        static RollingPointPairList BaroToGraph;
        static RollingPointPairList TempToGraph;
        static RollingPointPairList AccToGraph;

        GMapOverlay OverlayToHome = new GMapOverlay();

        static GMapOverlay Routes;
        static GMapRoute Grout;
        List<PointLatLng> Points = new List<PointLatLng>();
        PointLatLng GPS_Position;
        GMapOverlay PositionToRoutes;

        int CompassHealthCount = 9999999;
        int HeadingCompassPrev = 0;

        byte ComboBoxIOC = 0;
        byte ComboBoxAltHold = 0;
        byte ComboBoxGPSHold = 0;
        byte ComboBoxRTH = 0;
        byte ComboBoxPPM = 0;
        byte ComboBoxGimbal = 0;
        byte ComboBoxFrame = 0;
        byte ComboBoxParachute = 0;
        byte ComboBoxRthAltitude = 0;
        byte ComboBoxSPI = 0;
        byte ComboBoxUART2 = 0;
        byte ComboBoxCompass = 0;
        byte ComboBoxCompassRot = 0;
        byte ComboBoxAcro = 0;
        byte ComboBoxSport = 0;
        byte ComboBoxAutoFlip = 0;
        byte ComboBoxAuto = 0;
        byte ComboBoxArmDisarm = 0;
        byte ComboBoxGyroLPF = 0;
        byte ComboBoxKalmanState = 0;
        byte ComboBoxCompSpeed = 0;

        byte IOCDataGuard = 0;
        byte AltHoldGuard = 0;
        byte GPSHoldGuard = 0;
        byte RTHGuard = 0;
        byte PPMGuard = 0;
        byte GimbalGuard = 0;
        byte FrameGuard = 0;
        byte MotorSpeedGuard = 0;
        byte ParachuteGuard = 0;
        byte RthAltitudeGuard = 0;
        byte OptFlowGuard = 0;
        byte SonarGuard = 0;
        byte CompassGuard = 0;
        byte CompassRotGuard = 0;
        byte AcroGuard = 0;
        byte SportGuard = 0;
        byte AutoFlipGuard = 0;
        byte AutoGuard = 0;
        byte ArmDisarmGuard = 0;
        byte DevicesSum = 0;

        double AmperInMah = 0;

        int BreakPoint = 1000;

        byte TPAFactor = 0;
        byte GyroLPF = 0;
        int DerivativeLPF;
        int RCSmooth;
        byte KalmanState;
        int BiAccLPF;
        int BiGyroLPF;
        int BiAccNotch;
        int BiGyroNotch;
        byte CompSpeed;

        int CoG = 0;
        Int32 Crosstrack = 0;

        bool ItsSafeToUpdate = true;
        bool ToogleState = false;
        bool NoticeLarger = true;

        static int PacketsError = 0;
        static int PacketsReceived = 0;

        double GetAccGForce = 0;

        byte GetAccCalibFlag = 0;

        Form WaitUart = Program.WaitUart;

        public GCS()
        {
            InitializeComponent();
            SerialPort.DataBits = 8;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.Handshake = Handshake.None;
            SerialPort.DtrEnable = false;
            SerialPort.ReadBufferSize = 4096;
            foreach (string PortsName in SerialPorts) comboBox7.Items.Add(PortsName);
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);
            circularProgressBar1.Value = 0;
            circularProgressBar2.Value = 0;
            comboBox7.MouseWheel += new MouseEventHandler(comboBox7_MouseWheel);
            metroTrackBar1.MouseWheel += new MouseEventHandler(metroTrackBar1_MouseWheel);
        }

        private void GCS_Load(object sender, EventArgs e)
        {
            MyGMap.PolygonsEnabled = true;
            //MAPAS
            //MyGMap.MapProvider = GMapProviders.GoogleMap;
            MyGMap.MapProvider = GMapProviders.GoogleSatelliteMap;
            //MyGMap.MapProvider = GMapProviders.BingHybridMap;
            MyGMap.ShowCenter = false;
            MyGMap.Manager.Mode = AccessMode.ServerAndCache;
            MyGMap.Zoom = 2;
            MyGMap.DragButton = MouseButtons.Left;
            PositionToRoutes = new GMapOverlay("PositionToRoutes");
            MyGMap.Overlays.Add(PositionToRoutes);
            PositionToRoutes.Markers.Clear();
            Routes = new GMapOverlay("Routes");
            MyGMap.Overlays.Add(Routes);
            Pen PenRoute = new Pen(Color.Purple, 3);
            Grout = new GMapRoute(Points, "Track");
            Grout.Stroke = PenRoute;
            Routes.Routes.Add(Grout);
            metroTrackBar1.Value = 2;
            metroTrackBar1.Minimum = 2;
            metroTrackBar1.Maximum = 20;
            //PLOTTER ROLL
            RollGraph = zedGraphControl1.GraphPane;
            RollGraph.Title.Text = "ATTITUDE ROLL";
            RollGraph.XAxis.Title.Text = "";
            RollGraph.YAxis.Title.Text = "";
            RollGraph.XAxis.MajorGrid.IsVisible = true;
            RollGraph.YAxis.MajorGrid.IsVisible = true;
            RollGraph.XAxis.Scale.IsVisible = false;
            RollGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            RollGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            RollGraph.YAxis.MajorTic.IsOpposite = false;
            RollGraph.YAxis.MinorTic.IsOpposite = false;
            RollGraph.YAxis.MajorGrid.IsZeroLine = true;
            RollGraph.YAxis.Scale.Align = AlignP.Inside;
            RollGraph.YAxis.Scale.IsVisible = false;
            RollGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            RollGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            RollGraph.Legend.IsVisible = false;
            RollGraph.XAxis.Scale.IsVisible = false;
            RollGraph.YAxis.Scale.IsVisible = true;
            RollGraph.XAxis.Scale.MagAuto = true;
            RollGraph.YAxis.Scale.MagAuto = false;
            RollGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            RollGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            RollGraph.XAxis.Scale.Min = 0;
            RollGraph.XAxis.Scale.Max = 300;
            RollGraph.XAxis.Type = AxisType.Linear;
            RollToGraph = new RollingPointPairList(300);
            RollGraph.AddCurve("Attitude Roll", RollToGraph, Color.Blue, SymbolType.None);
            zedGraphControl1.ScrollGrace = 0;
            aScale = zedGraphControl1.GraphPane.XAxis.Scale;
            zedGraphControl1.AxisChange();
            //PLOTTER PITCH
            PitchGraph = zedGraphControl2.GraphPane;
            PitchGraph.Title.Text = "ATTITUDE PITCH";
            PitchGraph.XAxis.Title.Text = "";
            PitchGraph.YAxis.Title.Text = "";
            PitchGraph.XAxis.MajorGrid.IsVisible = true;
            PitchGraph.YAxis.MajorGrid.IsVisible = true;
            PitchGraph.XAxis.Scale.IsVisible = false;
            PitchGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            PitchGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            PitchGraph.YAxis.MajorTic.IsOpposite = false;
            PitchGraph.YAxis.MinorTic.IsOpposite = false;
            PitchGraph.YAxis.MajorGrid.IsZeroLine = true;
            PitchGraph.YAxis.Scale.Align = AlignP.Inside;
            PitchGraph.YAxis.Scale.IsVisible = false;
            PitchGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            PitchGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            PitchGraph.Legend.IsVisible = false;
            PitchGraph.XAxis.Scale.IsVisible = false;
            PitchGraph.YAxis.Scale.IsVisible = true;
            PitchGraph.XAxis.Scale.MagAuto = true;
            PitchGraph.YAxis.Scale.MagAuto = false;
            PitchGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            PitchGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            PitchGraph.XAxis.Scale.Min = 0;
            PitchGraph.XAxis.Scale.Max = 300;
            PitchGraph.XAxis.Type = AxisType.Linear;
            PitchToGraph = new RollingPointPairList(300);
            PitchGraph.AddCurve("Attitude Pitch", PitchToGraph, Color.Orange, SymbolType.None);
            zedGraphControl2.ScrollGrace = 0;
            bScale = zedGraphControl2.GraphPane.XAxis.Scale;
            zedGraphControl2.AxisChange();
            //PLOTTER COMPASS
            CompassGraph = zedGraphControl3.GraphPane;
            CompassGraph.Title.Text = "COMPASS (YAW)";
            CompassGraph.XAxis.Title.Text = "";
            CompassGraph.YAxis.Title.Text = "";
            CompassGraph.XAxis.MajorGrid.IsVisible = true;
            CompassGraph.YAxis.MajorGrid.IsVisible = true;
            CompassGraph.XAxis.Scale.IsVisible = false;
            CompassGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            CompassGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            CompassGraph.YAxis.MajorTic.IsOpposite = false;
            CompassGraph.YAxis.MinorTic.IsOpposite = false;
            CompassGraph.YAxis.MajorGrid.IsZeroLine = true;
            CompassGraph.YAxis.Scale.Align = AlignP.Inside;
            CompassGraph.YAxis.Scale.IsVisible = false;
            CompassGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            CompassGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            CompassGraph.Legend.IsVisible = false;
            CompassGraph.XAxis.Scale.IsVisible = false;
            CompassGraph.YAxis.Scale.IsVisible = true;
            CompassGraph.XAxis.Scale.MagAuto = true;
            CompassGraph.YAxis.Scale.MagAuto = false;
            CompassGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            CompassGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            CompassGraph.XAxis.Scale.Min = 0;
            CompassGraph.XAxis.Scale.Max = 300;
            CompassGraph.XAxis.Type = AxisType.Linear;
            CompassToGraph = new RollingPointPairList(300);
            CompassGraph.AddCurve("Compass", CompassToGraph, Color.Red, SymbolType.None);
            zedGraphControl3.ScrollGrace = 0;
            cScale = zedGraphControl3.GraphPane.XAxis.Scale;
            zedGraphControl3.AxisChange();
            //PLOTTER BARO
            BaroGraph = zedGraphControl4.GraphPane;
            BaroGraph.Title.Text = "BARÔMETRO";
            BaroGraph.XAxis.Title.Text = "";
            BaroGraph.YAxis.Title.Text = "";
            BaroGraph.XAxis.MajorGrid.IsVisible = true;
            BaroGraph.YAxis.MajorGrid.IsVisible = true;
            BaroGraph.XAxis.Scale.IsVisible = false;
            BaroGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            BaroGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            BaroGraph.YAxis.MajorTic.IsOpposite = false;
            BaroGraph.YAxis.MinorTic.IsOpposite = false;
            BaroGraph.YAxis.MajorGrid.IsZeroLine = true;
            BaroGraph.YAxis.Scale.Align = AlignP.Inside;
            BaroGraph.YAxis.Scale.IsVisible = false;
            BaroGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            BaroGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            BaroGraph.Legend.IsVisible = false;
            BaroGraph.XAxis.Scale.IsVisible = false;
            BaroGraph.YAxis.Scale.IsVisible = true;
            BaroGraph.XAxis.Scale.MagAuto = true;
            BaroGraph.YAxis.Scale.MagAuto = false;
            BaroGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            BaroGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            BaroGraph.XAxis.Scale.Min = 0;
            BaroGraph.XAxis.Scale.Max = 300;
            BaroGraph.XAxis.Type = AxisType.Linear;
            BaroToGraph = new RollingPointPairList(300);
            BaroGraph.AddCurve("Baro", BaroToGraph, Color.Green, SymbolType.None);
            zedGraphControl4.ScrollGrace = 0;
            dScale = zedGraphControl4.GraphPane.XAxis.Scale;
            zedGraphControl4.AxisChange();
            //PLOTTER TEMPERATURA
            TempGraph = zedGraphControl5.GraphPane;
            TempGraph.Title.Text = "TEMPERATURA °C";
            TempGraph.XAxis.Title.Text = "";
            TempGraph.YAxis.Title.Text = "";
            TempGraph.XAxis.MajorGrid.IsVisible = true;
            TempGraph.YAxis.MajorGrid.IsVisible = true;
            TempGraph.XAxis.Scale.IsVisible = false;
            TempGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            TempGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            TempGraph.YAxis.MajorTic.IsOpposite = false;
            TempGraph.YAxis.MinorTic.IsOpposite = false;
            TempGraph.YAxis.MajorGrid.IsZeroLine = true;
            TempGraph.YAxis.Scale.Align = AlignP.Inside;
            TempGraph.YAxis.Scale.IsVisible = false;
            TempGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            TempGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            TempGraph.Legend.IsVisible = false;
            TempGraph.XAxis.Scale.IsVisible = false;
            TempGraph.YAxis.Scale.IsVisible = true;
            TempGraph.XAxis.Scale.MagAuto = true;
            TempGraph.YAxis.Scale.MagAuto = false;
            TempGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            TempGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            TempGraph.XAxis.Scale.Min = 0;
            TempGraph.XAxis.Scale.Max = 300;
            TempGraph.XAxis.Type = AxisType.Linear;
            TempToGraph = new RollingPointPairList(300);
            TempGraph.AddCurve("Temperatura", TempToGraph, Color.Violet, SymbolType.None);
            zedGraphControl5.ScrollGrace = 0;
            eScale = zedGraphControl5.GraphPane.XAxis.Scale;
            zedGraphControl5.AxisChange();
            //PLOTTER DA FORÇA G NO ACELEROMETRO
            AccGraph = zedGraphControl6.GraphPane;
            AccGraph.Title.Text = "FORÇA G NO ACELERÔMETRO";
            AccGraph.XAxis.Title.Text = "";
            AccGraph.YAxis.Title.Text = "";
            AccGraph.XAxis.MajorGrid.IsVisible = true;
            AccGraph.YAxis.MajorGrid.IsVisible = true;
            AccGraph.XAxis.Scale.IsVisible = false;
            AccGraph.YAxis.Scale.FontSpec.FontColor = Color.White;
            AccGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            AccGraph.YAxis.MajorTic.IsOpposite = false;
            AccGraph.YAxis.MinorTic.IsOpposite = false;
            AccGraph.YAxis.MajorGrid.IsZeroLine = true;
            AccGraph.YAxis.Scale.Align = AlignP.Inside;
            AccGraph.YAxis.Scale.IsVisible = false;
            AccGraph.Chart.Fill = new Fill(Color.DimGray, Color.DarkGray, 45.0f);
            AccGraph.Fill = new Fill(Color.DimGray, Color.DimGray, 45.0f);
            AccGraph.Legend.IsVisible = false;
            AccGraph.XAxis.Scale.IsVisible = false;
            AccGraph.YAxis.Scale.IsVisible = true;
            AccGraph.XAxis.Scale.MagAuto = true;
            AccGraph.YAxis.Scale.MagAuto = false;
            AccGraph.YAxis.Title.FontSpec.FontColor = Color.White;
            AccGraph.XAxis.Title.FontSpec.FontColor = Color.White;
            AccGraph.XAxis.Scale.Min = 0;
            AccGraph.XAxis.Scale.Max = 300;
            AccGraph.XAxis.Type = AxisType.Linear;
            AccToGraph = new RollingPointPairList(300);
            AccGraph.AddCurve("Força G", AccToGraph, Color.Pink, SymbolType.None);
            zedGraphControl6.ScrollGrace = 0;
            fScale = zedGraphControl6.GraphPane.XAxis.Scale;
            zedGraphControl6.AxisChange();
            Thread.Sleep(3000);
            //FECHA O SPLASH SCREEN
            Program.Splash?.Close();
        }

        private void iconmaximizar_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                maximinizar.Visible = false;
                iconmaximizar.Visible = true;
                return;
            }
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size; //NÃO CUBRA A BARRA DE TAREFAS
            this.WindowState = FormWindowState.Maximized;
            maximinizar.Visible = true;
            iconmaximizar.Visible = true;
        }

        private void maximinizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            maximinizar.Visible = false;
            iconmaximizar.Visible = true;
        }

        private void iconminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void encerrar_Click_1(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SerialPort.Close();
            }
            Application.Exit();
            this.Close();
        }

        private void RealTimer_Tick(object sender, EventArgs e)
        {
            CheckCompassState(ReadCompass);
            Edit_Labels_To_Aero();
            UpdateDevices();
            DateTime DateTimeNow = DateTime.UtcNow;
            TimeZoneInfo HRBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            label72.Text = Convert.ToString(TimeZoneInfo.ConvertTimeFromUtc(DateTimeNow, HRBrasilia));
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
                                    PacketsReceived++;
                                    Serial_Parse(Command);
                                }
                            }
                            else
                            {
                                PacketsError++;
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

                case 7:
                    ptr = 0;
                    ReadPitch = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ReadRoll = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ReadCompass = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    DevicesSum = (byte)InBuffer[ptr++];
                    ThrottleData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    YawData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    PitchData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RollData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux1Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux2Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux3Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux4Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux5Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux6Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux7Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux8Data = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GPS_NumSat = (byte)InBuffer[ptr++];
                    GPSLAT = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    GPSLONG = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    LatitudeHome = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    LongitudeHome = Convert.ToString(BitConverter.ToInt32(InBuffer, ptr)); ptr += 4;
                    ReadBarometer = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr)) / 100; ptr += 4;
                    FailSafeDetect = (byte)InBuffer[ptr++];
                    BattVoltage = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    BattPercentage = (byte)InBuffer[ptr++];
                    CommandArmDisarm = (byte)InBuffer[ptr++];
                    HDOP = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    Current = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Watts = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Declination = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    FlightMode = (byte)InBuffer[ptr++];
                    FrameMode = (byte)InBuffer[ptr++];
                    HomePointOK = (byte)InBuffer[ptr++];
                    Temperature = (byte)InBuffer[ptr++];
                    HomePointDisctance = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    AmperInMah = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    CoG = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Crosstrack = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetAccGForce = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    GetAccCalibFlag = (byte)InBuffer[ptr++];
                    CompassX = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    CompassY = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    CompassZ = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    break;

                case 8:
                    ptr = 0;
                    FrameGuard = (byte)InBuffer[ptr++];
                    PPMGuard = (byte)InBuffer[ptr++];
                    GimbalGuard = (byte)InBuffer[ptr++];
                    ParachuteGuard = (byte)InBuffer[ptr++];
                    OptFlowGuard = (byte)InBuffer[ptr++];
                    SonarGuard = (byte)InBuffer[ptr++];
                    CompassGuard = (byte)InBuffer[ptr++];
                    CompassRotGuard = (byte)InBuffer[ptr++];
                    RthAltitudeGuard = (byte)InBuffer[ptr++];
                    MotorSpeedGuard = (byte)InBuffer[ptr++];
                    AcroGuard = (byte)InBuffer[ptr++];
                    AltHoldGuard = (byte)InBuffer[ptr++];
                    GPSHoldGuard = (byte)InBuffer[ptr++];
                    IOCDataGuard = (byte)InBuffer[ptr++];
                    RTHGuard = (byte)InBuffer[ptr++];
                    SportGuard = (byte)InBuffer[ptr++];
                    AutoFlipGuard = (byte)InBuffer[ptr++];
                    AutoGuard = (byte)InBuffer[ptr++];
                    ArmDisarmGuard = (byte)InBuffer[ptr++];
                    break;

                case 9:
                    ptr = 0;
                    TPAFactor = (byte)InBuffer[ptr++];
                    BreakPoint = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GyroLPF = (byte)InBuffer[ptr++];
                    DerivativeLPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RCSmooth = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    KalmanState = (byte)InBuffer[ptr++];
                    BiAccLPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    BiGyroLPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    BiAccNotch = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    BiGyroNotch = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    CompSpeed = (byte)InBuffer[ptr++];
                    NumericConvert[0] = (byte)InBuffer[ptr++];
                    NumericConvert[1] = (byte)InBuffer[ptr++];
                    NumericConvert[2] = (byte)InBuffer[ptr++];
                    NumericConvert[3] = (byte)InBuffer[ptr++];
                    NumericConvert[4] = (byte)InBuffer[ptr++];
                    NumericConvert[5] = (byte)InBuffer[ptr++];
                    NumericConvert[6] = (byte)InBuffer[ptr++];
                    NumericConvert[7] = (byte)InBuffer[ptr++];
                    NumericConvert[8] = (byte)InBuffer[ptr++];
                    NumericConvert[9] = (byte)InBuffer[ptr++];
                    NumericConvert[10] = (byte)InBuffer[ptr++];
                    NumericConvert[11] = (byte)InBuffer[ptr++];
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
            if (SerialPort.IsOpen == false) return;
            if (SerialPort.BytesToRead == 0)
            {
                if (ItsSafeToUpdate)
                {
                    Serial_Write_To_FC(7);
                    Serial_Write_To_FC(8);
                    Serial_Write_To_FC(9);
                    label69.Text = "GCS RSSI:" + Convert.ToString(CalculateAverage(PacketsReceived, PacketsError)) + "%";
                    UpdateAccImageStatus();
                }
            }
        }

        private static double CalculateAverage(int PR, int PE)
        {
            return PR / (PR + (double)PE) * 100;
        }

        private void ProgressBarControl(int CHThrottle, int CHYaw, int CHPitch, int CHRoll, int CHAux1, int CHAux2,
   int CHAux3, int CHAux4, int CHAux5, int CHAux6, int CHAux7, int CHAux8)
        {
            //CONTROLE DAS BARRAS DE PROGRESSO
            metroProgressBar1.Value = Convert.ToInt16(ValueConverterProgressBar(CHThrottle, 900, 2000, 0, 100));
            metroProgressBar2.Value = Convert.ToInt16(ValueConverterProgressBar(CHYaw, 1000, 2000, 0, 100));
            metroProgressBar3.Value = Convert.ToInt16(ValueConverterProgressBar(CHPitch, 1000, 2000, 0, 100));
            metroProgressBar4.Value = Convert.ToInt16(ValueConverterProgressBar(CHRoll, 1000, 2000, 0, 100));
            metroProgressBar5.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux1, 1000, 2000, 0, 100));
            metroProgressBar6.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux2, 1000, 2000, 0, 100));
            metroProgressBar7.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux3, 1000, 2000, 0, 100));
            metroProgressBar8.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux4, 1000, 2000, 0, 100));
            metroProgressBar9.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux5, 1000, 2000, 0, 100));
            metroProgressBar10.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux6, 1000, 2000, 0, 100));
            metroProgressBar11.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux7, 1000, 2000, 0, 100));
            metroProgressBar12.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux8, 1000, 2000, 0, 100));
            //LABEL'S
            label28.Text = Convert.ToString(CHThrottle);
            label29.Text = Convert.ToString(CHYaw);
            label30.Text = Convert.ToString(CHPitch);
            label31.Text = Convert.ToString(CHRoll);
            label32.Text = Convert.ToString(CHAux1);
            label33.Text = Convert.ToString(CHAux2);
            label34.Text = Convert.ToString(CHAux3);
            label35.Text = Convert.ToString(CHAux4);
            label36.Text = Convert.ToString(CHAux5);
            label37.Text = Convert.ToString(CHAux6);
            label38.Text = Convert.ToString(CHAux7);
            label38.Text = Convert.ToString(CHAux8);
        }

        long ValueConverterProgressBar(int x, int in_min, int in_max, int out_min, int out_max)
        {
            if (x <= 1000) return 0;
            if (x >= 2000) return 100;
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            label76.Text = Convert.ToString(GPS_NumSat);
            if (GPS_NumSat < 10) label76.Location = new Point(215, 78);
            else label76.Location = new Point(210, 78);
            if (HDOP >= 10)
            {
                label78.Location = new Point(197, 125);
                label78.Text = HDOP.ToString(new CultureInfo("en-US"));
            }
            else
            {
                label78.Location = new Point(200, 125);
                label78.Text = HDOP.ToString(new CultureInfo("en-US"));
            }
            if (ReadBarometer < 1000)
            {
                label79.Location = new Point(205, 165);
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "M";
            }
            else if (ReadBarometer >= 1000 && ReadBarometer <= 10000)
            {
                label79.Location = new Point(185, 165);
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "KM";
            }
            else
            {
                label79.Location = new Point(180, 165);
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "KM";
            }
            label87.Text = Current.ToString(new CultureInfo("en-US")) + "A";
            if (Current < 10) label87.Location = new Point(208, 355);
            if (Current >= 10 && Current < 100) label87.Location = new Point(200, 355);
            if (Current >= 100) label87.Location = new Point(195, 355);
            label116.Text = AmperInMah.ToString(new CultureInfo("en-US")) + "MAH";
            if (AmperInMah < 0.10f) label116.Location = new Point(200, 399);
            if (AmperInMah >= 0.10f && AmperInMah < 0.100f) label116.Location = new Point(195, 399);
            if (AmperInMah >= 0.100f && AmperInMah < 1) label116.Location = new Point(185, 399);
            if (AmperInMah >= 1 && AmperInMah < 10) label116.Location = new Point(175, 399);
            if (AmperInMah >= 10) label116.Location = new Point(165, 399);
            label89.Text = Watts.ToString(new CultureInfo("en-US")) + "W";
            if (Watts < 10) label89.Location = new Point(208, 444);
            if (Watts >= 10 && Watts < 100) label89.Location = new Point(195, 444);
            if (Watts >= 100) label89.Location = new Point(185, 444);
            if (Declination != 0) label81.Text = Declination.ToString(new CultureInfo("en-US")) + "°";
            if (Declination > 0 && Declination < 10) label81.Location = new Point(205, 207);
            if (Declination > (-10) && Declination < 0) label81.Location = new Point(205, 207);
            if (Declination >= 10 && Declination < 100) label81.Location = new Point(195, 207);
            if (Declination <= (-10) && Declination > (-100)) label81.Location = new Point(195, 207);
            if (Declination >= 100) label81.Location = new Point(190, 207);
            if (Declination <= (-100)) label81.Location = new Point(190, 207);
            FlightModeToLabel(FlightMode);
            if (ReadRoll > 1200)
            {
                AccNotCalibrated = true;
                RollToGraph.Add((double)xTimeStamp, 0);
            }
            else
            {
                AccNotCalibrated = false;
                RollToGraph.Add((double)xTimeStamp, ReadRoll);
            }
            PitchToGraph.Add((double)xTimeStamp, ReadPitch);
            CompassToGraph.Add((double)xTimeStamp, ReadCompass);
            BaroToGraph.Add((double)xTimeStamp, ReadBarometer);
            TempToGraph.Add((double)xTimeStamp, Temperature);
            AccToGraph.Add((double)xTimeStamp, GetAccGForce);

            xTimeStamp = xTimeStamp + 1;

            if (xTimeStamp > aScale.Max)
            {
                double range = aScale.Max - aScale.Min;
                aScale.Max = aScale.Max + 1;
                aScale.Min = aScale.Max - range;
            }

            if (xTimeStamp > bScale.Max)
            {
                double range = bScale.Max - bScale.Min;
                bScale.Max = bScale.Max + 1;
                bScale.Min = bScale.Max - range;
            }

            if (xTimeStamp > cScale.Max)
            {
                double range = cScale.Max - cScale.Min;
                cScale.Max = cScale.Max + 1;
                cScale.Min = cScale.Max - range;
            }

            if (xTimeStamp > dScale.Max)
            {
                double range = dScale.Max - dScale.Min;
                dScale.Max = dScale.Max + 1;
                dScale.Min = dScale.Max - range;
            }

            if (xTimeStamp > eScale.Max)
            {
                double range = eScale.Max - eScale.Min;
                eScale.Max = eScale.Max + 1;
                eScale.Min = eScale.Max - range;
            }

            if (xTimeStamp > fScale.Max)
            {
                double range = fScale.Max - fScale.Min;
                fScale.Max = fScale.Max + 1;
                fScale.Min = fScale.Max - range;
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();
            zedGraphControl3.AxisChange();
            zedGraphControl3.Invalidate();
            zedGraphControl4.AxisChange();
            zedGraphControl4.Invalidate();
            zedGraphControl5.AxisChange();
            zedGraphControl5.Invalidate();
            zedGraphControl6.AxisChange();
            zedGraphControl6.Invalidate();

            MyGMap.MinZoom = 2;
            MyGMap.MaxZoom = 24;
            MyGMap.Zoom = metroTrackBar1.Value;

            if (FailSafeDetect == 0)
            {
                if (ThrottleData > 975)
                {
                    label25.Location = new Point(15, 90);
                    label25.Text = "Conectado";
                }
                else
                {
                    label25.Location = new Point(4, 90);
                    label25.Text = "Desconectado";
                }
            }
            else
            {
                label25.Location = new Point(22, 92);
                label25.Text = "Fail-Safe";
            }

            if (GPS_NumSat < 5)
            {
                label19.Location = new Point(16, 90);
                label19.Text = "Sem Sinal";
            }
            else
            {
                label19.Location = new Point(10, 90);
                label19.Text = "Núm.Sats:" + Convert.ToString(GPS_NumSat);
            }
            ProgressBarControl(ThrottleData, YawData, PitchData, RollData, Aux1Data, Aux2Data, Aux3Data,
                   Aux4Data, Aux5Data, Aux6Data, Aux7Data, Aux8Data);
            if (ReadRoll > 1200) HorizonIndicator.SetAttitudeIndicatorParameters(ReadPitch / 10, 0);
            else HorizonIndicator.SetAttitudeIndicatorParameters(ReadPitch / 10, -ReadRoll / 10);
            HeadingIndicator.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);
            if (ReadRoll > 1200) HorizonIndicator2.SetAttitudeIndicatorParameters(-ReadPitch / 10, 0);
            else HorizonIndicator2.SetAttitudeIndicatorParameters(ReadPitch / 10, -ReadRoll / 10);
            HeadingIndicator2.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);
            circularProgressBar1.Text = Convert.ToString(BattVoltage);
            circularProgressBar2.Text = Convert.ToString(BattVoltage);
            BattPercentage = ConstrainByte(BattPercentage, 0, 100);
            circularProgressBar1.Value = BattPercentage;
            circularProgressBar2.Value = BattPercentage;
            label2.Text = Convert.ToString(BattPercentage + "%");
            label3.Text = Convert.ToString(BattPercentage + "%");
            if (SerialPort.IsOpen == true)
            {
                Program.WaitUart.Close();
                label4.Text = "Habilitado";
                label5.Text = "Habilitado";
                label4.Location = new Point(17, 90);
                label5.Location = new Point(19, 89);
            }
        }

        byte ConstrainByte(byte amt, byte low, byte high)
        {
            return ((amt) < (low) ? (low) : ((amt) > (high) ? (high) : (amt)));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;
            PacketsError = 0;
            PacketsReceived = 0;
            comboBox7.Enabled = true;
            comboBox7.Text = "Selecione";
            SerialPort.Close();
            if (SerialPort.IsOpen == false)
            {
                pictureBox9.Image = Properties.Resources.Desconectado;
                label4.Text = "Desabilitado";
                label5.Text = "Desabilitado";
                label4.Location = new Point(10, 90);
                label5.Location = new Point(12, 89);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            SerialComPort = Convert.ToString(comboBox7.SelectedItem);
            try
            {
                Program.WaitUart.Show();
                WaitUart.Refresh();
                SerialPort.PortName = SerialComPort;
                WaitUart.Refresh();
                SerialPort.BaudRate = 115200;
                WaitUart.Refresh();
                SerialPort.DataBits = 8;
                WaitUart.Refresh();
                SerialPort.Parity = Parity.None;
                WaitUart.Refresh();
                SerialPort.StopBits = StopBits.One;
                WaitUart.Refresh();
                SerialPort.Open();
                WaitUart.Refresh();

                for (Int32 i = 0; i < 300; i++)
                {
                    WaitUart.Refresh();
                    Thread.Sleep(10);
                }

                if (SerialPort.IsOpen == true)
                {
                    comboBox7.Enabled = false;
                    pictureBox9.Image = Properties.Resources.Conectado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conexão falhou! " + ex);
                Program.WaitUart.Close();
            }
        }

        private void comboBox7_Click(object sender, EventArgs e)
        {
            comboBox7.Items.Clear();
            SerialPorts = SerialPort.GetPortNames();
            foreach (string s in SerialPorts) comboBox7.Items.Add(s);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SmallCompass = false;
            NoticeLarger = true;
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            tabControl1.SelectTab(tabPage1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (SerialOpen) return;
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as configurações com a JCFLIGHT em Voo!");
                    return;
                }
                ItsSafeToUpdate = false;
                comboBox4.SelectedIndex = ((IOCDataGuard > comboBox4.Items.Count) ? 0 : IOCDataGuard);
                comboBox2.SelectedIndex = AltHoldGuard;
                comboBox3.SelectedIndex = GPSHoldGuard;
                comboBox5.SelectedIndex = RTHGuard;
                comboBox12.SelectedIndex = PPMGuard;
                comboBox13.SelectedIndex = GimbalGuard;
                comboBox11.SelectedIndex = FrameGuard;
                MotorSpeed.Value = MotorSpeedGuard;
                comboBox14.SelectedIndex = ParachuteGuard;
                comboBox1.SelectedIndex = AcroGuard;
                comboBox6.SelectedIndex = SportGuard;
                comboBox8.SelectedIndex = AutoFlipGuard;
                comboBox9.SelectedIndex = AutoGuard;
                comboBox15.SelectedIndex = OptFlowGuard;
                comboBox16.SelectedIndex = ((SonarGuard > comboBox16.Items.Count) ? 0 : SonarGuard);
                comboBox17.SelectedIndex = CompassGuard;
                comboBox18.SelectedIndex = CompassRotGuard;
                comboBox19.SelectedIndex = RthAltitudeGuard;
                comboBox10.SelectedIndex = ArmDisarmGuard;
                Serial_Write_To_FC(13);
                SerialOpen = true;
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (AccNotCalibrated)
            {
                pictureBox10.BackColor = Color.Red;
                pictureBox13.BackColor = Color.Red;
                pictureBox15.BackColor = Color.Red;
                pictureBox17.BackColor = Color.Red;
                pictureBox19.BackColor = Color.Red;
                pictureBox21.BackColor = Color.Red;
            }

            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            SmallCompass = false;
            tabControl1.SelectTab(tabPage3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            SmallCompass = false;
            tabControl1.SelectTab(tabPage4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            SmallCompass = false;
            tabControl1.SelectTab(tabPage5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NoticeLarger = false;
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            SmallCompass = true;
            tabControl1.SelectTab(tabPage6);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!SerialPort.IsOpen) return;
            Serial_Write_To_FC(11);
        }

        static int CompassX = 0;
        static int CompassY = 0;
        static int CompassZ = 0;

        public static int CompassRoll
        {
            get { return CompassX; }
        }

        public static int CompassPitch
        {
            get { return CompassY; }
        }

        public static int CompassYaw
        {
            get { return CompassZ; }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!SerialPort.IsOpen) return;
            Compass CompassOpen = new Compass();
            CompassOpen.Show();
            Serial_Write_To_FC(12);
        }

        private void GmapAtt_Tick(object sender, EventArgs e)
        {
            GPS_Position.Lat = double.Parse(GPSLAT) / 10000000.0f;
            GPS_Position.Lng = double.Parse(GPSLONG) / 10000000.0f;
            HomePointMarkerInMap(double.Parse(LatitudeHome), double.Parse(LongitudeHome));
            if (GPS_NumSat >= 5 && GPSLatPrev != GPS_Position.Lat &&
                GPSLonPrev != GPS_Position.Lng && GPS_Position.Lat != 0 && GPS_Position.Lng != 0)
            {
                PositionToRoutes.Markers.Clear();
                GPSLatPrev = GPS_Position.Lat;
                GPSLonPrev = GPS_Position.Lng;
                if (FrameMode == 0)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerQuad(GPS_Position, ReadCompass, CoG, Crosstrack));
                }
                else if (FrameMode == 1)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerHexaX(GPS_Position, ReadCompass, CoG, Crosstrack));
                }
                else if (FrameMode == 2)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerHexaI(GPS_Position, ReadCompass, CoG, Crosstrack));
                }
                else if (FrameMode == 3 || FrameMode == 4 || FrameMode == 5)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerAero(GPS_Position, ReadCompass, CoG, Crosstrack));
                }
                if (HomePointDisctance >= 1000 && HomePointDisctance < 10000)
                {
                    label74.Location = new Point(190, 32);
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else if (HomePointDisctance >= 10000 && HomePointDisctance < 100000)
                {
                    label74.Location = new Point(185, 32);
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else if (HomePointDisctance >= 100000)
                {
                    label74.Location = new Point(180, 32);
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else
                {
                    label74.Location = new Point(208, 32);
                    label74.Text = Convert.ToInt32(HomePointDisctance) + "M";
                }
                if (CommandArmDisarm == 1) Grout.Points.Add(GPS_Position);
                MyGMap.Position = GPS_Position;
                MyGMap.Invalidate();
            }
        }

        private void FlightModeToLabel(byte _FlightMode)
        {
            switch (_FlightMode)
            {
                case 0: //ACRO
                    label83.Location = new Point(188, 252);
                    label83.Text = "ACRO";
                    break;

                case 1: //STABILIZE
                    label83.Location = new Point(174, 252);
                    label83.Text = "STABILIZE";
                    break;

                case 2: //ALT-HOLD
                    label83.Location = new Point(175, 252);
                    label83.Text = "ALT-HOLD";
                    break;

                case 3: //ATAQUE              
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        label83.Location = new Point(179, 252);
                        label83.Text = "ATAQUE";
                    }
                    else
                    {
                        label83.Location = new Point(172, 252);
                        label83.Text = "TAKE-OFF";
                    }
                    break;

                case 4: //GPS-HOLD
                    label83.Location = new Point(175, 252);
                    label83.Text = "GPS-HOLD";
                    break;

                case 5: //IOC
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        label83.Location = new Point(202, 252);
                        label83.Text = "IOC";
                    }
                    else
                    {
                        label83.Location = new Point(184, 252);
                        label83.Text = "MANUAL";
                    }
                    break;

                case 6: //RTH
                case 7:
                    label83.Location = new Point(200, 252);
                    label83.Text = "RTH";
                    break;

                case 8: //LAND
                case 9:
                case 10:
                    label83.Location = new Point(192, 252);
                    label83.Text = "LAND";
                    break;

                case 11: //FLIP
                    label83.Location = new Point(198, 252);
                    label83.Text = "FLIP";
                    break;

                case 12: //AUTO
                    label83.Location = new Point(192, 252);
                    label83.Text = "AUTO";
                    break;

                case 13: //LANDED
                    label83.Location = new Point(165, 252);
                    label83.Text = "ATERRIZADO";
                    break;
            }
        }

        bool ResetTimer = false;
        byte Hour_Debug = 0;
        int Seconds_Count = 0;
        int Hour_Count = 0;
        private void FlightTimer_Tick(object sender, EventArgs e)
        {
            if (CommandArmDisarm == 1)
            {
                if (ResetTimer)
                {
                    Seconds_Count = 0;
                    Hour_Count = 0;
                    Hour_Debug = 0;
                    label85.Text = "00:00:00";
                    ResetTimer = false;
                }
                Seconds_Count++;
                label85.Text = ((Hour_Count).ToString("00.") + ":" + (Seconds_Count / 60).ToString("00.") + ":" + (Seconds_Count % 60).ToString("00."));
                if (Seconds_Count == 3600 && Hour_Debug == 0)
                {
                    Hour_Debug++;
                    Hour_Count += 1;
                    Seconds_Count = 0;
                }
            }
            else
            {
                ResetTimer = true;
            }
        }

        bool HomePointMarkerOK = false;
        private void HomePointMarkerInMap(double _Latitude, double _Longitutude)
        {
            _Latitude /= 10000000.0;
            _Longitutude /= 10000000.0;
            if (GPS_NumSat >= 5 && _Latitude != 0 && _Longitutude != 0)
            {
                if (HomePointOK == 1)
                {
                    if (!HomePointMarkerOK)
                    {
                        HomePointMarkerOK = true;
                        GMarkerGoogle HomePointMarker = new GMarkerGoogle(new PointLatLng(_Latitude, _Longitutude),
                        GMarkerGoogleType.yellow);
                        OverlayToHome.Markers.Add(HomePointMarker);
                        HomePointMarker.ToolTip = new GMapToolTip(HomePointMarker);
                        HomePointMarker.ToolTipMode = MarkerTooltipMode.Always;
                        HomePointMarker.ToolTipText = "HOME-POINT";
                        MyGMap.Overlays.Add(OverlayToHome);
                    }
                }
                else
                {
                    HomePointMarkerOK = false;
                    OverlayToHome.Markers.Clear();
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Grout.Points.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAcro = Convert.ToByte(comboBox1.SelectedIndex);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SendConfigurationsToJCFLIHGT(SerialPort, 1);
                Thread.Sleep(150);
                Serial_Write_To_FC(16);
                if (CompassGuard != comboBox17.SelectedIndex)
                {
                    MessageBox.Show("O modelo do Compass foi alterado,para que o mesmo funcione é necessario reiniciar o sistema!");
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (MessageBox.Show("Clicando em 'Sim' todas as configurações feitas aqui serão apagadas,você realmete deseja fazer isso?",
               "Limpar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(17);
                    comboBox4.SelectedIndex = 0;
                    comboBox2.SelectedIndex = 0;
                    comboBox3.SelectedIndex = 0;
                    comboBox5.SelectedIndex = 0;
                    comboBox12.SelectedIndex = 0;
                    comboBox13.SelectedIndex = 0;
                    comboBox11.SelectedIndex = 0;
                    MotorSpeed.Value = 0;
                    comboBox14.SelectedIndex = 0;
                    comboBox1.SelectedIndex = 0;
                    comboBox6.SelectedIndex = 0;
                    comboBox8.SelectedIndex = 0;
                    comboBox9.SelectedIndex = 0;
                    comboBox15.SelectedIndex = 0;
                    comboBox16.SelectedIndex = 0;
                    comboBox17.SelectedIndex = 0;
                    comboBox18.SelectedIndex = 0;
                    comboBox19.SelectedIndex = 0;
                    comboBox10.SelectedIndex = 0;
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIOC = Convert.ToByte(comboBox4.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAltHold = Convert.ToByte(comboBox2.SelectedIndex);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxGPSHold = Convert.ToByte(comboBox3.SelectedIndex);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxRTH = Convert.ToByte(comboBox5.SelectedIndex);
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxPPM = Convert.ToByte(comboBox12.SelectedIndex);
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxGimbal = Convert.ToByte(comboBox13.SelectedIndex);
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxFrame = Convert.ToByte(comboBox11.SelectedIndex);
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxParachute = Convert.ToByte(comboBox14.SelectedIndex);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxSport = Convert.ToByte(comboBox6.SelectedIndex);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAutoFlip = Convert.ToByte(comboBox8.SelectedIndex);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAuto = Convert.ToByte(comboBox9.SelectedIndex);
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxSPI = Convert.ToByte(comboBox15.SelectedIndex);
        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxUART2 = Convert.ToByte(comboBox16.SelectedIndex);
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxCompass = Convert.ToByte(comboBox17.SelectedIndex);
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxCompassRot = Convert.ToByte(comboBox18.SelectedIndex);
        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxRthAltitude = Convert.ToByte(comboBox19.SelectedIndex);
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxArmDisarm = Convert.ToByte(comboBox10.SelectedIndex);
        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxGyroLPF = Convert.ToByte(comboBox20.SelectedIndex);
        }

        private void comboBox21_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxKalmanState = Convert.ToByte(comboBox21.SelectedIndex);
        }

        private void comboBox22_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxCompSpeed = Convert.ToByte(comboBox22.SelectedIndex);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }
            ItsSafeToUpdate = true;

            if (PidAndFiltersCommunicationOpen == true)
            {
                Serial_Write_To_FC(14);
                PidAndFiltersCommunicationOpen = false;
            }

            SmallCompass = false;
            WayPoint WayPointOpen = new WayPoint();
            WayPointOpen.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (PidAndFiltersCommunicationOpen) return;
            if (SerialOpen == true)
            {
                Serial_Write_To_FC(14);
                SerialOpen = false;
            }

            ItsSafeToUpdate = false;

            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as configurações com a JCFLIGHT em Voo!");
                    return;
                }

                numericUpDown19.Value = BreakPoint;
                numericUpDown18.Value = TPAFactor;
                comboBox20.SelectedIndex = GyroLPF;
                numericUpDown14.Value = DerivativeLPF;
                numericUpDown20.Value = RCSmooth;
                comboBox21.SelectedIndex = KalmanState;
                numericUpDown13.Value = BiAccLPF;
                numericUpDown15.Value = BiGyroLPF;
                numericUpDown16.Value = BiAccNotch;
                numericUpDown17.Value = BiGyroNotch;
                comboBox22.SelectedIndex = CompSpeed;
                numericUpDown1.Value = (decimal)(NumericConvert[0]) / 10;
                numericUpDown2.Value = (decimal)(NumericConvert[1]) / 1000;
                numericUpDown3.Value = NumericConvert[2];
                numericUpDown4.Value = (decimal)(NumericConvert[3]) / 10;
                numericUpDown5.Value = (decimal)(NumericConvert[4]) / 1000;
                numericUpDown6.Value = NumericConvert[5];
                numericUpDown7.Value = (decimal)(NumericConvert[6]) / 10;
                numericUpDown8.Value = (decimal)(NumericConvert[7]) / 1000;
                numericUpDown9.Value = NumericConvert[8];
                numericUpDown10.Value = (decimal)(NumericConvert[9]) / 10;
                numericUpDown11.Value = (decimal)(NumericConvert[10]) / 100;
                numericUpDown12.Value = (decimal)(NumericConvert[11]) / 100;
                Serial_Write_To_FC(13);
                PidAndFiltersCommunicationOpen = true;
            }

            SmallCompass = false;
            tabControl1.SelectTab(tabPage7);
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            Manual ManualOpen = new Manual();
            ManualOpen.ShowDialog();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UC6kk7H1CiaPVv4iKVGS9GsA/featured");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("https://www.youtube.com/channel/UC6kk7H1CiaPVv4iKVGS9GsA/featured");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DerivativeLPF DerivativeLPFOpen = new DerivativeLPF();
            DerivativeLPFOpen.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RcControllerLPF RcControllerLPFOpen = new RcControllerLPF();
            RcControllerLPFOpen.ShowDialog();
        }

        private void CheckCompassState(int HeadingCompass)
        {
            //COMPASS
            if (HeadingCompass != HeadingCompassPrev)
            {
                CompassHealthCount = 0;
                HeadingCompassPrev = HeadingCompass;
            }
            else CompassHealthCount++;
            if (CompassHealthCount >= 4000) //4 SEGUNDOS
            {
                label95.Text = "Compass:Ruim";
                label95.ForeColor = Color.Red;
            }
            else
            {
                label95.Text = "Compass:Bom";
                label95.ForeColor = Color.Green;
            }
            //AHRS
            if (ReadRoll > 1200)
            {
                label94.Text = "AHRS:Ruim";
                label94.ForeColor = Color.Red;
            }
            else
            {
                label94.Text = "AHRS:Bom";
                label94.ForeColor = Color.Green;
            }
        }

        private void Edit_Labels_To_Aero()
        {
            if (ComboBoxFrame == 0 || ComboBoxFrame == 1 || ComboBoxFrame == 2 || ComboBoxFrame == 6 || ComboBoxFrame == 7) //QUAD & HEXA
            {
                label22.Text = "IOC";
                label44.Text = "> Controle de Orientação Inteligente";
                label23.Text = "Ataque";
                label46.Text = "> Modo Stabilize com Limite maior no Ângulo (55°)";
                if (groupBox6.Text == "VELOCIDADE INICIAL DO MOTOR AO ARMAR A JCFLIGHT")
                    groupBox6.Text = "VELOCIDADE INICIAL DOS MOTORES AO ARMAR A JCFLIGHT";
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
                comboBox7.Enabled = true;
                comboBox8.Enabled = true;
                comboBox9.Enabled = true;
                comboBox10.Enabled = true;
                comboBox19.Enabled = true;
                MotorSpeed.Enabled = true;
            }
            else if (ComboBoxFrame == 3 || ComboBoxFrame == 4 || ComboBoxFrame == 5) //AERO, ASA-FIXA & V-TAIL
            {
                label22.Text = "Manual";
                label44.Text = "> Servos independentes do controlador PID";
                label23.Text = "Auto-TakeOff";
                label46.Text = "> Lançamento Automático para Aeros e Asa";
                if (groupBox6.Text == "VELOCIDADE INICIAL DOS MOTORES AO ARMAR A JCFLIGHT")
                    groupBox6.Text = "VELOCIDADE INICIAL DO MOTOR AO ARMAR A JCFLIGHT";
                comboBox1.Enabled = true;
                comboBox2.Enabled = false;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
                comboBox7.Enabled = true;
                comboBox8.Enabled = false;
                comboBox9.Enabled = true;
                comboBox10.Enabled = true;
                comboBox19.Enabled = true;
                MotorSpeed.Enabled = true;
            }
            else if (ComboBoxFrame == 8) //FOGUETE
            {
                if (groupBox6.Text == "VELOCIDADE INICIAL DO MOTOR AO ARMAR A JCFLIGHT")
                    groupBox6.Text = "VELOCIDADE INICIAL DOS MOTORES AO ARMAR A JCFLIGHT";
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
                comboBox7.Enabled = false;
                comboBox8.Enabled = false;
                comboBox9.Enabled = false;
                comboBox10.Enabled = false;
                comboBox19.Enabled = false;
                MotorSpeed.Enabled = false;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            AirFrameOrientation AirFrameOrientationOpen = new AirFrameOrientation();
            AirFrameOrientationOpen.ShowDialog();
        }

        void comboBox7_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        void metroTrackBar1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SendConfigurationsToJCFLIHGT(SerialPort, 2);
                Thread.Sleep(150);
                Serial_Write_To_FC(19);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (MessageBox.Show("Clicando em 'Sim' todas as configurações feitas aqui serão apagadas,você realmete deseja fazer isso?",
               "Limpar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(20);
                    numericUpDown1.Value = (decimal)(35) / 10;
                    numericUpDown2.Value = (decimal)(35) / 1000;
                    numericUpDown3.Value = 26;
                    numericUpDown4.Value = (decimal)(35) / 10;
                    numericUpDown5.Value = (decimal)(35) / 1000;
                    numericUpDown6.Value = 26;
                    numericUpDown7.Value = (decimal)(69) / 10;
                    numericUpDown8.Value = (decimal)(50) / 1000;
                    numericUpDown9.Value = 0;
                    numericUpDown10.Value = (decimal)(50) / 10;
                    numericUpDown11.Value = (decimal)(100) / 100;
                    numericUpDown12.Value = (decimal)(90) / 100;
                    numericUpDown18.Value = 0;
                    numericUpDown19.Value = 1500;
                    comboBox20.SelectedIndex = 0;
                    numericUpDown14.Value = 40;
                    numericUpDown20.Value = 50;
                    comboBox21.SelectedIndex = 0;
                    numericUpDown13.Value = 15;
                    numericUpDown15.Value = 60;
                    numericUpDown16.Value = 0;
                    numericUpDown17.Value = 0;
                    comboBox22.SelectedIndex = 0;
                }
            }
        }

        void UpdateDevices()
        {

            if ((DevicesSum & 1) > 0)
            {
                label7.Location = new Point(18, 90);
                label7.Text = "Habilitado";
            }
            else
            {
                label7.Location = new Point(11, 90);
                label7.Text = "Desabilitado";
            }

            if ((DevicesSum & 2) > 0)
            {
                label14.Location = new Point(18, 90);
                label14.Text = "Habilitado";
            }
            else
            {
                label14.Location = new Point(11, 90);
                label14.Text = "Desabilitado";
            }

            if ((DevicesSum & 4) > 0)
            {
                label17.Location = new Point(18, 90);
                label17.Text = "Habilitado";
            }
            else
            {
                label17.Location = new Point(11, 90);
                label17.Text = "Desabilitado";
            }

            if ((DevicesSum & 8) > 0)
            {
                label11.Location = new Point(18, 90);
                label11.Text = "Habilitado";
            }
            else
            {
                label11.Location = new Point(11, 90);
                label11.Text = "Desabilitado";
            }

        }

        private void GCS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SerialPort.IsOpen) SerialPort.Close();
        }

        public void SendConfigurationsToJCFLIHGT(SerialPort SerialPort, byte ConfigType)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;

            if (SerialPort.IsOpen)
            {
                if (ConfigType == 1)
                {
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4a;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x3c;
                    SendBuffer[VectorPointer++] = 19;
                    SendBuffer[VectorPointer++] = (byte)15;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxFrame;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxPPM;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxGimbal;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxParachute;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSPI;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxUART2;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxCompass;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxCompassRot;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxRthAltitude;
                    SendBuffer[VectorPointer++] = (byte)MotorSpeed.Value;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAcro;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAltHold;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxGPSHold;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxIOC;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxRTH;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSport;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAutoFlip;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAuto;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxArmDisarm;
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    SerialPort.Write(SendBuffer, 0, VectorPointer);
                }
                else if (ConfigType == 2)
                {
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4a;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x3c;
                    SendBuffer[VectorPointer++] = 30;
                    SendBuffer[VectorPointer++] = (byte)18;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToByte(numericUpDown18.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown19.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown19.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)ComboBoxGyroLPF;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown14.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown14.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown20.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown20.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)ComboBoxKalmanState;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown13.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown13.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown15.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown15.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown16.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown16.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown17.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown17.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)ComboBoxCompSpeed;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown1.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown2.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown3.Value;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown4.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown5.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown6.Value;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown7.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown8.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown9.Value;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown10.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown11.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown12.Value * 100);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    SerialPort.Write(SendBuffer, 0, VectorPointer);
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;
            if (!ToogleState)
            {
                if (ReadRoll > 1200)
                {
                    HorizonIndicator.SetNoticeInArtificialHorizon(4, NoticeLarger, true);
                    HorizonIndicator2.SetNoticeInArtificialHorizon(4, NoticeLarger, true);
                }
                else
                {
                    if ((ReadRoll > 250 || ReadRoll < (-250)) || (ReadPitch > 250 || ReadPitch < (-250)))
                    {
                        HorizonIndicator.SetNoticeInArtificialHorizon(3, NoticeLarger, true);
                        HorizonIndicator2.SetNoticeInArtificialHorizon(3, NoticeLarger, true);
                    }
                    else
                    {
                        if (FailSafeDetect == 1)
                        {
                            HorizonIndicator.SetNoticeInArtificialHorizon(2, NoticeLarger, true);
                            HorizonIndicator2.SetNoticeInArtificialHorizon(2, NoticeLarger, true);
                        }
                        else
                        {
                            if (CommandArmDisarm == 0)
                            {
                                HorizonIndicator.SetNoticeInArtificialHorizon(0, NoticeLarger, true);
                                HorizonIndicator2.SetNoticeInArtificialHorizon(0, NoticeLarger, true);
                            }
                            else
                            {
                                HorizonIndicator.SetNoticeInArtificialHorizon(1, NoticeLarger, true);
                                HorizonIndicator2.SetNoticeInArtificialHorizon(1, NoticeLarger, true);
                            }
                        }
                    }
                }
                ToogleState = true;
            }
            else
            {
                if (ReadRoll > 1200)
                {
                    HorizonIndicator.SetNoticeInArtificialHorizon(4, NoticeLarger, false);
                    HorizonIndicator2.SetNoticeInArtificialHorizon(4, NoticeLarger, false);
                }
                else
                {
                    if ((ReadRoll > 250 || ReadRoll < (-250)) || (ReadPitch > 250 || ReadPitch < (-250)))
                    {
                        HorizonIndicator.SetNoticeInArtificialHorizon(3, NoticeLarger, false);
                        HorizonIndicator2.SetNoticeInArtificialHorizon(3, NoticeLarger, false);
                    }
                    else
                    {
                        if (FailSafeDetect == 1)
                        {
                            HorizonIndicator.SetNoticeInArtificialHorizon(2, NoticeLarger, false);
                            HorizonIndicator2.SetNoticeInArtificialHorizon(2, NoticeLarger, false);
                        }
                        else
                        {
                            if (CommandArmDisarm == 0)
                            {
                                HorizonIndicator.SetNoticeInArtificialHorizon(0, NoticeLarger, false);
                                HorizonIndicator2.SetNoticeInArtificialHorizon(0, NoticeLarger, false);
                            }
                            else
                            {
                                HorizonIndicator.SetNoticeInArtificialHorizon(1, NoticeLarger, false);
                                HorizonIndicator2.SetNoticeInArtificialHorizon(1, NoticeLarger, false);
                            }
                        }
                    }
                }
                ToogleState = false;
            }
        }

        private void UpdateAccImageStatus()
        {
            if ((1 & (GetAccCalibFlag >> 0)) > 0) pictureBox10.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 1)) > 0) pictureBox13.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 2)) > 0) pictureBox19.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 3)) > 0) pictureBox21.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 4)) > 0) pictureBox17.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 5)) > 0) pictureBox15.BackColor = Color.Green;
        }
    }
}