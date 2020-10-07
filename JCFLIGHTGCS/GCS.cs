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
        string SerialComPort, RxString;
        string[] JCFLIGHTData = null;
        string[] SerialPorts = SerialPort.GetPortNames();

        Int32 TelemetryPing = 0;
        Int64 Begin;
        Int64 Begin2;
        Int64 Begin3;
        Int16 ThrottleData = 900;
        Int16 YawData = 1000;
        Int16 PitchData = 1000;
        Int16 RollData = 1000;
        Int16 Aux1Data = 1000;
        Int16 Aux2Data = 1000;
        Int16 Aux3Data = 1000;
        Int16 Aux4Data = 1000;
        Int16 Aux5Data = 1000;
        Int16 Aux6Data = 1000;
        Int16 Aux7Data = 1000;
        Int16 Aux8Data = 1000;
        Int16 ReadRoll = 2000;
        Int16 ReadPitch = 0;
        Int16 ReadCompass = 0;
        Int32 SecondsCompass;
        double ReadBarometer = 0;
        double BattVoltage = 0;
        double GPSLatPrev = 0;
        double GPSLonPrev = 0;
        double HDOP = 99.99;
        double Current = 0;
        double Watts = 0;
        double Declination = 00.00;
        double Temperature = 0;
        double Dist;
        static double xTimeStamp = 0;
        byte CountAccImage = 0;
        byte CommandArmDisarm = 0;
        byte GPS_NumSat = 0;
        byte FailSafeDetect = 0;
        byte BattPercentage = 0;
        byte FlightMode = 0;
        byte FrameMode = 0;
        byte HomePointOK = 0;
        Boolean SmallCompass = false;
        Boolean RF24Open = false;
        Boolean RF24OpenPidAndFilters = false;
        Boolean AccNotCalibrated = false;
        string GPSLAT = "0";
        string GPSLONG = "0";
        string LatitudeHome = "0";
        string LongitudeHome = "0";

        static Scale aScale;
        static Scale bScale;
        static Scale cScale;
        static Scale dScale;
        static Scale eScale;

        GraphPane RollGraph;
        GraphPane PitchGraph;
        GraphPane CompassGraph;
        GraphPane BaroGraph;
        GraphPane TempGraph;

        static RollingPointPairList RollToGraph;
        static RollingPointPairList PitchToGraph;
        static RollingPointPairList CompassToGraph;
        static RollingPointPairList BaroToGraph;
        static RollingPointPairList TempToGraph;

        GMapOverlay OverlayToHome = new GMapOverlay();

        static GMapOverlay Routes;
        static GMapRoute Grout;
        List<PointLatLng> Points = new List<PointLatLng>();
        private List<PointLatLng> TwoPointsDist;
        private List<PointLatLng> DistanceValid;
        PointLatLng GPS_Position;
        GMapOverlay PositionToRoutes;

        const int KEY_CONFIG1 = 1;
        const int KEY_CONFIG2 = 2;
        const int KEY_CONFIG3 = 3;
        const int KEY_CONFIG4 = 4;
        const int KEY_CONFIG5 = 5;
        const int KEY_CONFIG6 = 6;
        const int KEY_CONFIG7 = 7;
        const int KEY_CONFIG8 = 8;
        const int KEY_CONFIG9 = 9;

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

        UInt16 BreakPointGuard = 1000;
        byte DynamicPIDGuard = 0;
        byte GyroLPFGuard = 0;
        byte DerivativeLPFGuard = 0;
        byte RCSmoothGuard = 0;
        byte KalmanStateGuard = 0;
        byte BiAccLPFGuard = 0;
        byte BiGyroLPFGuard = 0;
        byte BiAccNotchGuard = 0;
        byte BiGyroNotchGuard = 0;
        byte CompSpeedGuard = 0;

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
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            circularProgressBar1.Value = 0;
            circularProgressBar2.Value = 0;
            comboBox7.MouseWheel += new MouseEventHandler(comboBox7_MouseWheel);
            metroTrackBar1.MouseWheel += new MouseEventHandler(metroTrackBar1_MouseWheel);
        }

        private void GCS_Load(object sender, EventArgs e)
        {
            MyGMap.ShowCenter = false;
            MyGMap.Manager.Mode = AccessMode.ServerAndCache;
            MyGMap.Zoom = 5;
            //MAPAS
            //MyGMap.MapProvider = GMapProviders.GoogleMap;
            MyGMap.MapProvider = GMapProviders.GoogleSatelliteMap;
            //MyGMap.MapProvider = GMapProviders.BingHybridMap;
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
            CompassGraph.Title.Text = "COMPASS";
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
            Thread.Sleep(3000);
            //FECHA O SPLASH SCREEN
            Program.Splash?.Close();
        }

        private void iconmaximizar_Click(object sender, EventArgs e)
        {
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
                this.Close();
            }
            Application.Exit();
            this.Close();
        }

        private void RealTimer_Tick(object sender, EventArgs e)
        {
            CompassX = ReadRoll;
            CompassY = ReadPitch;
            CheckCompassState(ReadCompass);
            Edit_Labels_To_Aero();
            Calculate_Ping_Min_Max();
            label114.Text = "Ping Da Telemetria:" + TelemetryPing + "ms";
            DateTime DateTimeNow = DateTime.UtcNow;
            TimeZoneInfo HRBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            label72.Text = Convert.ToString(TimeZoneInfo.ConvertTimeFromUtc(DateTimeNow, HRBrasilia));
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (SerialPort.IsOpen == false) return;
                RxString = SerialPort.ReadLine();
                JCFLIGHTData = RxString.Split(',');
                if (RxString == "") return;
                if (RxString == "Debug Serial Monitor Para JCFlightGCS\r") return;
                if (RxString == "POR FAVOR,EXECUTE A TELEMETRIA NO PROGRAMA JCFLIGHTGCS\r") return;
                if (JCFLIGHTData[2] == "\r" || JCFLIGHTData[35] == "\r" || JCFLIGHTData[60] == "\r") return;
                if (JCFLIGHTData[2] == null || JCFLIGHTData[35] == null || JCFLIGHTData[60] == null) return;
                if (JCFLIGHTData[2] == "0" || JCFLIGHTData[35] == "0" || JCFLIGHTData[60] == "0") return;
                Begin = Convert.ToInt32(JCFLIGHTData[2]);
                Begin2 = Convert.ToInt32(JCFLIGHTData[35]);
                Begin3 = Convert.ToInt32(JCFLIGHTData[60]);
                if (Begin == 4 && Begin2 == 22 && Begin3 == 44)
                {
                    IOCDataGuard = Convert.ToByte(JCFLIGHTData[15]);
                    AltHoldGuard = Convert.ToByte(JCFLIGHTData[29]);
                    GPSHoldGuard = Convert.ToByte(JCFLIGHTData[30]);
                    RTHGuard = Convert.ToByte(JCFLIGHTData[31]);
                    PPMGuard = Convert.ToByte(JCFLIGHTData[32]);
                    GimbalGuard = Convert.ToByte(JCFLIGHTData[36]);
                    FrameMode = FrameGuard = Convert.ToByte(JCFLIGHTData[37]);
                    MotorSpeedGuard = Convert.ToByte(JCFLIGHTData[39]);
                    ParachuteGuard = Convert.ToByte(JCFLIGHTData[40]);
                    RthAltitudeGuard = Convert.ToByte(JCFLIGHTData[41]);
                    OptFlowGuard = Convert.ToByte(JCFLIGHTData[61]);
                    SonarGuard = Convert.ToByte(JCFLIGHTData[7]);
                    CompassGuard = Convert.ToByte(JCFLIGHTData[8]);
                    CompassRotGuard = Convert.ToByte(JCFLIGHTData[11]);
                    AcroGuard = Convert.ToByte(JCFLIGHTData[33]);
                    SportGuard = Convert.ToByte(JCFLIGHTData[34]);
                    AutoFlipGuard = Convert.ToByte(JCFLIGHTData[44]);
                    AutoGuard = Convert.ToByte(JCFLIGHTData[45]);
                    ArmDisarmGuard = Convert.ToByte(JCFLIGHTData[49]);
                    ThrottleData = Convert.ToInt16(JCFLIGHTData[16]);
                    YawData = Convert.ToInt16(JCFLIGHTData[17]);
                    PitchData = Convert.ToInt16(JCFLIGHTData[18]);
                    RollData = Convert.ToInt16(JCFLIGHTData[19]);
                    Aux1Data = Convert.ToInt16(JCFLIGHTData[20]);
                    Aux2Data = Convert.ToInt16(JCFLIGHTData[21]);
                    Aux3Data = Convert.ToInt16(JCFLIGHTData[22]);
                    Aux4Data = Convert.ToInt16(JCFLIGHTData[28]);
                    Aux5Data = Convert.ToInt16(JCFLIGHTData[12]);
                    Aux6Data = Convert.ToInt16(JCFLIGHTData[46]);
                    Aux7Data = Convert.ToInt16(JCFLIGHTData[47]);
                    Aux8Data = Convert.ToInt16(JCFLIGHTData[48]);
                    GPS_NumSat = Convert.ToByte(JCFLIGHTData[4]);
                    ReadPitch = Convert.ToInt16(JCFLIGHTData[0]);
                    ReadRoll = Convert.ToInt16(JCFLIGHTData[1]);
                    ReadCompass = Convert.ToInt16(JCFLIGHTData[9]);
                    ReadBarometer = Convert.ToDouble(JCFLIGHTData[10]) / 100;
                    FailSafeDetect = Convert.ToByte(JCFLIGHTData[14]);
                    BattVoltage = Convert.ToDouble(JCFLIGHTData[3]) / 100;
                    BattPercentage = Convert.ToByte(JCFLIGHTData[43]);
                    CommandArmDisarm = Convert.ToByte(JCFLIGHTData[13]);
                    GPSLAT = Convert.ToString(JCFLIGHTData[23]);
                    GPSLONG = Convert.ToString(JCFLIGHTData[24]);
                    HDOP = Convert.ToDouble(JCFLIGHTData[42]) / 100;
                    Current = Convert.ToDouble(JCFLIGHTData[62]) / 100;
                    Watts = Convert.ToDouble(JCFLIGHTData[63]) / 100;
                    Declination = Convert.ToDouble(JCFLIGHTData[38]) / 100;
                    FlightMode = Convert.ToByte(JCFLIGHTData[5]);
                    LatitudeHome = Convert.ToString(JCFLIGHTData[26]);
                    LongitudeHome = Convert.ToString(JCFLIGHTData[27]);
                    HomePointOK = Convert.ToByte(JCFLIGHTData[25]);
                    Temperature = Convert.ToByte(JCFLIGHTData[6]);
                    TelemetryPing = Convert.ToInt32(JCFLIGHTData[50]);
                    BreakPointGuard = Convert.ToUInt16(JCFLIGHTData[51]);
                    DynamicPIDGuard = Convert.ToByte(JCFLIGHTData[52]);
                    GyroLPFGuard = Convert.ToByte(JCFLIGHTData[53]);
                    DerivativeLPFGuard = Convert.ToByte(JCFLIGHTData[54]);
                    RCSmoothGuard = Convert.ToByte(JCFLIGHTData[55]);
                    KalmanStateGuard = Convert.ToByte(JCFLIGHTData[56]);
                    BiAccLPFGuard = Convert.ToByte(JCFLIGHTData[57]);
                    BiGyroLPFGuard = Convert.ToByte(JCFLIGHTData[58]);
                    BiAccNotchGuard = Convert.ToByte(JCFLIGHTData[64]);
                    BiGyroNotchGuard = Convert.ToByte(JCFLIGHTData[65]);
                    CompSpeedGuard = Convert.ToByte(JCFLIGHTData[66]);
                }
            }
        }

        private void ProgressBarControl(Int16 CHThrottle, Int16 CHYaw, Int16 CHPitch, Int16 CHRoll, Int16 CHAux1, Int16 CHAux2,
       Int16 CHAux3, Int16 CHAux4, Int16 CHAux5, Int16 CHAux6, Int16 CHAux7, Int16 CHAux8)
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

        long ValueConverterProgressBar(Int16 x, Int16 in_min, Int16 in_max, Int16 out_min, Int16 out_max)
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
            if (ReadBarometer <= 1000)
            {
                label79.Location = new Point(205, 165);
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "M";
            }
            else if (ReadBarometer <= 10000)
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
            if (Current > 10) label87.Location = new Point(200, 355);
            if (Current > 100) label87.Location = new Point(195, 355);
            else label87.Location = new Point(208, 355);
            label89.Text = Watts.ToString(new CultureInfo("en-US")) + "W";
            if (Watts > 10) label89.Location = new Point(200, 403);
            if (Watts > 100) label89.Location = new Point(195, 403);
            else label89.Location = new Point(208, 403);
            if (Declination != 0) label81.Text = Declination.ToString(new CultureInfo("en-US")) + "°";
            if (Declination >= 10 && Declination < 100) label81.Location = new Point(208, 207);
            if (Declination > 0 && Declination < 10) label81.Location = new Point(213, 207);

            if (Declination > (-10) && Declination < 0) label81.Location = new Point(213, 207);

            if (Declination <= (-10) && Declination > (-100)) label81.Location = new Point(194, 207);

            if (Declination >= 100) label81.Location = new Point(202, 207);
            if (Declination <= (-100)) label81.Location = new Point(198, 207);
            FlightModeToLabel(FlightMode);
            if (ReadRoll > 1000)
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
            if (ReadRoll > 1000) HorizonIndicator.SetAttitudeIndicatorParameters(-ReadPitch / 10, 0);
            else HorizonIndicator.SetAttitudeIndicatorParameters(ReadPitch / 10, -ReadRoll / 10);
            HeadingIndicator.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);
            if (ReadRoll > 1000) HorizonIndicator2.SetAttitudeIndicatorParameters(ReadPitch / 10, 0);
            else HorizonIndicator2.SetAttitudeIndicatorParameters(ReadPitch / 10, -ReadRoll / 10);
            HeadingIndicator2.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);
            circularProgressBar1.Text = Convert.ToString(BattVoltage);
            circularProgressBar1.Value = BattPercentage;
            circularProgressBar2.Text = Convert.ToString(BattVoltage);
            circularProgressBar2.Value = BattPercentage;
            label2.Text = Convert.ToString(BattPercentage + "%");
            label3.Text = Convert.ToString(BattPercentage + "%");
            if (Begin == 4 && Begin2 == 22 && Begin3 == 44)
            {
                label4.Text = "Habilitado";
                label5.Text = "Habilitado";
                label7.Text = "Habilitado";
                label4.Location = new Point(17, 90);
                label5.Location = new Point(19, 89);
                label7.Location = new Point(18, 90);
            }
            if (SerialPort.IsOpen == true)
            {
                if (Begin == 4 && Begin2 == 22 && Begin3 == 44)
                {
                    Program.WaitUart.Close();
                }
                else
                {
                    WaitUart.Refresh();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == false) return;

            comboBox7.Enabled = true;
            comboBox7.Text = "Selecione";
            SerialPort.Close();

            serialPort1.PortName = SerialComPort;
            serialPort1.BaudRate = 115200;
            serialPort1.Open();
            for (Int32 i = 0; i < 51; i++)
            {
                serialPort1.WriteLine("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
                Thread.Sleep(1);
            }
            serialPort1.Close();

            if (SerialPort.IsOpen == false)
            {
                pictureBox9.Image = Properties.Resources.Desconectado;
                Begin = Begin2 = Begin3 = 0;
                label4.Text = "Desabilitado";
                label5.Text = "Desabilitado";
                label7.Text = "Desabilitado";
                label4.Location = new Point(10, 90);
                label5.Location = new Point(12, 89);
                label7.Location = new Point(11, 90);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean Ignore = false;
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
                for (Int32 i = 0; i < 100; i++)
                {
                    WaitUart.Refresh();
                    if (Ignore == false)
                    {
                        SerialPort.WriteLine("vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv");
                        Thread.Sleep(1);
                    }
                    if (i == 99)
                    {
                        Ignore = true;
                        SerialPort.WriteLine("RUN LOOP TELEMETRY");
                    }
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
            CountAccImage = 0;
            SmallCompass = false;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            tabControl1.SelectTab(tabPage1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;
            OpenConfigLoad ConfigLoadOpen = new OpenConfigLoad();
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as Configurações com JCFLIGHT em Voo.");
                    return;
                }

                ConfigLoadOpen.Show();

                if (IOCDataGuard > comboBox4.Items.Count) IOCDataGuard = Convert.ToByte(comboBox4.Items.Count);
                if (AltHoldGuard > comboBox2.Items.Count) AltHoldGuard = Convert.ToByte(comboBox2.Items.Count);
                if (GPSHoldGuard > comboBox3.Items.Count) GPSHoldGuard = Convert.ToByte(comboBox3.Items.Count);
                if (RTHGuard > comboBox5.Items.Count) RTHGuard = Convert.ToByte(comboBox5.Items.Count);
                if (PPMGuard > comboBox12.Items.Count) PPMGuard = Convert.ToByte(comboBox12.Items.Count);
                if (GimbalGuard > comboBox13.Items.Count) GimbalGuard = Convert.ToByte(comboBox13.Items.Count);
                if (FrameGuard > comboBox11.Items.Count) FrameGuard = Convert.ToByte(comboBox11.Items.Count);
                if (MotorSpeedGuard > MotorSpeed.Maximum) MotorSpeedGuard = Convert.ToByte(MotorSpeed.Maximum);
                if (ParachuteGuard > comboBox14.Items.Count) ParachuteGuard = Convert.ToByte(comboBox14.Items.Count);
                if (AcroGuard > comboBox1.Items.Count) AcroGuard = Convert.ToByte(comboBox1.Items.Count);
                if (SportGuard > comboBox6.Items.Count) SportGuard = Convert.ToByte(comboBox6.Items.Count);
                if (AutoFlipGuard > comboBox8.Items.Count) AutoFlipGuard = Convert.ToByte(comboBox8.Items.Count);
                if (AutoGuard > comboBox9.Items.Count) AutoGuard = Convert.ToByte(comboBox9.Items.Count);
                if (OptFlowGuard > comboBox15.Items.Count) OptFlowGuard = Convert.ToByte(comboBox15.Items.Count);
                if (SonarGuard > comboBox16.Items.Count) SonarGuard = Convert.ToByte(comboBox16.Items.Count);
                if (CompassGuard > comboBox17.Items.Count) CompassGuard = Convert.ToByte(comboBox17.Items.Count);
                if (CompassRotGuard > comboBox18.Items.Count) CompassRotGuard = Convert.ToByte(comboBox18.Items.Count);
                if (RthAltitudeGuard > comboBox19.Items.Count) RthAltitudeGuard = Convert.ToByte(comboBox19.Items.Count);
                if (ArmDisarmGuard > comboBox10.Items.Count) ArmDisarmGuard = Convert.ToByte(comboBox10.Items.Count);

                comboBox4.SelectedIndex = IOCDataGuard;
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
                comboBox16.SelectedIndex = SonarGuard;
                comboBox17.SelectedIndex = CompassGuard;
                comboBox18.SelectedIndex = CompassRotGuard;
                comboBox19.SelectedIndex = RthAltitudeGuard;
                comboBox10.SelectedIndex = ArmDisarmGuard;

                for (int i = 0; i < 300; i++)
                {
                    ConfigLoadOpen.Refresh();
                    OpenSendConfiguration(SerialPort);
                    Thread.Sleep(1);
                }

                RF24Open = true;
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage2);
            ConfigLoadOpen.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;

            if (AccNotCalibrated)
            {
                pictureBox10.BackColor = Color.Red;
                pictureBox13.BackColor = Color.Red;
                pictureBox15.BackColor = Color.Red;
                pictureBox17.BackColor = Color.Red;
                pictureBox19.BackColor = Color.Red;
                pictureBox21.BackColor = Color.Red;
            }

            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            SmallCompass = false;
            tabControl1.SelectTab(tabPage3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            SmallCompass = false;
            tabControl1.SelectTab(tabPage4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            SmallCompass = false;
            tabControl1.SelectTab(tabPage5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            SmallCompass = true;
            tabControl1.SelectTab(tabPage6);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            /* if ((ReadPitch > 500 && ReadRoll > 500) ||
                 (ReadPitch < (-500) && ReadRoll < (-500)) ||
                 (ReadPitch > 500 && ReadRoll < (-500)))
             {
                 MessageBox.Show("IMU muito inclinada,refaça novamente!");
                 return;
             }*/

            if (SerialPort.IsOpen == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    SendCalibration(SerialPort, 98); //ACELEROMETRO
                    Thread.Sleep(1);
                }
                CountAccImage++;
                pictureBox10.BackColor = Color.Green;
                if (CountAccImage > 1) pictureBox13.BackColor = Color.Green;
                if (CountAccImage > 2)
                {
                    if (ReadPitch > 750) pictureBox21.BackColor = Color.Green;
                    if (ReadPitch < (-750)) pictureBox19.BackColor = Color.Green;
                    if (ReadRoll > 750) pictureBox17.BackColor = Color.Green;
                    if (ReadRoll < (-750)) pictureBox15.BackColor = Color.Green;
                }
            }
        }

        static Int16 CompassX = 0;
        static Int16 CompassY = 0;

        public static Int16 CompassRoll
        {
            get { return CompassX; }
        }

        public static Int16 CompassPitch
        {
            get { return CompassY; }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    SendCalibration(SerialPort, 100); //COMPASS
                    Thread.Sleep(1);
                }
                Compass CompassOpen = new Compass();
                CompassOpen.Show();
                if (CompassCalib.Enabled == false) CompassCalib.Enabled = true;
            }
        }

        private void GmapAtt_Tick(object sender, EventArgs e)
        {
            DistanceValid = new List<PointLatLng>();
            GPS_Position.Lat = double.Parse(GPSLAT) / 10000000;
            GPS_Position.Lng = double.Parse(GPSLONG) / 10000000;
            HomePointMarkerInMap(double.Parse(LatitudeHome), double.Parse(LongitudeHome));
            if (GPS_NumSat >= 5 && GPS_Position.Lat != 0 && GPS_Position.Lng != 0 &&
                GPSLatPrev != GPS_Position.Lat && GPSLonPrev != GPS_Position.Lng)
            {
                if (HomePointOK == 1)
                {
                    TwoPointsDist = new List<PointLatLng>();
                    TwoPointsDist.Add(new PointLatLng(Convert.ToDouble(LatitudeHome) / 10000000, Convert.ToDouble(LongitudeHome) / 10000000));
                    TwoPointsDist.Add(new PointLatLng(Convert.ToDouble(GPS_Position.Lat), Convert.ToDouble(GPS_Position.Lng)));
                    Dist = MyGMap.MapProvider.Projection.GetDistance(TwoPointsDist[0], TwoPointsDist[1]) * 1000;
                    double DistFloat = Convert.ToDouble(Convert.ToInt32(Dist)) / 1000;
                    if (Dist >= 1000)
                    {
                        label74.Location = new Point(190, 32);
                        label74.Text = DistFloat.ToString(new CultureInfo("en-US")) + "KM";
                    }
                    else if (Dist >= 10000)
                    {
                        label74.Location = new Point(185, 32);
                        label74.Text = DistFloat.ToString(new CultureInfo("en-US")) + "KM";
                    }
                    else if (Dist >= 100000)
                    {
                        label74.Location = new Point(180, 32);
                        label74.Text = DistFloat.ToString(new CultureInfo("en-US")) + "KM";
                    }
                    else
                    {
                        label74.Location = new Point(208, 32);
                        label74.Text = Convert.ToInt32(Dist) + "M";
                    }
                }

                DistanceValid.Add(new PointLatLng(Convert.ToDouble(GPSLatPrev), Convert.ToDouble(GPSLonPrev)));
                DistanceValid.Add(new PointLatLng(Convert.ToDouble(GPS_Position.Lat), Convert.ToDouble(GPS_Position.Lng)));
                double Distance = MyGMap.MapProvider.Projection.GetDistance(DistanceValid[0], DistanceValid[1]) * 1000;
                double Dist1Float = Convert.ToDouble(Convert.ToInt32(Distance));
                if (Dist1Float <= 1) return; //DISTANCIA ENTRE O PONTO ANTERIOR E O ATUAL É MENOR QUE 1M?SIM...SAIA DA FUNÇÃO

                PositionToRoutes.Markers.Clear();
                GPSLatPrev = GPS_Position.Lat;
                GPSLonPrev = GPS_Position.Lng;
                if (FrameMode == 0)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerQuad(GPS_Position, ReadCompass, 0, 0));
                }
                else if (FrameMode == 3 || FrameMode == 4 || FrameMode == 5)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerAero(GPS_Position, ReadCompass, 0, 0));
                }
                else if (FrameMode == 1)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerHexaX(GPS_Position, ReadCompass, 0, 0));
                }
                else if (FrameMode == 2)
                {
                    PositionToRoutes.Markers.Add(new GMapMarkerHexaI(GPS_Position, ReadCompass, 0, 0));
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

                case 3: //SPORT
                    label83.Location = new Point(177, 252);
                    label83.Text = "ESPORTE";
                    break;

                case 4: //GPS-HOLD
                    label83.Location = new Point(175, 252);
                    label83.Text = "GPS-HOLD";
                    break;

                case 5: //IOC
                    label83.Location = new Point(202, 252);
                    label83.Text = "IOC";
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

        Boolean ResetTimer = false;
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

        Boolean HomePointMarkerOK = false;
        private void HomePointMarkerInMap(double _Latitude, double _Longitutude)
        {
            _Latitude /= 10000000.0;
            _Longitutude /= 10000000.0;
            if (GPS_NumSat >= 5)
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
            SendConfigLoad SendConfigLoadOpen = new SendConfigLoad();
            if (SerialPort.IsOpen == true)
            {
                SendConfigLoadOpen.Show();

                for (int i = 0; i < 300; i++)
                {
                    SendConfigLoadOpen.Refresh();
                    SendConfiguration(SerialPort);
                    Thread.Sleep(1);
                }

                SendConfigLoadOpen.Close();

                if (MessageBox.Show("É necessario reiciar a JCFLIGHT para aplicar as alterações,deseja reiniciar automaticamete?",
                    "Reboot Automático", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RebootLoad RebootLoadOpen = new RebootLoad();
                    RebootLoadOpen.Show();
                    for (int i = 0; i < 300; i++)
                    {
                        RebootLoadOpen.Refresh();
                        SendRebootOrClearEEPROM(SerialPort, 10);
                        Thread.Sleep(1);
                    }
                    RebootLoadOpen.Close();
                }
            }
        }

        public void SendCalibration(SerialPort serialSerialPort, byte AccOrCompass)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 3;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG3;
                SendBuffer[VectorPointer++] = (byte)AccOrCompass;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void SendRebootOrClearEEPROM(SerialPort serialSerialPort, byte RebootOrClearEEPROM)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 3;
                if (RebootOrClearEEPROM == 100) SendBuffer[VectorPointer++] = (byte)KEY_CONFIG5;
                if (RebootOrClearEEPROM == 50) SendBuffer[VectorPointer++] = (byte)KEY_CONFIG8;
                SendBuffer[VectorPointer++] = (byte)RebootOrClearEEPROM;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void SendConfiguration(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 21;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG2;
                SendBuffer[VectorPointer++] = (byte)ComboBoxIOC;
                SendBuffer[VectorPointer++] = (byte)ComboBoxAltHold;
                SendBuffer[VectorPointer++] = (byte)ComboBoxGPSHold;
                SendBuffer[VectorPointer++] = (byte)ComboBoxRTH;
                SendBuffer[VectorPointer++] = (byte)ComboBoxPPM;
                SendBuffer[VectorPointer++] = (byte)ComboBoxGimbal;
                SendBuffer[VectorPointer++] = (byte)ComboBoxFrame;
                SendBuffer[VectorPointer++] = (byte)MotorSpeed.Value;
                SendBuffer[VectorPointer++] = (byte)ComboBoxParachute;
                SendBuffer[VectorPointer++] = (byte)ComboBoxRthAltitude;
                SendBuffer[VectorPointer++] = (byte)ComboBoxSPI;
                SendBuffer[VectorPointer++] = (byte)ComboBoxUART2;
                SendBuffer[VectorPointer++] = (byte)ComboBoxCompass;
                SendBuffer[VectorPointer++] = (byte)ComboBoxCompassRot;
                SendBuffer[VectorPointer++] = (byte)ComboBoxAcro;
                SendBuffer[VectorPointer++] = (byte)ComboBoxSport;
                SendBuffer[VectorPointer++] = (byte)ComboBoxAutoFlip;
                SendBuffer[VectorPointer++] = (byte)ComboBoxAuto;
                SendBuffer[VectorPointer++] = (byte)ComboBoxArmDisarm;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void SendConfigurationPIDAndFilters(SerialPort serialSerialPort)
        {
            UInt16 BreakPointValue = Convert.ToUInt16(numericUpDown19.Value);
            byte DynamicPIDValue = Convert.ToByte(numericUpDown18.Value);
            byte GyroLPFValue = Convert.ToByte(ComboBoxGyroLPF);
            byte DerivativeLPFValue = Convert.ToByte(numericUpDown14.Value); 
            byte RCSmoothValue = Convert.ToByte(numericUpDown20.Value);
            byte BiAccLPFValue = Convert.ToByte(numericUpDown13.Value);
            byte BiGyroLPFValue = Convert.ToByte(numericUpDown15.Value); 
            byte BiAccNotchValue = Convert.ToByte(numericUpDown16.Value);
            byte BiGyroNotchValue = Convert.ToByte(numericUpDown17.Value);
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 13;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG9;
                SendBuffer[VectorPointer++] = (byte)BreakPointValue;
                SendBuffer[VectorPointer++] = (byte)(BreakPointValue >> 8);
                SendBuffer[VectorPointer++] = (byte)DynamicPIDValue;
                SendBuffer[VectorPointer++] = (byte)GyroLPFValue;
                SendBuffer[VectorPointer++] = (byte)DerivativeLPFValue;
                SendBuffer[VectorPointer++] = (byte)RCSmoothValue;
                SendBuffer[VectorPointer++] = (byte)Convert.ToByte(ComboBoxKalmanState);
                SendBuffer[VectorPointer++] = (byte)BiAccLPFValue;
                SendBuffer[VectorPointer++] = (byte)BiGyroLPFValue;
                SendBuffer[VectorPointer++] = (byte)BiAccNotchValue;
                SendBuffer[VectorPointer++] = (byte)BiGyroNotchValue;
                SendBuffer[VectorPointer++] = (byte)Convert.ToByte(ComboBoxCompSpeed);
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void OpenSendConfiguration(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG1;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }


        public void OpenSendConfigurationPIDAndFilters(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG6;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void ExitConfiguration(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG4;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
            }
        }

        public void ExitConfigurationPidAndFilters(SerialPort serialSerialPort)
        {
            byte[] SendBuffer = new byte[250];
            int VectorPointer = 0;
            byte CheckAllBuffers = 0;
            if (serialSerialPort.IsOpen)
            {
                VectorPointer = 0;
                CheckAllBuffers = 0;
                for (int i = 0; i < 250; i++) SendBuffer[i] = 0;
                SendBuffer[VectorPointer++] = (byte)0x4A;
                SendBuffer[VectorPointer++] = (byte)0x43;
                SendBuffer[VectorPointer++] = (byte)0x5D;
                SendBuffer[VectorPointer++] = 1;
                SendBuffer[VectorPointer++] = (byte)KEY_CONFIG7;
                for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                SendBuffer[VectorPointer++] = CheckAllBuffers;
                serialSerialPort.Write(SendBuffer, 0, VectorPointer);
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
            CountAccImage = 0;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            if (RF24OpenPidAndFilters == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfigurationPidAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24OpenPidAndFilters == true) RF24OpenPidAndFilters = false;

            SmallCompass = false;
            WayPoint WayPointOpen = new WayPoint();
            WayPointOpen.Show();
        }

        private void CompassCalib_Tick(object sender, EventArgs e)
        {
            SecondsCompass++;
            label92.Text = "Tempo corrido da Calibração:" + ((SecondsCompass / 60).ToString("00.") + ":" + (SecondsCompass % 60).ToString("00."));
            if (SecondsCompass / 60 == 1)
            {
                SecondsCompass = 0;
                label92.Text = "Tempo corrido da Calibração:00:00";
                CompassCalib.Enabled = false;
                MessageBox.Show("Calibração do Compass concluída!");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CountAccImage = 0;
            if (RF24Open == true)
            {
                for (int i = 0; i < 300; i++)
                {
                    ExitConfiguration(SerialPort);
                    Thread.Sleep(1);
                }
            }
            if (RF24Open == true) RF24Open = false;

            OpenConfigLoad ConfigLoadOpen = new OpenConfigLoad();
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as Configurações com JCFLIGHT em Voo.");
                    return;
                }

                ConfigLoadOpen.Show();

                if (GyroLPFGuard > comboBox20.Items.Count) GyroLPFGuard = 4;
                if (KalmanStateGuard > comboBox21.Items.Count) KalmanStateGuard = 1;
                if (CompSpeedGuard > comboBox22.Items.Count) CompSpeedGuard = 1;

                numericUpDown19.Value = BreakPointGuard;
                numericUpDown18.Value = DynamicPIDGuard;
                comboBox20.SelectedIndex = GyroLPFGuard;
                numericUpDown14.Value = DerivativeLPFGuard;
                numericUpDown20.Value = RCSmoothGuard;
                comboBox21.SelectedIndex = KalmanStateGuard;
                numericUpDown13.Value = BiAccLPFGuard;
                numericUpDown15.Value = BiGyroLPFGuard;
                numericUpDown16.Value = BiAccNotchGuard;
                numericUpDown17.Value = BiGyroNotchGuard;
                comboBox22.SelectedIndex = CompSpeedGuard;

                for (int i = 0; i < 300; i++)
                {
                    ConfigLoadOpen.Refresh();
                    OpenSendConfigurationPIDAndFilters(SerialPort);
                    Thread.Sleep(1);
                }
                RF24OpenPidAndFilters = true;
            }
            ConfigLoadOpen.Close();
            SmallCompass = false;
            tabControl1.SelectTab(tabPage7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            EraseEEPROM EraseEEPROMOpen = new EraseEEPROM();
            if (SerialPort.IsOpen == true)
            {
                if (MessageBox.Show("Clicando em 'Sim' todas as configurações feitas aqui serão apagadas,você realmete deseja fazer isso?",
               "Limpar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EraseEEPROMOpen.Show();
                    for (int i = 0; i < 300; i++)
                    {
                        EraseEEPROMOpen.Refresh();
                        SendRebootOrClearEEPROM(SerialPort, 100);
                        Thread.Sleep(1);
                    }
                    EraseEEPROMOpen.Close();
                }
            }
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

        private void CheckCompassState(Int16 HeadingCompass)
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
            if (ReadRoll > 1000)
            {
                label94.Text = "AHRS:Ruim";
                label94.ForeColor = Color.Red;
                pictureBox10.BackColor = Color.Red;
                pictureBox13.BackColor = Color.Red;
                pictureBox15.BackColor = Color.Red;
                pictureBox17.BackColor = Color.Red;
                pictureBox19.BackColor = Color.Red;
                pictureBox21.BackColor = Color.Red;
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
                label20.Text = "Altitude-Hold";
                label42.Text = "> Retenção de Altitude com base no Barômetro e INS";
                label24.Text = "Auto-Flip";
                label48.Text = "> Realiza Flips Automáticos de 180° no Pitch e Roll";
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
                label24.Text = "Turn Mode";
                label48.Text = "> Ajuste dinâmico para o Aileron com base na Attitude";
                label20.Text = "Auto-Throttle";
                label42.Text = "> Velocidade constante com base no Tubo de Pitot";
                if (groupBox6.Text == "VELOCIDADE INICIAL DOS MOTORES AO ARMAR A JCFLIGHT")
                    groupBox6.Text = "VELOCIDADE INICIAL DO MOTOR AO ARMAR A JCFLIGHT";
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
            else if (ComboBoxFrame == 8) //FOGUETE
            {
                label22.Text = "IOC";
                label44.Text = "> Controle de Orientação Inteligente";
                label23.Text = "Ataque";
                label46.Text = "> Modo Stabilize com Limite maior no Ângulo (55°)";
                label20.Text = "Altitude-Hold";
                label42.Text = "> Retenção de Altitude com base no Barômetro e INS";
                label24.Text = "Auto-Flip";
                label48.Text = "> Realiza Flips Automáticos de 180° no Pitch e Roll";
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

        private void button16_Click(object sender, EventArgs e)
        {
            SendConfigLoad SendConfigLoadOpen = new SendConfigLoad();
            if (SerialPort.IsOpen == true)
            {
                SendConfigLoadOpen.Show();

                for (int i = 0; i < 300; i++)
                {
                    SendConfigLoadOpen.Refresh();
                    SendConfigurationPIDAndFilters(SerialPort);
                    Thread.Sleep(11);
                }

                SendConfigLoadOpen.Close();

                if (MessageBox.Show("É necessario reiciar a JCFLIGHT para aplicar as alterações,deseja reiniciar automaticamete?",
                    "Reboot Automático", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RebootLoad RebootLoadOpen = new RebootLoad();
                    RebootLoadOpen.Show();
                    for (int i = 0; i < 300; i++)
                    {
                        RebootLoadOpen.Refresh();
                        SendRebootOrClearEEPROM(SerialPort, 10);
                        Thread.Sleep(1);
                    }
                    RebootLoadOpen.Close();
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            EraseEEPROM EraseEEPROMOpen = new EraseEEPROM();
            if (SerialPort.IsOpen == true)
            {
                if (MessageBox.Show("Clicando em 'Sim' todas as configurações feitas aqui serão apagadas,você realmete deseja fazer isso?",
               "Limpar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    numericUpDown18.Value = 0;
                    numericUpDown19.Value = 1000;
                    comboBox20.SelectedIndex = 0;
                    numericUpDown14.Value = 0;
                    numericUpDown20.Value = 0;
                    comboBox21.SelectedIndex = 0;
                    numericUpDown13.Value = 0;
                    numericUpDown15.Value = 0;
                    numericUpDown16.Value = 0;
                    numericUpDown17.Value = 0;
                    comboBox22.SelectedIndex = 0;
                    EraseEEPROMOpen.Show();
                    for (int i = 0; i < 300; i++)
                    {
                        EraseEEPROMOpen.Refresh();
                        SendRebootOrClearEEPROM(SerialPort, 50);
                        Thread.Sleep(1);
                    }
                    EraseEEPROMOpen.Close();
                }
            }
        }

        void metroTrackBar1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        int PingMin = 99999999;
        int PingMax = 0;
        void Calculate_Ping_Min_Max()
        {
            if (TelemetryPing == 0) return;
            if (TelemetryPing < PingMin) PingMin = TelemetryPing;
            if (TelemetryPing > PingMax) PingMax = TelemetryPing;
            label113.Text = "Ping Min/Max:" + PingMin + "ms" + "/" + PingMax + "ms";
        }
    }
}