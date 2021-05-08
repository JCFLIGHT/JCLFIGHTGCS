using System;
using System.Collections.Generic;
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
using System.Drawing.Drawing2D;
using System.IO;
using WMPLib;

namespace JCFLIGHTGCS
{
    public partial class GCS : Form
    {
        SerialPort SerialPort = new SerialPort();
        WindowsMediaPlayer Player = new WindowsMediaPlayer();

        string SerialComPort;
        string[] SerialPorts = SerialPort.GetPortNames();
        static bool Error_Received = false;
        static byte Read_State = 0;
        static byte OffSet = 0;
        static byte DataSize = 0;
        static byte CheckSum = 0;
        static byte Command;
        static byte[] InBuffer = new byte[300];
        static byte[] NumericConvert = new byte[50];

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
        int ThrottleActualData = 900;
        int YawActualData = 1000;
        int PitchActualData = 1000;
        int RollActualData = 1000;
        int Aux1ActualData = 1000;
        int Aux2ActualData = 1000;
        int Aux3ActualData = 1000;
        int Aux4ActualData = 1000;
        int Aux5ActualData = 1000;
        int Aux6ActualData = 1000;
        int Aux7ActualData = 1000;
        int Aux8ActualData = 1000;
        int ThrottleAttitudeData = 1000;
        int YawAttitudeData = 0;
        int PitchAttitudeData = 0;
        int RollAttitudeData = 0;
        int ReadRoll = 0;
        int ReadPitch = 0;
        int ReadCompass = 0;
        byte CPULoad = 0;

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
        int GPS_NumSat = 0;
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

        static GMapRoute Grout;
        static GMapOverlay Routes;
        static GMapOverlay AirportsOverlay;
        GMapOverlay PositionToRoutes;
        GMapOverlay MarkersOverlay = new GMapOverlay("Markers");
        GMapOverlay GmapPolygons = new GMapOverlay("Poligonos");
        List<PointLatLng> Points = new List<PointLatLng>();
        PointLatLng GPS_Position;
        PointLatLng GPS_Position2;

        int CompassHealthCount = 9999999;
        int HeadingCompassPrev = 0;

        byte ComboBoxSimpleMode = 0;
        byte ComboBoxAltHold = 0;
        byte ComboBoxGPSHold = 0;
        byte ComboBoxRTH = 0;
        byte ComboBoxPPM = 0;
        byte ComboBoxGimbal = 0;
        byte ComboBoxFrame = 0;
        byte ComboBoxParachute = 0;
        byte ComboBoxSafeBtn = 0;
        byte ComboBoxAirSpeed = 0;
        byte ComboBoxSPI = 0;
        byte ComboBoxUART2 = 0;
        byte ComboBoxUart1 = 0;
        byte ComboBoxCompassRot = 0;
        byte ComboBoxAcro = 0;
        byte ComboBoxSport = 0;
        byte ComboBoxAutoFlip = 0;
        byte ComboBoxAuto = 0;
        byte ComboBoxArmDisarm = 0;
        byte ComboBoxGyroLPF = 0;
        byte ComboBoxKalmanState = 0;
        byte ComboBoxCompSpeed = 0;
        byte ComboBoxAutoLand = 0;
        byte SimpleDataGuard = 0;
        byte AltHoldGuard = 0;
        byte GPSHoldGuard = 0;
        byte RTHGuard = 0;
        byte PPMGuard = 0;
        byte GimbalGuard = 0;
        byte FrameGuard = 0;
        byte ParachuteGuard = 0;
        byte RthAltitudeGuard = 0;
        byte OptFlowGuard = 0;
        byte SonarGuard = 0;
        byte Uart1Guard = 0;
        byte CompassRotGuard = 0;
        byte AcroGuard = 0;
        byte SportGuard = 0;
        byte AutoFlipGuard = 0;
        byte AutoGuard = 0;
        byte ArmDisarmGuard = 0;
        byte AutoLandGuard = 0;
        byte SafeBtnGuard = 0;
        byte AirSpeedGuard = 0;
        int PitchLevelTrimGuard;

        int DevicesSum = 0;

        double AmperInMah = 0;

        int TPABreakPoint = 1000;

        byte TPAFactor = 0;
        byte GyroLPF = 0;
        int DerivativeLPF;
        int RCSmooth;
        int ServosLPF;
        byte KalmanState;
        int BiAccLPF;
        int BiGyroLPF;
        int BiAccNotch;
        int BiGyroNotch;
        byte CompSpeed;

        int CoG = 0;
        Int32 Crosstrack = 0;

        bool ItsSafeToUpdate = true;

        static int PacketsError = 0;
        static int PacketsReceived = 0;

        double GetAccGForce = 0;

        byte GetAccCalibFlag = 0;

        bool MessageRead = false;
        bool Reboot = false;
        bool SmallFlightData = false;
        bool ForceNewLocationToLabels = false;
        bool HomePointMarkerOK = false;
        bool ResetTimer = false;
        byte Hour_Debug = 0;
        int Seconds_Count = 0;
        int Hour_Count = 0;

        byte MemoryRamUsedPercent = 0;
        int MemoryRamUsed = 0;

        int AirportsCountTime = 0;

        public string[] GetString1;
        public string[] GetString2;
        public string[] GetString3;
        public string[] GetString4;
        public string[] GetString5;
        public string[] GetString6;
        public string[] GetString7;
        Boolean StringsChecked = false;
        int DecodeString = 0;

        double[] PushedLatitude = new double[100];
        double[] PushedLongitude = new double[100];
        Boolean ParamsPushed = false;
        Boolean PrintArea2 = false;
        Boolean BlockPushParams = false;
        Boolean SafeToPushParams = false;
        byte CountWP2 = 0;
        byte CountToBlock = 0;
        int WPRadius = 200;
        bool AutoWpPushed = false;

        byte ThrottleMiddle = 0;
        byte ThrottleExpo = 0;
        byte RCRate = 0;
        byte RcExpo = 0;
        byte YawRate = 0;
        int RadioMin = 1000;
        int RadioMax = 2000;
        int ThrottleMin;
        int YawMin;
        int PitchMin;
        int RollMin;
        int ThrottleMax;
        int YawMax;
        int PitchMax;
        int RollMax;
        byte ThrottleDeadZone;
        byte YawDeadZone;
        byte PitchDeadZone;
        byte RollDeadZone;
        byte ChannelsReverse;
        byte ServosReverse;
        int Servo1Min;
        int Servo2Min;
        int Servo3Min;
        int Servo4Min;
        int Servo1Med;
        int Servo2Med;
        int Servo3Med;
        int Servo4Med;
        int Servo1Max;
        int Servo2Max;
        int Servo3Max;
        int Servo4Max;
        int Servo1Rate;
        int Servo2Rate;
        int Servo3Rate;
        int Servo4Rate;
        int FailSafeValue;
        byte MaxRollLevel;
        byte MaxPitchLevel;
        int Integral_Relax_LPF;
        int kCD_or_FF_LPF;

        StreamWriter BlackBoxStream;
        static bool BlackBoxRunning = false;

        string RamMemString = "8192KB";

        Form WaitUart = Program.WaitUart;
        Form RebootBoard = Program.RebootBoard;

        public GCS()
        {
            InitializeComponent();
            SerialPort.DataBits = 8;
            SerialPort.Parity = Parity.None;
            SerialPort.StopBits = StopBits.One;
            SerialPort.Handshake = Handshake.None;
            SerialPort.DtrEnable = false;
            SerialPort.ReadBufferSize = 4096;
            SerialPort.ReadTimeout = 500;
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
            Pen PenRoute = new Pen(Color.Purple, 5);
            Grout = new GMapRoute(Points, "Track");
            Grout.Stroke = PenRoute;
            Routes.Routes.Add(Grout);
            AirportsOverlay = new GMapOverlay("AirportsOverlay");
            MyGMap.Overlays.Add(AirportsOverlay);
            MyGMap.Invalidate(false);
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
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size; //NÃO CUBRA A BARRA DE TAREFAS
            this.WindowState = FormWindowState.Maximized;
            maximinizar.Visible = true;
            iconmaximizar.Visible = true;
            preArmNotification2.Visible = false;
            HUD2.Visible = false;
            label159.BackColor = Color.Blue;
            label159.ForeColor = Color.Red;
            label156.BackColor = Color.Blue;
            label156.ForeColor = Color.White;
            label160.BackColor = Color.Blue;
            label160.ForeColor = Color.Orange;
            label157.BackColor = Color.Blue;
            label157.ForeColor = Color.White;
            label158.BackColor = Color.Blue;
            label158.ForeColor = Color.White;

            if (!GCSSettings.Read_From_XML(Settings.GetRunningDirectory() + "GCSSettings.xml"))
            {
                MessageBox.Show("Error ao ler o arquivo de configuração do GCS.");
            }

            Airports.ReadOurairports(Settings.GetRunningDirectory() + "Airports.csv");
            Airports.checkdups = true;

            terminalControl1.Width = 1113;
            terminalControl1.Height = 590;

            this.ResumeLayout();

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
            if (!ForceNewLocationToLabels)
            {
                MyGMap.Width = 800;
                MyGMap.Height = 555;
            }
            else
            {
                MyGMap.Size = new Size(850, 555);
            }
            terminalControl1.Width = 1113;
            terminalControl1.Height = 590;
            maximinizar.Visible = true;
            iconmaximizar.Visible = true;
        }

        private void maximinizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            if (!ForceNewLocationToLabels)
            {
                MyGMap.Width = 740;
                MyGMap.Height = 555;
            }
            else
            {
                MyGMap.Size = new Size(790, 555);
            }
            terminalControl1.Width = 1047;
            terminalControl1.Height = 572;
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
                SerialPort.DiscardInBuffer();
                SerialPort.Close();
            }
            MyGMap.Dispose();
            MyGMap.Invalidate();
            this.Close();
        }

        public delegate bool ProcessCmdKeyHandler(ref Message msg, Keys keyData);
        public event ProcessCmdKeyHandler ProcessCmdKeyCallback;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                button1_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F2))
            {
                button6_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F3))
            {
                button2_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F4))
            {
                button3_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F5))
            {
                button4_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F6))
            {
                button5_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F7))
            {
                button10_Click_1(null, null);
                return true;
            }
            if (keyData == (Keys.F8))
            {
                button13_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F9))
            {
                button12_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F10))
            {
                button19_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F11))
            {
                button18_Click(null, null);
                return true;
            }
            if (keyData == (Keys.F12))
            {
                button11_Click(null, null);
                return true;
            }
            if (keyData == (Keys.Control | Keys.P))
            {
                Form Form = new BoardInfo();
                Form.Show();
                return true;
            }

            if (ProcessCmdKeyCallback != null)
            {
                return ProcessCmdKeyCallback(ref msg, keyData);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void RealTimer_Tick(object sender, EventArgs e)
        {
            CheckCompassState(ReadCompass);
            Edit_Labels_To_Aero();
            UpdateDevices();
            DateTime DateTimeNow = DateTime.UtcNow;
            TimeZoneInfo HRBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            label72.Text = Convert.ToString(TimeZoneInfo.ConvertTimeFromUtc(DateTimeNow, HRBrasilia));
            PushWayPointCoordinatesOfFlightController();
            if (BlackBoxRunning && BlackBoxStream.BaseStream != null)
            {
                UpdateBlackBoxData();
            }
            throttleExpo1.SetRCExpoParameters((double)ThrottleMiddle / 100, (double)ThrottleExpo / 100, ThrottleData);
            rcExpo1.SetRCExpoParameters((double)RCRate / 100, (double)RcExpo / 100);
            throttleExpo2.SetRCExpoParameters((double)numericUpDown25.Value, (double)numericUpDown26.Value, ThrottleData);
            rcExpo2.SetRCExpoParameters((double)numericUpDown27.Value, (double)numericUpDown28.Value);
            InertialSensor.AccCalcVibrationAndClipping();
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte CheckState;

            try
            {
                if (SerialPort.IsOpen == false)
                {
                    return;
                }
            }
            catch { }

            while (SerialPort.BytesToRead > 0)
            {
                try
                {
                    if (SerialPort.IsOpen == false) break;
                }
                catch (UnauthorizedAccessException) { }

                try
                {
                    if (SerialPort.BytesToRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        CheckState = (byte)SerialPort.ReadByte();
                    }

                    switch (Read_State)
                    {
                        case 0:
                            if (CheckState == 0x4a)
                            {
                                Read_State = 1;
                            }
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
                            if (DataSize > 254)
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
                catch (UnauthorizedAccessException) { }
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
                    if (!BlockPushParams) ParamsPushed = true;
                    break;

                case 7:
                    ptr = 0;
                    GetValues.ReadAttitudePitch = ReadPitch = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.ReadAttitudeRoll = ReadRoll = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.ReadAttitudeYaw = ReadCompass = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
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
                    GetValues.ReadBarometer = ReadBarometer = Convert.ToDouble(BitConverter.ToInt32(InBuffer, ptr)) / 100; ptr += 4;
                    FailSafeDetect = (byte)InBuffer[ptr++];
                    GetValues.ReadBattVoltage = BattVoltage = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    GetValues.ReadBattPercentage = BattPercentage = (byte)InBuffer[ptr++];
                    CommandArmDisarm = (byte)InBuffer[ptr++];
                    HDOP = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    GetValues.ReadBattCurrent = Current = (double)BitConverter.ToInt16(InBuffer, ptr) / 1000.0; ptr += 2;
                    GetValues.ReadBattWatts = Watts = (double)BitConverter.ToInt32(InBuffer, ptr) / 1000.0; ptr += 4;
                    Declination = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    FlightMode = (byte)InBuffer[ptr++];
                    FrameMode = (byte)InBuffer[ptr++];
                    HomePointOK = (byte)InBuffer[ptr++];
                    GetValues.ReadTemperature = Temperature = (byte)InBuffer[ptr++];
                    HomePointDisctance = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    AmperInMah = (double)BitConverter.ToInt16(InBuffer, ptr) / 1000.0; ptr += 2;
                    GetValues.ReadGroundCourse = CoG = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Crosstrack = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetAccGForce = Convert.ToDouble(BitConverter.ToInt16(InBuffer, ptr)) / 100; ptr += 2;
                    GetAccCalibFlag = (byte)InBuffer[ptr++];
                    GetValues.CompassX = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.CompassY = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.CompassZ = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    break;

                case 8:
                    ptr = 0;
                    FrameGuard = (byte)InBuffer[ptr++];
                    PPMGuard = (byte)InBuffer[ptr++];
                    GimbalGuard = (byte)InBuffer[ptr++];
                    ParachuteGuard = (byte)InBuffer[ptr++];
                    OptFlowGuard = (byte)InBuffer[ptr++];
                    SonarGuard = (byte)InBuffer[ptr++];
                    Uart1Guard = (byte)InBuffer[ptr++];
                    CompassRotGuard = (byte)InBuffer[ptr++];
                    RthAltitudeGuard = (byte)InBuffer[ptr++];
                    AcroGuard = (byte)InBuffer[ptr++];
                    AltHoldGuard = (byte)InBuffer[ptr++];
                    GPSHoldGuard = (byte)InBuffer[ptr++];
                    SimpleDataGuard = (byte)InBuffer[ptr++];
                    RTHGuard = (byte)InBuffer[ptr++];
                    SportGuard = (byte)InBuffer[ptr++];
                    AutoFlipGuard = (byte)InBuffer[ptr++];
                    AutoGuard = (byte)InBuffer[ptr++];
                    ArmDisarmGuard = (byte)InBuffer[ptr++];
                    AutoLandGuard = (byte)InBuffer[ptr++];
                    SafeBtnGuard = (byte)InBuffer[ptr++];
                    GetValues.AirSpeedEnabled = AirSpeedGuard = (byte)InBuffer[ptr++];
                    PitchLevelTrimGuard = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    break;

                case 9:
                    ptr = 0;
                    TPAFactor = (byte)InBuffer[ptr++];
                    TPABreakPoint = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
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
                    ServosLPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    NumericConvert[12] = (byte)InBuffer[ptr++];
                    NumericConvert[13] = (byte)InBuffer[ptr++];
                    NumericConvert[14] = (byte)InBuffer[ptr++];
                    NumericConvert[15] = (byte)InBuffer[ptr++];
                    NumericConvert[16] = (byte)InBuffer[ptr++];
                    NumericConvert[17] = (byte)InBuffer[ptr++];
                    NumericConvert[18] = (byte)InBuffer[ptr++];
                    GetValues.BankAngleRollValue = NumericConvert[19] = (byte)InBuffer[ptr++];
                    NumericConvert[20] = (byte)InBuffer[ptr++];
                    NumericConvert[21] = (byte)InBuffer[ptr++];
                    NumericConvert[22] = (byte)InBuffer[ptr++];
                    NumericConvert[23] = (byte)InBuffer[ptr++];
                    Integral_Relax_LPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    kCD_or_FF_LPF = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    NumericConvert[24] = (byte)InBuffer[ptr++];
                    NumericConvert[25] = (byte)InBuffer[ptr++];
                    NumericConvert[26] = (byte)InBuffer[ptr++];
                    NumericConvert[27] = (byte)InBuffer[ptr++];
                    NumericConvert[28] = (byte)InBuffer[ptr++];
                    NumericConvert[29] = (byte)InBuffer[ptr++];
                    NumericConvert[30] = (byte)InBuffer[ptr++];
                    NumericConvert[31] = (byte)InBuffer[ptr++];
                    NumericConvert[32] = (byte)InBuffer[ptr++];
                    NumericConvert[33] = (byte)InBuffer[ptr++];
                    NumericConvert[34] = (byte)InBuffer[ptr++];
                    break;

                case 10:
                    ptr = 0;
                    ThrottleActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    YawActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    PitchActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RollActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux1ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux2ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux3ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux4ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux5ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux6ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux7ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Aux8ActualData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ThrottleAttitudeData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    YawAttitudeData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    PitchAttitudeData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RollAttitudeData = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    MemoryRamUsed = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    MemoryRamUsedPercent = (byte)InBuffer[ptr++];
                    GetValues.AccX = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.AccY = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.AccZ = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.GyroX = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.GyroY = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.GyroZ = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.ReadGroundSpeed = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.ReadI2CError = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    GetValues.ReadAirSpeed = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    CPULoad = (byte)InBuffer[ptr++];
                    break;

                case 21:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString1 = new string[builder.ToString().Split(';').Length];
                        GetString1 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 22:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString2 = new string[builder.ToString().Split(';').Length];
                        GetString2 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 23:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString3 = new string[builder.ToString().Split(';').Length];
                        GetString3 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 24:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString4 = new string[builder.ToString().Split(';').Length];
                        GetString4 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 25:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString5 = new string[builder.ToString().Split(';').Length];
                        GetString5 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 26:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString6 = new string[builder.ToString().Split(';').Length];
                        GetString6 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 27:
                    DecodeString = 20;
                    while (DecodeString > 0)
                    {
                        ptr = 0;
                        StringBuilder builder = new StringBuilder();
                        while (ptr < DataSize) builder.Append((char)InBuffer[ptr++]);
                        builder.Remove(builder.Length - 1, 1);
                        GetString7 = new string[builder.ToString().Split(';').Length];
                        GetString7 = builder.ToString().Split(';');
                        DecodeString = DecodeString - 1;
                    }
                    break;

                case 30:
                    ptr = 0;
                    ThrottleMiddle = (byte)InBuffer[ptr++];
                    ThrottleExpo = (byte)InBuffer[ptr++];
                    RCRate = (byte)InBuffer[ptr++];
                    RcExpo = (byte)InBuffer[ptr++];
                    YawRate = (byte)InBuffer[ptr++];
                    RadioMin = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RadioMax = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ThrottleMin = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    YawMin = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    PitchMin = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RollMin = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ThrottleMax = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    YawMax = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    PitchMax = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    RollMax = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ThrottleDeadZone = (byte)InBuffer[ptr++];
                    YawDeadZone = (byte)InBuffer[ptr++];
                    PitchDeadZone = (byte)InBuffer[ptr++];
                    RollDeadZone = (byte)InBuffer[ptr++];
                    ChannelsReverse = (byte)InBuffer[ptr++];
                    Servo1Rate = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo2Rate = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo3Rate = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo4Rate = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    ServosReverse = (byte)InBuffer[ptr++];
                    Servo1Min = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo2Min = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo3Min = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo4Min = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo1Med = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo2Med = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo3Med = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo4Med = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo1Max = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo2Max = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo3Max = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    Servo4Max = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    FailSafeValue = BitConverter.ToInt16(InBuffer, ptr); ptr += 2;
                    MaxRollLevel = (byte)InBuffer[ptr++];
                    MaxPitchLevel = (byte)InBuffer[ptr++];
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
            try
            {
                SerialPort.Write(Buffer, 0, 6);
            }
            catch { }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (SerialPort.IsOpen == false)
                {
                    return;
                }
            }
            catch { }

            if (SerialPort.BytesToRead == 0)
            {
                if (ItsSafeToUpdate)
                {
                    Serial_Write_To_FC(7);
                    Serial_Write_To_FC(8);
                    Serial_Write_To_FC(9);
                    Serial_Write_To_FC(10);
                    Serial_Write_To_FC(30);
                    UpdateAccImageStatus();
                    GetBuildOfBoard();
                    UpdateComboBoxSum();
                }
            }
        }

        private void GetBuildOfBoard()
        {
            if (!StringsChecked)
            {
                Serial_Write_To_FC(21);
                Serial_Write_To_FC(22);
                Serial_Write_To_FC(23);
                Serial_Write_To_FC(24);
                Serial_Write_To_FC(25);
                Serial_Write_To_FC(26);
            }

            if (GetString1 != null)
            {
                GetValues.GetPlatformName = GetString1[0];
                if (GetValues.GetPlatformName == "AVR")
                {
                    numericUpDown13.Enabled = false;
                    numericUpDown15.Enabled = false;
                    numericUpDown16.Enabled = false;
                    numericUpDown17.Enabled = false;
                    numericUpDown20.Enabled = false;
                    numericUpDown21.Enabled = false;
                    comboBox22.Enabled = false;

                    RamMemString = "8192KB";
                }
                else if (GetValues.GetPlatformName == "ESP32")
                {
                    numericUpDown13.Enabled = true;
                    numericUpDown15.Enabled = true;
                    numericUpDown16.Enabled = true;
                    numericUpDown17.Enabled = true;
                    numericUpDown20.Enabled = true;
                    numericUpDown21.Enabled = true;
                    comboBox22.Enabled = true;
                    RamMemString = "327680KB";
                }
                else if (GetValues.GetPlatformName == "STM32")
                {
                    numericUpDown13.Enabled = true;
                    numericUpDown15.Enabled = true;
                    numericUpDown16.Enabled = true;
                    numericUpDown17.Enabled = true;
                    numericUpDown20.Enabled = true;
                    numericUpDown21.Enabled = true;
                    comboBox22.Enabled = true;
                    RamMemString = "131072KB";
                }
            }

            if (GetString2 != null)
            {
                GetValues.GetFirwareName = GetString2[0];
            }

            if (GetString3 != null)
            {
                GetValues.GetFirwareVersion = GetString3[0];
            }

            if (GetString4 != null)
            {

                GetValues.GetCompilerVersion = GetString4[0];
            }

            if (GetString5 != null)
            {
                GetValues.GetBuildDate = GetString5[0];
            }

            if (GetString6 != null)
            {
                GetValues.GetBuildTime = GetString6[0];
            }

            if (GetString7 != null)
            {
                GetValues.PreArmMessage = GetString7[0];
                StringsChecked = true;
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
            metroProgressBar1.Value = Convert.ToInt16(ValueConverterProgressBar(CHThrottle, 1000, 2000, 0, 100));
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
            label28.Text = Convert.ToString(Math.Max(1000, CHThrottle));
            label29.Text = Convert.ToString(Math.Max(1000, CHYaw));
            label30.Text = Convert.ToString(Math.Max(1000, CHPitch));
            label31.Text = Convert.ToString(Math.Max(1000, CHRoll));
            label32.Text = Convert.ToString(Math.Max(1000, CHAux1));
            label33.Text = Convert.ToString(Math.Max(1000, CHAux2));
            label34.Text = Convert.ToString(Math.Max(1000, CHAux3));
            label35.Text = Convert.ToString(Math.Max(1000, CHAux4));
            label36.Text = Convert.ToString(Math.Max(1000, CHAux5));
            label37.Text = Convert.ToString(Math.Max(1000, CHAux6));
            label38.Text = Convert.ToString(Math.Max(1000, CHAux7));
            label38.Text = Convert.ToString(Math.Max(1000, CHAux8));
        }

        private void ProgressBarControl2(int CHThrottle, int CHYaw, int CHPitch, int CHRoll, int CHAux1, int CHAux2,
        int CHAux3, int CHAux4, int CHAux5, int CHAux6, int CHAux7, int CHAux8)
        {
            //CONTROLE DAS BARRAS DE PROGRESSO
            metroProgressBar23.Value = Convert.ToInt16(ValueConverterProgressBar(CHThrottle, 900, 2200, 0, 100));
            metroProgressBar22.Value = Convert.ToInt16(ValueConverterProgressBar(CHYaw, 900, 2200, 0, 100));
            metroProgressBar21.Value = Convert.ToInt16(ValueConverterProgressBar(CHPitch, 900, 2200, 0, 100));
            metroProgressBar20.Value = Convert.ToInt16(ValueConverterProgressBar(CHRoll, 900, 2200, 0, 100));
            metroProgressBar19.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux1, 900, 2200, 0, 100));
            metroProgressBar18.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux2, 900, 2200, 0, 100));
            metroProgressBar17.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux3, 900, 2200, 0, 100));
            metroProgressBar16.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux4, 900, 2200, 0, 100));
            metroProgressBar15.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux5, 900, 2200, 0, 100));
            metroProgressBar14.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux6, 900, 2200, 0, 100));
            metroProgressBar24.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux7, 900, 2200, 0, 100));
            metroProgressBar13.Value = Convert.ToInt16(ValueConverterProgressBar(CHAux8, 900, 2200, 0, 100));
            //LABEL'S
            label141.Text = Convert.ToString(Math.Max(900, CHThrottle));
            label140.Text = Convert.ToString(Math.Max(900, CHYaw));
            label139.Text = Convert.ToString(Math.Max(900, CHPitch));
            label138.Text = Convert.ToString(Math.Max(900, CHRoll));
            label137.Text = Convert.ToString(Math.Max(900, CHAux1));
            label136.Text = Convert.ToString(Math.Max(900, CHAux2));
            label135.Text = Convert.ToString(Math.Max(900, CHAux3));
            label134.Text = Convert.ToString(Math.Max(900, CHAux4));
            label133.Text = Convert.ToString(Math.Max(900, CHAux5));
            label132.Text = Convert.ToString(Math.Max(900, CHAux6));
            label131.Text = Convert.ToString(Math.Max(900, CHAux7));
            label130.Text = Convert.ToString(Math.Max(900, CHAux8));
        }

        private void ProgressBarControl3(int CHThrottle, int CHYaw, int CHPitch, int CHRoll)
        {
            //CONTROLE DAS BARRAS DE PROGRESSO
            metroProgressBar28.Value = Convert.ToInt16(ValueConverterProgressBar(CHThrottle, 1000, 2000, 0, 100));
            metroProgressBar27.Value = Convert.ToInt16(ValueConverterProgressBar(CHYaw, -YawRate * 10, YawRate * 10, 0, 100));
            metroProgressBar26.Value = Convert.ToInt16(ValueConverterProgressBar(CHPitch, -RCRate * 10, RCRate * 10, 0, 100));
            metroProgressBar25.Value = Convert.ToInt16(ValueConverterProgressBar(CHRoll, -RCRate * 10, RCRate * 10, 0, 100));
            //LABEL'S
            label149.Text = Convert.ToString(CHThrottle);
            label148.Text = Convert.ToString(CHYaw);
            label147.Text = Convert.ToString(CHPitch);
            label146.Text = Convert.ToString(CHRoll);
        }

        long ValueConverterProgressBar(int x, int in_min, int in_max, int out_min, int out_max)
        {
            if (x <= in_min) return 0;
            if (x >= in_max) return 100;
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer2.Interval = GCSSettings.GCSRate;
            GetValues.GCSFrequency = GCSSettings.GCSFrequency;
            GetValues.GCSSpeech = GCSSettings.GCSSpeech;
            GetValues.GCSRebootBoard = GCSSettings.GCSRebootBoard;
            GetValues.GCSAutoWP = GCSSettings.GCSAutoWP;
            GetValues.GCSTrackLength = GCSSettings.GCSTrackLength;
            GetValues.GCSAirPorts = GCSSettings.GCSAirPorts;
            GetValues.GCSTrackSize = GCSSettings.GCSTrackSize;

            if (GCSSettings.GCSAutoWP > 0 && !AutoWpPushed && GPS_NumSat >= 5)
            {
                carregarWPsToolStripMenuItem_Click(null, null);
                AutoWpPushed = true;
            }

            label76.Text = Convert.ToString(GPS_NumSat);

            if (GPS_NumSat < 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label76.Location = new Point(215, 89);
                }
                else
                {
                    label76.Location = new Point(415, 89);
                }
            }
            else
            {
                if (!ForceNewLocationToLabels)
                {
                    label76.Location = new Point(210, 89);
                }
                else
                {
                    label76.Location = new Point(410, 89);
                }
            }

            if (HDOP >= 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label78.Location = new Point(197, 140);
                }
                else
                {
                    label78.Location = new Point(397, 140);
                }
                label78.Text = HDOP.ToString(new CultureInfo("en-US"));
            }
            else
            {
                if (!ForceNewLocationToLabels)
                {
                    label78.Location = new Point(200, 140);
                }
                else
                {
                    label78.Location = new Point(400, 140);
                }
                label78.Text = HDOP.ToString(new CultureInfo("en-US"));
            }

            if (ReadBarometer < 1000)
            {
                if (!ForceNewLocationToLabels)
                {
                    label79.Location = new Point(205, 190);
                }
                else
                {
                    label79.Location = new Point(405, 190);
                }
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "M";
            }
            else if (ReadBarometer >= 1000 && ReadBarometer <= 10000)
            {
                if (!ForceNewLocationToLabels)
                {
                    label79.Location = new Point(185, 190);
                }
                else
                {
                    label79.Location = new Point(385, 190);
                }
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "KM";
            }
            else
            {
                if (!ForceNewLocationToLabels)
                {
                    label79.Location = new Point(180, 190);
                }
                else
                {
                    label79.Location = new Point(380, 190);
                }
                label79.Text = ReadBarometer.ToString(new CultureInfo("en-US")) + "KM";
            }

            label87.Text = Current.ToString(new CultureInfo("en-US")) + "A";

            if (Current < 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label87.Location = new Point(200, 404);
                }
                else
                {
                    label87.Location = new Point(400, 404);
                }
            }

            if (Current >= 10 && Current < 100)
            {
                if (!ForceNewLocationToLabels)
                {
                    label87.Location = new Point(195, 404);
                }
                else
                {
                    label87.Location = new Point(395, 404);
                }
            }

            if (Current >= 100)
            {
                if (!ForceNewLocationToLabels)
                {
                    label87.Location = new Point(185, 404);
                }
                else
                {
                    label87.Location = new Point(385, 404);
                }
            }

            label116.Text = AmperInMah.ToString(new CultureInfo("en-US")) + "MAH";

            if (AmperInMah < 0.10f)
            {
                if (!ForceNewLocationToLabels)
                {
                    label116.Location = new Point(200, 450);
                }
                else
                {
                    label116.Location = new Point(400, 450);
                }
            }

            if (AmperInMah >= 0.10f && AmperInMah < 0.100f)
            {
                if (!ForceNewLocationToLabels)
                {
                    label116.Location = new Point(195, 450);
                }
                else
                {
                    label116.Location = new Point(395, 450);
                }
            }

            if (AmperInMah >= 0.100f && AmperInMah < 1)
            {
                if (!ForceNewLocationToLabels)
                {
                    label116.Location = new Point(185, 450);
                }
                else
                {
                    label116.Location = new Point(385, 450);
                }
            }

            if (AmperInMah >= 1 && AmperInMah < 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label116.Location = new Point(175, 450);
                }
                else
                {
                    label116.Location = new Point(375, 450);
                }
            }

            if (AmperInMah >= 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label116.Location = new Point(165, 450);
                }
                else
                {
                    label116.Location = new Point(365, 450);
                }
            }

            label89.Text = Watts.ToString(new CultureInfo("en-US")) + "W";

            if (Watts < 10)
            {
                if (!ForceNewLocationToLabels)
                {
                    label89.Location = new Point(208, 496);
                }
                else
                {
                    label89.Location = new Point(408, 496);
                }
            }

            if (Watts >= 10 && Watts < 100)
            {
                if (!ForceNewLocationToLabels)
                {
                    label89.Location = new Point(195, 496);
                }
                else
                {
                    label89.Location = new Point(395, 496);
                }
            }

            if (Watts >= 100)
            {
                if (!ForceNewLocationToLabels)
                {
                    label89.Location = new Point(185, 496);
                }
                else
                {
                    label89.Location = new Point(385, 496);
                }
            }

            if (Declination != 0) label81.Text = Declination.ToString(new CultureInfo("en-US")) + "°";
            if ((Declination > 0 && Declination < 10) || (Declination > (-10) && Declination < 0))
            {
                if (!ForceNewLocationToLabels)
                {
                    label81.Location = new Point(205, 246);
                }
                else
                {
                    label81.Location = new Point(405, 246);
                }
            }
            if ((Declination >= 10 && Declination < 100) || (Declination <= (-10) && Declination > (-100)))
            {
                if (!ForceNewLocationToLabels)
                {
                    label81.Location = new Point(195, 246);
                }
                else
                {
                    label81.Location = new Point(395, 246);
                }
            }
            if ((Declination >= 100) || (Declination <= (-100)))
            {
                if (!ForceNewLocationToLabels)
                {
                    label81.Location = new Point(190, 246);
                }
                else
                {
                    label81.Location = new Point(390, 246);
                }
            }

            horizontalProgressBar21.Value = MemoryRamUsedPercent;

            foreach (var item in new HorizontalProgressBar2[] { horizontalProgressBar21, horizontalProgressBar22 })
            {
                if (item.Value <= 50)
                {
                    item.ValueColor = Color.Lime;
                }

                if (item.Value > 50)
                {
                    item.ValueColor = Color.Orange;
                }

                if (item.Value > 90)
                {
                    item.ValueColor = Color.Red;
                }
            }

            label151.Text = "Memoria Ram Livre:" + MemoryRamUsed + "KB de " + RamMemString;

            FlightModeToLabel(FlightMode);

            if (GetAccCalibFlag != 63)
            {
                AccNotCalibrated = true;
                RollToGraph.Add((double)xTimeStamp, 0);
            }
            else
            {
                AccNotCalibrated = false;
                RollToGraph.Add((double)xTimeStamp, -ReadRoll);
            }
            PitchToGraph.Add((double)xTimeStamp, -ReadPitch);
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
            ProgressBarControl2(ThrottleActualData, YawActualData, PitchActualData, RollActualData, Aux1ActualData, Aux2ActualData, Aux3ActualData,
                   Aux4ActualData, Aux5ActualData, Aux6ActualData, Aux7ActualData, Aux8ActualData);
            ProgressBarControl3(ThrottleAttitudeData, YawAttitudeData, PitchAttitudeData, RollAttitudeData);

            GetValues.SafeStateToLaunch = (FrameMode == 3 || FrameMode == 4 || FrameMode == 5) && ThrottleData >= 1400 && FlightMode == 3;

            if (GetAccCalibFlag != 63)
            {
                HUD1.Roll = 0;
                HUD1.Pitch = 0;
            }
            else
            {
                HUD1.Roll = -ReadRoll / 10;
                HUD1.Pitch = ReadPitch / 10;
            }
            HUD1.ARMStatus = CommandArmDisarm == 0 ? false : true;
            HUD1.FailSafe = FailSafeDetect == 1 ? true : false;
            HUD1.IMUHealty = GetAccCalibFlag != 63 && SerialPort.IsOpen ? true : false;
            HUD1.LinkQualityGCS = (float)CalculateAverage(PacketsReceived, PacketsError);
            HUD1.AHRSHorizontalVariance = HorizontalVariance();
            HUD1.ThrottleSafe = ThrottleActualData > 1250 ? true : false;
            HUD1.VelSpeed = GetValues.AirSpeedEnabled > 0 ? GetValues.ReadAirSpeed : GetValues.ReadGroundSpeed;

            if (GetAccCalibFlag != 63)
            {
                HUD2.Roll = 0;
                HUD2.Pitch = 0;
            }
            else
            {
                HUD2.Roll = -ReadRoll / 10;
                HUD2.Pitch = ReadPitch / 10;
            }
            HUD2.ARMStatus = CommandArmDisarm == 0 ? false : true;
            HUD2.FailSafe = FailSafeDetect == 1 ? true : false;
            HUD2.IMUHealty = GetAccCalibFlag != 63 && SerialPort.IsOpen ? true : false;
            HUD2.LinkQualityGCS = (float)CalculateAverage(PacketsReceived, PacketsError);
            HUD2.AHRSHorizontalVariance = HorizontalVariance();
            HUD2.ThrottleSafe = ThrottleActualData > 1300 ? true : false;
            HUD2.VelSpeed = GetValues.AirSpeedEnabled > 0 ? GetValues.ReadAirSpeed : GetValues.ReadGroundSpeed;

            if (GetAccCalibFlag != 63)
            {
                HUDSMALL1.roll = 0;
                HUDSMALL1.pitch = 0;
            }
            else
            {
                HUDSMALL1.roll = -ReadRoll / 10;
                HUDSMALL1.pitch = ReadPitch / 10;
            }
            HUDSMALL1.status = CommandArmDisarm;
            HUDSMALL1.failsafe = FailSafeDetect == 1 ? true : false;
            HUDSMALL1.imuhealty = GetAccCalibFlag != 63 && SerialPort.IsOpen ? true : false;
            HUDSMALL1.linkqualitygcs = (float)CalculateAverage(PacketsReceived, PacketsError);

            HeadingIndicator.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);
            HeadingIndicator2.SetHeadingIndicatorParameters(ReadCompass, SmallCompass);

            circularProgressBar1.Text = Convert.ToString(BattVoltage);
            circularProgressBar2.Text = Convert.ToString(BattVoltage);
            BattPercentage = ConstrainByte(BattPercentage, 0, 100);
            circularProgressBar1.Value = BattPercentage;
            circularProgressBar2.Value = BattPercentage;
            label2.Text = Convert.ToString(BattPercentage + "%");
            label3.Text = Convert.ToString(BattPercentage + "%");
            if (!ForceNewLocationToLabels)
            {
                label3.Location = new Point(65, 480);
            }
            else
            {
                label3.Location = new Point(235, 480);
            }
            if (SerialPort.IsOpen == true)
            {
                if (Reboot)
                {
                    Program.RebootBoard.Close();
                    Reboot = false;
                }
                else
                {
                    Program.WaitUart.Close();
                }
                label4.Text = "Habilitado";
                label5.Text = "Habilitado";
                label4.Location = new Point(17, 90);
                label5.Location = new Point(19, 89);
            }
            else
            {
                AutoWpPushed = false;
            }
        }

        byte ConstrainByte(byte amt, byte low, byte high)
        {
            return ((amt) < (low) ? (low) : ((amt) > (high) ? (high) : (amt)));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!SerialPort.IsOpen) return;
            SerialPort.DiscardInBuffer();
            SerialPort.Close();
            PacketsError = 0;
            PacketsReceived = 0;
            StringsChecked = false;
            comboBox7.Enabled = true;
            comboBox7.Text = "Selecione";
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
            if (GCSSettings.GCSRebootBoard > 0)
            {
                SerialPort.DtrEnable = true;
            }
            try
            {
                if (!Reboot)
                {
                    Program.WaitUart.Show();
                    WaitUart.Refresh();
                }
                else
                {
                    Program.RebootBoard.Show();
                    RebootBoard.Refresh();
                }
                SerialPort.PortName = SerialComPort;
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                SerialPort.BaudRate = 115200;
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                SerialPort.DataBits = 8;
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                SerialPort.Parity = Parity.None;
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                SerialPort.StopBits = StopBits.One;
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                SerialPort.Open();
                if (!Reboot)
                {
                    WaitUart.Refresh();
                }
                else
                {
                    RebootBoard.Refresh();
                }
                for (Int32 i = 0; i < 300; i++)
                {
                    if (!Reboot)
                    {
                        WaitUart.Refresh();
                    }
                    else
                    {
                        RebootBoard.Refresh();
                    }
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
                Program.RebootBoard.Close();
                Reboot = false;
            }
        }

        private void comboBox7_Click(object sender, EventArgs e)
        {
            comboBox7.Items.Clear();
            SerialPorts = SerialPort.GetPortNames();
            foreach (string PortsAvailable in SerialPorts) comboBox7.Items.Add(PortsAvailable);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SmallCompass = false;
            if (SerialPort.IsOpen == true)
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
            }
            tabControl1.SelectTab(tabPage1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as configurações com a JCFLIGHT em Voo!");
                    return;
                }
                ItsSafeToUpdate = false;
                if (PidAndFiltersCommunicationOpen == true)
                {
                    Serial_Write_To_FC(14);
                    PidAndFiltersCommunicationOpen = false;
                }
                Serial_Write_To_FC(13);
                SerialOpen = true;
                comboBox4.SelectedIndex = ((SimpleDataGuard > comboBox4.Items.Count) ? 0 : SimpleDataGuard);
                comboBox2.SelectedIndex = AltHoldGuard;
                comboBox3.SelectedIndex = GPSHoldGuard;
                comboBox5.SelectedIndex = RTHGuard;
                comboBox12.SelectedIndex = PPMGuard;
                comboBox13.SelectedIndex = GimbalGuard;
                comboBox11.SelectedIndex = FrameGuard;
                comboBox14.SelectedIndex = ParachuteGuard;
                comboBox1.SelectedIndex = AcroGuard;
                comboBox6.SelectedIndex = SportGuard;
                comboBox8.SelectedIndex = AutoFlipGuard;
                comboBox9.SelectedIndex = AutoGuard;
                comboBox15.SelectedIndex = OptFlowGuard;
                comboBox16.SelectedIndex = ((SonarGuard > comboBox16.Items.Count) ? 0 : SonarGuard);
                comboBox17.SelectedIndex = Uart1Guard;
                comboBox18.SelectedIndex = CompassRotGuard;
                numericUpDown86.Value = RthAltitudeGuard < 10 ? 10 : RthAltitudeGuard;
                comboBox24.SelectedIndex = SafeBtnGuard;
                comboBox25.SelectedIndex = AirSpeedGuard;
                comboBox10.SelectedIndex = ArmDisarmGuard;
                comboBox23.SelectedIndex = AutoLandGuard;
                numericUpDown94.Value = Math.Abs(PitchLevelTrimGuard) > 10 ? 0 : PitchLevelTrimGuard;
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
            if (SerialPort.IsOpen == true)
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
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
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
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button25_Click(null, null);
            if (SerialPort.IsOpen == true)
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
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SmallCompass = true;
            if (SerialPort.IsOpen == true)
            {
                if (!SerialPort.IsOpen)
                    MessageRead = false;
                else
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
                }
            }
            button20_Click(null, null);
            tabControl1.SelectTab(tabPage6);
            if (SerialPort.IsOpen)
            {
                if (!MessageRead)
                {
                    if (!PingTest.PingNetwork("pingtest.com"))
                    {
                        MyGMap.Manager.Mode = AccessMode.CacheOnly;
                        MessageBox.Show("Você está sem internet,o mapa irá funcinar em modo cache,partes do mapa não carregados antes com internet podem falhar.", "Teste de conexão com a internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    MessageRead = true;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!SerialPort.IsOpen) return;
            Serial_Write_To_FC(11);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!SerialPort.IsOpen) return;
            if ((DevicesSum & 1) == 0)
            {
                MessageBox.Show("Compass não encontrado no barramento I2C da controladora de voo.");
                return;
            }
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
                label157.Text = Convert.ToString("Lat:" + GPS_Position.Lat);
                label158.Text = Convert.ToString("Lng:" + GPS_Position.Lng);
                int TrackLength = GCSSettings.GCSTrackLength;
                Pen PenRoute = new Pen(Color.Purple, GCSSettings.GCSTrackSize);
                Grout.Stroke = PenRoute;
                if (Grout.Points.Count > TrackLength) Grout.Points.RemoveRange(0, Grout.Points.Count - TrackLength);
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
                    int ExpoValue = 0;
                    int AttitudeRoll = -ReadRoll / 10;
                    if (GetAccCalibFlag != 63)
                    {
                        AttitudeRoll = 0;
                    }
                    if (AttitudeRoll >= 10 && AttitudeRoll < 35) ExpoValue = 150;
                    if (AttitudeRoll <= -10 && AttitudeRoll > -35) ExpoValue = -150;
                    if (AttitudeRoll >= 35 && AttitudeRoll < 45) ExpoValue = 50;
                    if (AttitudeRoll <= -35 && AttitudeRoll > -45) ExpoValue = -50;
                    if (AttitudeRoll >= 45) ExpoValue = 25;
                    if (AttitudeRoll <= -45) ExpoValue = -25;
                    PositionToRoutes.Markers.Add(new GMapMarkerPlane(GPS_Position, ReadCompass, CoG, Crosstrack, ExpoValue));
                }
                if (HomePointDisctance >= 1000 && HomePointDisctance < 10000)
                {
                    if (!ForceNewLocationToLabels)
                    {
                        label74.Location = new Point(190, 32);
                    }
                    else
                    {
                        label74.Location = new Point(390, 32);
                    }
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else if (HomePointDisctance >= 10000 && HomePointDisctance < 100000)
                {
                    if (!ForceNewLocationToLabels)
                    {
                        label74.Location = new Point(185, 32);
                    }
                    else
                    {
                        label74.Location = new Point(385, 32);
                    }
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else if (HomePointDisctance >= 100000)
                {
                    if (!ForceNewLocationToLabels)
                    {
                        label74.Location = new Point(180, 32);
                    }
                    else
                    {
                        label74.Location = new Point(380, 32);
                    }
                    label74.Text = HomePointDisctance.ToString(new CultureInfo("en-US")) + "KM";
                }
                else
                {
                    if (!ForceNewLocationToLabels)
                    {
                        label74.Location = new Point(208, 32);
                    }
                    else
                    {
                        label74.Location = new Point(408, 32);
                    }
                    label74.Text = Convert.ToInt32(HomePointDisctance) + "M";
                }

                if (CommandArmDisarm == 1) Grout.Points.Add(GPS_Position);
                if (checkBox1.Checked == true)
                {
                    MyGMap.Position = GPS_Position;
                }
                MyGMap.Invalidate();
            }
        }

        private void FlightModeToLabel(byte _FlightMode)
        {
            switch (_FlightMode)
            {
                case 0: //ACRO
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(188, 296);
                    }
                    else
                    {
                        label83.Location = new Point(388, 296);
                    }
                    label83.Text = "ACRO";
                    break;

                case 1: //STABILIZE
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(174, 296);
                    }
                    else
                    {
                        label83.Location = new Point(374, 296);
                    }
                    label83.Text = "STABILIZE";
                    break;

                case 2: //ALT-HOLD
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(175, 296);
                    }
                    else
                    {
                        label83.Location = new Point(375, 296);
                    }
                    label83.Text = "ALT-HOLD";
                    break;

                case 3: //ATAQUE              
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(179, 296);
                        }
                        else
                        {
                            label83.Location = new Point(379, 296);
                        }
                        label83.Text = "ATAQUE";
                    }
                    else
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(172, 296);
                        }
                        else
                        {
                            label83.Location = new Point(372, 296);
                        }
                        label83.Text = "TAKE-OFF";
                    }
                    break;

                case 4: //POS-HOLD
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(175, 296);
                        }
                        else
                        {
                            label83.Location = new Point(375, 296);
                        }
                        label83.Text = "POS-HOLD";
                    }
                    else
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(184, 296);
                        }
                        else
                        {
                            label83.Location = new Point(382, 296);
                        }
                        label83.Text = "CÍRCULO";
                    }
                    break;

                case 5: //MODO SIMPLES
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(202, 296);
                        }
                        else
                        {
                            label83.Location = new Point(402, 296);
                        }
                        label83.Text = "SIMPLES";
                    }
                    else
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(184, 296);
                        }
                        else
                        {
                            label83.Location = new Point(384, 296);
                        }
                        label83.Text = "MANUAL";
                    }
                    break;

                case 6: //RTH
                case 7:
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(200, 296);
                    }
                    else
                    {
                        label83.Location = new Point(400, 296);
                    }
                    label83.Text = "RTH";
                    break;

                case 8: //LAND
                case 9:
                case 10:
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(192, 296);
                        }
                        else
                        {
                            label83.Location = new Point(392, 296);
                        }
                        label83.Text = "LAND";
                    }
                    else
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(188, 296);
                        }
                        else
                        {
                            label83.Location = new Point(388, 296);
                        }
                        label83.Text = "CRUISE";
                    }
                    break;

                case 11: //FLIP
                    if (FrameMode < 3 || FrameMode == 6 || FrameMode == 7)
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(198, 296);
                        }
                        else
                        {
                            label83.Location = new Point(398, 296);
                        }
                        label83.Text = "FLIP";
                    }
                    else
                    {
                        if (!ForceNewLocationToLabels)
                        {
                            label83.Location = new Point(162, 296);
                        }
                        else
                        {
                            label83.Location = new Point(362, 296);
                        }
                        label83.Text = "TURN-COORD";
                    }
                    break;

                case 12: //AUTO
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(192, 296);
                    }
                    else
                    {
                        label83.Location = new Point(392, 296);
                    }
                    label83.Text = "AUTO";
                    break;

                case 13: //LANDED
                    if (!ForceNewLocationToLabels)
                    {
                        label83.Location = new Point(165, 296);
                    }
                    else
                    {
                        label83.Location = new Point(365, 296);
                    }
                    label83.Text = "ATERRIZADO";
                    break;
            }
        }

        private void FlightTimer_Tick(object sender, EventArgs e)
        {
            if (GCSSettings.GCSSpeech > 0)
            {
                SpeechRun();
            }

            if (GCSSettings.GCSAirPorts > 0)
            {
                if (AirportsCountTime >= 5)
                {
                    AirportsOverlay.Markers.Clear();
                    foreach (var item in Airports.getAirports(MyGMap.Position).ToArray())
                    {
                        try
                        {
                            AirportsOverlay.Markers.Add(new GMapMarkerAirport(item)
                            {
                                ToolTipText = item.Tag,
                                ToolTipMode = MarkerTooltipMode.OnMouseOver
                            });
                        }
                        catch (Exception)
                        {
                        }
                    }
                    AirportsCountTime = 0;
                }
                else
                {
                    AirportsCountTime += 1;
                }
            }

            if (FrameMode == 3 || FrameMode == 4 || FrameMode == 5)
            {
                groupBox6.Text = "FEED-FORWARD";
                numericUpDown94.Enabled = true;
                numericUpDown32.Enabled = false;
                numericUpDown33.Enabled = false;
                numericUpDown34.Enabled = false;
                numericUpDown38.Enabled = true;
                numericUpDown77.Enabled = true;
                label52.Text = "Auto-Throttle";
                metroLabel15.Text = "Ganho ";
                metroLabel14.Text = "Vel. Minima";
                metroLabel38.Text = "Raio do Círculo";
                metroLabel42.Visible = true;
                label53.Text = "Navegação XY";
                label54.Text = "Navegação Heading";
            }
            else
            {
                groupBox6.Text = "CONTROLE DERIVATIVO";
                numericUpDown94.Enabled = false;
                numericUpDown32.Enabled = true;
                numericUpDown33.Enabled = true;
                numericUpDown34.Enabled = true;
                numericUpDown38.Enabled = false;
                numericUpDown77.Enabled = false;
                label52.Text = "Posição";
                metroLabel15.Text = "Proporcional";
                metroLabel14.Text = "Integral";
                metroLabel38.Text = "Ataque Bank";
                metroLabel42.Visible = false;
                label53.Text = "Rate de Posição";
                label54.Text = "Rate de Navegação";
            }

            try
            {
                if (SerialPort.IsOpen == false) return;
            }
            catch { }

            if (SerialPort.BytesToRead == 0)
            {
                if (ItsSafeToUpdate)
                {
                    Serial_Write_To_FC(27);
                }
            }

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

        private void button8_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SendConfigurationsToJCFLIHGT(SerialPort, 1);
                Thread.Sleep(150);
                Serial_Write_To_FC(16);
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
                             "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
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
                    comboBox14.SelectedIndex = 0;
                    comboBox1.SelectedIndex = 0;
                    comboBox6.SelectedIndex = 0;
                    comboBox8.SelectedIndex = 0;
                    comboBox9.SelectedIndex = 0;
                    comboBox15.SelectedIndex = 0;
                    comboBox16.SelectedIndex = 0;
                    comboBox17.SelectedIndex = 0;
                    comboBox18.SelectedIndex = 0;
                    numericUpDown86.Value = 10;
                    comboBox10.SelectedIndex = 0;
                    comboBox23.SelectedIndex = 0;
                    comboBox24.SelectedIndex = 0;
                }
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
              "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAcro = Convert.ToByte(comboBox1.SelectedIndex);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxSimpleMode = Convert.ToByte(comboBox4.SelectedIndex);
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
            ComboBoxUart1 = Convert.ToByte(comboBox17.SelectedIndex);
        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxCompassRot = Convert.ToByte(comboBox18.SelectedIndex);
        }

        private void comboBox24_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxSafeBtn = Convert.ToByte(comboBox24.SelectedIndex);
        }

        private void comboBox25_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAirSpeed = Convert.ToByte(comboBox25.SelectedIndex);
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

        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxAutoLand = Convert.ToByte(comboBox23.SelectedIndex);
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
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as configurações com a JCFLIGHT em Voo!");
                    return;
                }

                if (PidAndFiltersCommunicationOpen) return;
                if (SerialOpen == true)
                {
                    Serial_Write_To_FC(14);
                    SerialOpen = false;
                }
                ItsSafeToUpdate = false;
                if (SerialPort.IsOpen == true)
                {
                    Serial_Write_To_FC(13);
                    PidAndFiltersCommunicationOpen = true;
                }
                numericUpDown19.Value = TPABreakPoint;
                numericUpDown18.Value = TPAFactor;
                comboBox20.SelectedIndex = GyroLPF;
                numericUpDown14.Value = DerivativeLPF;
                numericUpDown20.Value = RCSmooth;
                numericUpDown21.Value = ServosLPF;
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
                numericUpDown34.Value = (decimal)(NumericConvert[9]) / 10;
                numericUpDown11.Value = (decimal)(NumericConvert[10]) / 100;
                numericUpDown12.Value = (decimal)(NumericConvert[11]) / 100;
                numericUpDown68.Value = (decimal)(NumericConvert[12]) / 10;
                numericUpDown69.Value = (decimal)(NumericConvert[13]) / 10;
                numericUpDown31.Value = (decimal)(NumericConvert[14]) / 10;
                numericUpDown71.Value = (decimal)(NumericConvert[15]) / 10;
                numericUpDown72.Value = NumericConvert[16];
                numericUpDown70.Value = (decimal)(NumericConvert[17]) / 10;
                numericUpDown73.Value = NumericConvert[18];
                numericUpDown74.Value = NumericConvert[19];
                numericUpDown37.Value = -NumericConvert[20];
                numericUpDown29.Value = NumericConvert[21];
                numericUpDown75.Value = NumericConvert[22];
                numericUpDown76.Value = NumericConvert[23];
                numericUpDown91.Value = Integral_Relax_LPF;
                numericUpDown87.Value = kCD_or_FF_LPF;
                numericUpDown33.Value = (decimal)(NumericConvert[24]) / 1000;
                numericUpDown32.Value = (decimal)(NumericConvert[25]);
                numericUpDown10.Value = (decimal)(NumericConvert[26]) / 10;
                numericUpDown38.Value = (decimal)(NumericConvert[27]) / 1000;
                numericUpDown77.Value = (decimal)(NumericConvert[28]);
                numericUpDown80.Value = (decimal)(NumericConvert[29]) / 10;
                numericUpDown81.Value = (decimal)(NumericConvert[30]) / 1000;
                numericUpDown79.Value = (decimal)(NumericConvert[31]);
                numericUpDown83.Value = (decimal)(NumericConvert[32]) / 10;
                numericUpDown85.Value = (decimal)(NumericConvert[33]) / 1000;
                numericUpDown82.Value = (decimal)(NumericConvert[34]);
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

        private void CheckCompassState(int HeadingCompass)
        {
            //COMPASS
            if (HeadingCompass != HeadingCompassPrev)
            {
                CompassHealthCount = 0;
                HeadingCompassPrev = HeadingCompass;
            }
            else
            {
                CompassHealthCount++;
            }
            if (CompassHealthCount >= 4000) //4 SEGUNDOS
            {
                HUD1.CompassHealty = true;
                HUD2.CompassHealty = true;
            }
            else
            {
                HUD1.CompassHealty = false;
                HUD2.CompassHealty = false;
            }
        }

        private void Edit_Labels_To_Aero()
        {
            if (ComboBoxFrame == 0 || ComboBoxFrame == 1 || ComboBoxFrame == 2 || ComboBoxFrame == 6 || ComboBoxFrame == 7) //QUAD & HEXA
            {
                label22.Text = "Simples";
                label44.Text = "> Mantém a borda de ataque sempre pra frente";
                label23.Text = "Ataque";
                label46.Text = "> Estabilização com limite de ângulo maior";
                label20.Text = "Altitude-Hold";
                label42.Text = "> Retenção de Altitude com base no Barômetro e INS";
                label21.Text = "Pos-Hold";
                label43.Text = "> Retenção de Posição com base no GPS e INS";
                label24.Text = "Auto-Flip";
                label48.Text = "> Realiza Flips Automáticos de 180° no Pitch e Roll";
                label92.Text = "Auto-Land";
                label70.Text = "> Realiza um pouso automático";
            }
            else if (ComboBoxFrame == 3 || ComboBoxFrame == 4 || ComboBoxFrame == 5) //AERO, ASA-FIXA & V-TAIL
            {
                label22.Text = "Manual";
                label44.Text = "> Servos independentes do controlador PID";
                label23.Text = "Auto-TakeOff";
                label46.Text = "> Lançamento Automático para Aeros e Asa";
                label20.Text = "Altitude-Hold";
                label42.Text = "> Mantém a velocidade e a altitude constante";
                label21.Text = "Auto-Círculo";
                label43.Text = "> Mantém a posição e a altitude do Aero em Círculo";
                label24.Text = "Turn-Coord.";
                label48.Text = "> Curva Coordenada baseada na Veloc. da Fuselagem";
                label92.Text = "Cruzeiro";
                label70.Text = "> Mantém a posição e a altitude do Aero em linha reta";
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
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
             "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
                }
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
                    numericUpDown34.Value = (decimal)(50) / 10;
                    numericUpDown33.Value = (decimal)(20) / 1000;
                    numericUpDown32.Value = (decimal)(16);
                    numericUpDown11.Value = (decimal)(100) / 100;
                    numericUpDown12.Value = (decimal)(90) / 100;
                    numericUpDown18.Value = 0;
                    numericUpDown19.Value = 1500;
                    comboBox20.SelectedIndex = 0;
                    numericUpDown14.Value = 40;
                    numericUpDown20.Value = 50;
                    numericUpDown21.Value = 50;
                    comboBox21.SelectedIndex = 0;
                    numericUpDown13.Value = 15;
                    numericUpDown15.Value = 60;
                    numericUpDown16.Value = 0;
                    numericUpDown17.Value = 0;
                    numericUpDown94.Value = 0;
                    comboBox22.SelectedIndex = 0;
                    numericUpDown91.Value = 15;
                    numericUpDown87.Value = 30;
                    numericUpDown10.Value = (decimal)(0) / 10;
                    numericUpDown38.Value = (decimal)(50) / 1000;
                    numericUpDown77.Value = (decimal)(20);
                    if (FrameMode == 3 || FrameMode == 4 || FrameMode == 5)
                    {
                        numericUpDown1.Value = (decimal)(5) / 10;
                        numericUpDown2.Value = (decimal)(7) / 1000;
                        numericUpDown3.Value = 0;
                        numericUpDown4.Value = (decimal)(5) / 10;
                        numericUpDown5.Value = (decimal)(7) / 1000;
                        numericUpDown6.Value = 0;
                        numericUpDown7.Value = (decimal)(6) / 10;
                        numericUpDown8.Value = (decimal)(10) / 1000;
                        numericUpDown9.Value = 0;
                        numericUpDown68.Value = (decimal)(50) / 10;
                        numericUpDown69.Value = (decimal)(50) / 10;
                        numericUpDown31.Value = (decimal)(60) / 10;
                        numericUpDown71.Value = (decimal)(20) / 10;
                        numericUpDown72.Value = 5;
                        numericUpDown80.Value = (decimal)(75) / 10;
                        numericUpDown81.Value = (decimal)(5) / 1000;
                        numericUpDown79.Value = (decimal)(8);
                        numericUpDown83.Value = (decimal)(30) / 10;
                        numericUpDown85.Value = (decimal)(2) / 1000;
                        numericUpDown82.Value = (decimal)(0);
                    }
                    else
                    {
                        numericUpDown1.Value = (decimal)(40) / 10;
                        numericUpDown2.Value = (decimal)(30) / 1000;
                        numericUpDown3.Value = 23;
                        numericUpDown4.Value = (decimal)(40) / 10;
                        numericUpDown5.Value = (decimal)(30) / 1000;
                        numericUpDown6.Value = 23;
                        numericUpDown7.Value = (decimal)(85) / 10;
                        numericUpDown8.Value = (decimal)(45) / 1000;
                        numericUpDown9.Value = 0;
                        numericUpDown68.Value = (decimal)(60) / 10;
                        numericUpDown69.Value = (decimal)(60) / 10;
                        numericUpDown31.Value = (decimal)(60) / 10;
                        numericUpDown71.Value = (decimal)(20) / 10;
                        numericUpDown72.Value = 15;
                        numericUpDown80.Value = (decimal)(70) / 10;
                        numericUpDown81.Value = (decimal)(20) / 1000;
                        numericUpDown79.Value = (decimal)(20);
                        numericUpDown83.Value = (decimal)(25) / 10;
                        numericUpDown85.Value = (decimal)(33) / 1000;
                        numericUpDown82.Value = (decimal)(83);
                    }
                    numericUpDown70.Value = (decimal)(60) / 10;
                    numericUpDown73.Value = 90;
                    if (FrameMode == 3 || FrameMode == 4 || FrameMode == 5)
                    {
                        numericUpDown74.Value = 45;
                        numericUpDown37.Value = -25;
                        numericUpDown29.Value = 20;
                        numericUpDown75.Value = 40;
                        numericUpDown76.Value = 30;
                    }
                    else
                    {
                        numericUpDown74.Value = 30;
                        numericUpDown37.Value = -30;
                        numericUpDown29.Value = 30;
                        numericUpDown75.Value = 40;
                        numericUpDown76.Value = 30;

                    }
                }
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
              "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
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

        void UpdateComboBoxSum()
        {
            if ((ChannelsReverse & 1) > 0)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }

            if ((ChannelsReverse & 2) > 0)
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }

            if ((ChannelsReverse & 4) > 0)
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }

            if ((ChannelsReverse & 8) > 0)
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }

            if ((ServosReverse & 1) > 0)
            {
                checkBox9.Checked = true;
            }
            else
            {
                checkBox9.Checked = false;
            }

            if ((ServosReverse & 2) > 0)
            {
                checkBox8.Checked = true;
            }
            else
            {
                checkBox8.Checked = false;
            }

            if ((ServosReverse & 4) > 0)
            {
                checkBox7.Checked = true;
            }
            else
            {
                checkBox7.Checked = false;
            }

            if ((ServosReverse & 8) > 0)
            {
                checkBox6.Checked = true;
            }
            else
            {
                checkBox6.Checked = false;
            }
        }

        private void GCS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SerialPort.IsOpen) SerialPort.Close();
            Environment.Exit(0);
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
                    SendBuffer[VectorPointer++] = 23;
                    SendBuffer[VectorPointer++] = (byte)15;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxFrame;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxPPM;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxGimbal;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxParachute;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSPI;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxUART2;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxUart1;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxCompassRot;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown86.Value;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAcro;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAltHold;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxGPSHold;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSimpleMode;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxRTH;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSport;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAutoFlip;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAuto;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxArmDisarm;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAutoLand;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxSafeBtn;
                    SendBuffer[VectorPointer++] = (byte)ComboBoxAirSpeed;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown94.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown94.Value) >> 8);
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
                    SendBuffer[VectorPointer++] = 59;
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
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown34.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown11.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown12.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown21.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown21.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown68.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown69.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown31.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown71.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown72.Value;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown70.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown73.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown74.Value;
                    SendBuffer[VectorPointer++] = (byte)Math.Abs(numericUpDown37.Value);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown29.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown75.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown76.Value;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown91.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown91.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown87.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown87.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown33.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown32.Value;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown10.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown38.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown77.Value);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown80.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown81.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown79.Value);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown83.Value * 10);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown85.Value * 1000);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown82.Value);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    SerialPort.Write(SendBuffer, 0, VectorPointer);
                }
                else if (ConfigType == 3)
                {
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4a;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x3c;
                    SendBuffer[VectorPointer++] = 34;
                    SendBuffer[VectorPointer++] = (byte)31;
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown25.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown26.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown27.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown28.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(numericUpDown30.Value * 100);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown35.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown35.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown36.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown36.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown62.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown62.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown61.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown61.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown60.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown60.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown59.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown59.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown58.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown58.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown57.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown57.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown56.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown56.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown55.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown55.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown66.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown65.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown64.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown63.Value;
                    SendBuffer[VectorPointer++] = (byte)GetChannelsReverse();
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown67.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown67.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)numericUpDown89.Value;
                    SendBuffer[VectorPointer++] = (byte)numericUpDown90.Value;
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    SerialPort.Write(SendBuffer, 0, VectorPointer);
                }
                else if (ConfigType == 4)
                {
                    VectorPointer = 0;
                    CheckAllBuffers = 0;
                    SendBuffer[VectorPointer++] = (byte)0x4a;
                    SendBuffer[VectorPointer++] = (byte)0x43;
                    SendBuffer[VectorPointer++] = (byte)0x3c;
                    SendBuffer[VectorPointer++] = 33;
                    SendBuffer[VectorPointer++] = (byte)33;
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown54.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown54.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown53.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown53.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown52.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown52.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown51.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown51.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)GetServosReverse();
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown41.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown41.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown39.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown39.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown40.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown40.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown42.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown42.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown43.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown43.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown44.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown44.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown45.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown45.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown46.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown46.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown50.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown50.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown49.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown49.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown48.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown48.Value) >> 8);
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown47.Value));
                    SendBuffer[VectorPointer++] = (byte)(Convert.ToInt16(numericUpDown47.Value) >> 8);
                    for (int i = 3; i < VectorPointer; i++) CheckAllBuffers ^= SendBuffer[i];
                    SendBuffer[VectorPointer++] = CheckAllBuffers;
                    SerialPort.Write(SendBuffer, 0, VectorPointer);
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (GCSConfigurations.CloseGCSNow)
            {
                this.Close();
            }
            if (SerialPort.IsOpen == false)
            {
                return;
            }
            if (tabControl1.SelectedIndex == 0)
            {
                SmallCompass = false;
            }
            if (tabControl1.SelectedIndex == 5)
            {
                SmallCompass = true;
            }
            horizontalProgressBar22.Value = CPULoad;
        }

        private void UpdateAccImageStatus()
        {
            if ((1 & (GetAccCalibFlag >> 0)) > 0) pictureBox10.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 1)) > 0) pictureBox13.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 2)) > 0) pictureBox17.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 3)) > 0) pictureBox15.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 4)) > 0) pictureBox21.BackColor = Color.Green;
            if ((1 & (GetAccCalibFlag >> 5)) > 0) pictureBox19.BackColor = Color.Green;
        }

        private void limparMapaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grout.Points.Clear();
        }

        double Latitude_For_FC = 0;
        double Longitude_For_FC = 0;
        private void MyGMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                var WayPoint = MyGMap.FromLocalToLatLng(e.X, e.Y);
                Latitude_For_FC = WayPoint.Lat;
                Longitude_For_FC = WayPoint.Lng;
            }
        }

        private void decolarToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void button19_Click(object sender, EventArgs e)
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
            FirmwareUpdate HexOpen = new FirmwareUpdate();
            HexOpen.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
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
            }
            SmallCompass = false;
            tabControl1.SelectTab(tabPage8);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (SmallFlightData)
            {
                button20.Text = "Ampliar Tudo";
                button20.Location = new Point(19, 540);
                if (this.WindowState == FormWindowState.Normal)
                {
                    MyGMap.Size = new Size(740, 555);
                }
                else
                {
                    MyGMap.Size = new Size(800, 555);
                }
                HeadingIndicator2.Location = new Point(0, 186);
                circularProgressBar2.Location = new Point(0, 373);
                btnlogoInicio.Location = new Point(257, 22);
                panel18.Location = new Point(819, 38);
                label72.Location = new Point(5, 98);
                panel1.Location = new Point(250, 137);
                preArmNotification2.Visible = false;
                MyGMap.Location = new Point(298, 6);
                label73.Location = new Point(170, 16);
                label74.Location = new Point(208, 32);
                label75.Location = new Point(168, 73);
                label76.Location = new Point(215, 89);
                label77.Location = new Point(200, 123);
                label78.Location = new Point(196, 140);
                label80.Location = new Point(199, 174);
                label79.Location = new Point(203, 190);
                label82.Location = new Point(192, 231);
                label81.Location = new Point(215, 246);
                label84.Location = new Point(185, 281);
                label83.Location = new Point(184, 296);
                label86.Location = new Point(180, 334);
                label85.Location = new Point(184, 349);
                label88.Location = new Point(192, 389);
                label87.Location = new Point(206, 404);
                label117.Location = new Point(156, 435);
                label116.Location = new Point(193, 450);
                label90.Location = new Point(204, 481);
                label89.Location = new Point(208, 496);
                label91.Location = new Point(176, 527);
                metroTrackBar1.Location = new Point(143, 537);
                MenuVertical.Width = 249;
                MenuVertical.Height = 710;
                ForceNewLocationToLabels = false;
                HUDSMALL1.Visible = true;
                HUD2.Visible = false;
            }
            else
            {
                button20.Text = "Diminuir";
                button20.Location = new Point(120, 540);
                if (this.WindowState == FormWindowState.Normal)
                {
                    MyGMap.Size = new Size(790, 555);
                }
                else
                {
                    MyGMap.Size = new Size(850, 555);
                }
                HeadingIndicator2.Location = new Point(0, 379);
                circularProgressBar2.Location = new Point(170, 373);
                btnlogoInicio.Location = new Point(514, 22);
                panel18.Location = new Point(1065, 38);
                label72.Location = new Point(250, 98);
                panel1.Location = new Point(0, 137);
                preArmNotification2.Visible = true;
                preArmNotification2.Location = new Point(2, 0);
                MyGMap.Location = new Point(450, 6);
                MyGMap.Location = new Point(498, 6);
                label73.Location = new Point(370, 16);
                label74.Location = new Point(408, 32);
                label75.Location = new Point(368, 73);
                label76.Location = new Point(415, 89);
                label77.Location = new Point(400, 123);
                label78.Location = new Point(396, 140);
                label80.Location = new Point(399, 174);
                label79.Location = new Point(403, 190);
                label82.Location = new Point(392, 231);
                label81.Location = new Point(415, 246);
                label84.Location = new Point(385, 281);
                label83.Location = new Point(384, 296);
                label86.Location = new Point(380, 334);
                label85.Location = new Point(384, 349);
                label88.Location = new Point(392, 389);
                label87.Location = new Point(406, 404);
                label117.Location = new Point(356, 435);
                label116.Location = new Point(393, 450);
                label90.Location = new Point(404, 481);
                label89.Location = new Point(408, 496);
                label91.Location = new Point(376, 527);
                metroTrackBar1.Location = new Point(343, 537);
                MenuVertical.Width = 0;
                MenuVertical.Height = 710;
                ForceNewLocationToLabels = true;
                HUDSMALL1.Visible = false;
                HUD2.Visible = true;
            }
            SmallFlightData = !SmallFlightData;
        }

        bool HorizontalVariance()
        {
            if (InertialSensor.get_vibration_level_X() > 30)
            {
                return true;
            }
            return false;
        }

        private void MyGMap_OnMapZoomChanged()
        {
            metroTrackBar1.Value = (int)(MyGMap.Zoom * 10);
        }

        private void MyGMap_Resize(object sender, EventArgs e)
        {
            MyGMap.Zoom = MyGMap.Zoom + 0.01;
        }

        private void metroTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (MyGMap.MaxZoom + 1 == (double)(metroTrackBar1.Value / 10))
                {
                    MyGMap.Zoom = (double)(metroTrackBar1.Value / 10) - 0.1;
                }
                else
                {
                    MyGMap.Zoom = (double)(metroTrackBar1.Value / 10);
                }
            }
            catch
            {
            }
        }

        private void carregarWPsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SafeToPushParams = true;
            BlockPushParams = false;
            ParamsPushed = false;
            PrintArea2 = false;
            CountToBlock = 0;
            CountWP2 = 0;
            GPS_Position2.Lat = 0;
            GPS_Position2.Lng = 0;
        }

        private void limparWPsDoMapaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarkersOverlay.Markers.Clear();
            GmapPolygons.Polygons.Clear();
            GmapPolygons.Clear();
            for (int i = 0; i < 100; i++)
            {
                PushedLatitude[i] = 0;
                PushedLongitude[i] = 0;
            }
            SafeToPushParams = false;
            BlockPushParams = false;
            ParamsPushed = false;
            PrintArea2 = false;
            CountToBlock = 0;
            CountWP2 = 0;
            GPS_Position2.Lat = 0;
            GPS_Position2.Lng = 0;
        }

        private void PushWayPointCoordinatesOfFlightController()
        {
            if (!SafeToPushParams) return;
            List<PointLatLng> WPCoordinatesToPush = new List<PointLatLng>();
            if (SerialPort.IsOpen)
            {
                Program.WaitUart.Close();
                if (SerialPort.BytesToRead == 0)
                {
                    if (!ParamsPushed)
                    {
                        Serial_Write_To_FC(1);
                    }

                    if (ParamsPushed)
                    {
                        if (PushedLatitude[0] != 0 && PushedLongitude[0] != 0 && !PrintArea2 && CountWP2 == 0)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[0];
                            GPS_Position2.Lng = PushedLongitude[0];
                        }

                        if (PushedLatitude[1] != 0 && PushedLongitude[1] != 0 && !PrintArea2 && CountWP2 == 1)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[1];
                            GPS_Position2.Lng = PushedLongitude[1];
                        }

                        if (PushedLatitude[2] != 0 && PushedLongitude[2] != 0 && !PrintArea2 && CountWP2 == 2)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[2];
                            GPS_Position2.Lng = PushedLongitude[2];
                        }

                        if (PushedLatitude[3] != 0 && PushedLongitude[3] != 0 && !PrintArea2 && CountWP2 == 3)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[3];
                            GPS_Position2.Lng = PushedLongitude[3];
                        }

                        if (PushedLatitude[4] != 0 && PushedLongitude[4] != 0 && !PrintArea2 && CountWP2 == 4)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[4];
                            GPS_Position2.Lng = PushedLongitude[4];
                        }

                        if (PushedLatitude[5] != 0 && PushedLongitude[5] != 0 && !PrintArea2 && CountWP2 == 5)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[5];
                            GPS_Position2.Lng = PushedLongitude[5];
                        }

                        if (PushedLatitude[6] != 0 && PushedLongitude[6] != 0 && !PrintArea2 && CountWP2 == 6)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[6];
                            GPS_Position2.Lng = PushedLongitude[6];
                        }

                        if (PushedLatitude[7] != 0 && PushedLongitude[7] != 0 && !PrintArea2 && CountWP2 == 7)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[7];
                            GPS_Position2.Lng = PushedLongitude[7];
                        }

                        if (PushedLatitude[8] != 0 && PushedLongitude[8] != 0 && !PrintArea2 && CountWP2 == 8)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[8]), Convert.ToDouble(PushedLongitude[8])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[8];
                            GPS_Position2.Lng = PushedLongitude[8];
                        }

                        if (PushedLatitude[9] != 0 && PushedLongitude[9] != 0 && !PrintArea2 && CountWP2 == 9)
                        {
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[0]), Convert.ToDouble(PushedLongitude[0])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[1]), Convert.ToDouble(PushedLongitude[1])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[2]), Convert.ToDouble(PushedLongitude[2])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[3]), Convert.ToDouble(PushedLongitude[3])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[4]), Convert.ToDouble(PushedLongitude[4])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[5]), Convert.ToDouble(PushedLongitude[5])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[6]), Convert.ToDouble(PushedLongitude[6])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[7]), Convert.ToDouble(PushedLongitude[7])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[8]), Convert.ToDouble(PushedLongitude[8])));
                            WPCoordinatesToPush.Add(new PointLatLng(Convert.ToDouble(PushedLatitude[9]), Convert.ToDouble(PushedLongitude[9])));
                            PrintArea2 = true;
                            GPS_Position2.Lat = PushedLatitude[9];
                            GPS_Position2.Lng = PushedLongitude[9];
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
                                FirstPointTrace.Points.Add(WPCoordinatesToPush[0]);
                                FirstPointTrace.Points.Add(WPCoordinatesToPush[WPCoordinatesToPush.Count - 1]);
                            }
                            GMapRoute WPLineRoute = new GMapRoute("WPLineRoute");
                            WPLineRoute.Stroke = new Pen(Color.Green, 4);
                            WPLineRoute.Stroke.DashStyle = DashStyle.Custom;
                            for (int a = 0; a < WPCoordinatesToPush.Count; a++) WPLineRoute.Points.Add(WPCoordinatesToPush[a]);
                            GmapPolygons.Routes.Add(FirstPointTrace);
                            GmapPolygons.Routes.Add(WPLineRoute);
                            MyGMap.Overlays.Add(GmapPolygons);
                            PrintArea2 = false;
                        }
                        CountToBlock++;
                        if (CountToBlock > 80)
                        {
                            SafeToPushParams = false;
                            BlockPushParams = true;
                            ParamsPushed = false;
                            CountToBlock = 151;
                        }
                    }
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (BlackBoxRunning)
            {
                CloseBlackBoxData();
                button21.Text = "Iniciar gravação da Caixa-Preta";
                button21.BackColor = Color.Lime;
            }
            else
            {
                RunBlackBoxData();
                if (BlackBoxRunning)
                {
                    button21.Text = "Parar gravação da Caixa-Preta";
                    button21.BackColor = Color.Red;
                }
            }
        }

        void RunBlackBoxData()
        {
            try
            {
                BlackBoxStream = new StreamWriter(Directory.GetCurrentDirectory() + "\\Caixa Preta" + "\\JCFLIGHT CAIXAPRETA" + String.Format(" - {0:dd MM yyyy - hh mm}.log", DateTime.Now));
            }
            catch
            {
                MessageBox.Show("Não foi possivel encontrar a pasta" + Directory.GetCurrentDirectory() + "\\Caixa Preta" + "\\JCFLIGHT_CAIXAPRETA" + String.Format("-{0:yyyyMMdd-hhmm}.log", DateTime.Now), "Erro ao tentar abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (BlackBoxStream != null)
            {
                BlackBoxRunning = true;
                BlackBoxStream.WriteLine("JCFLIGHT CAIXAPRETA - Iniciada em:{0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"));
            }
        }

        void CloseBlackBoxData()
        {
            BlackBoxStream.Flush();
            BlackBoxStream.Close();
            BlackBoxStream.Dispose();
            BlackBoxRunning = false;
        }

        void UpdateBlackBoxData()
        {
            BlackBoxStream.WriteLine("IMU,{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString("HH:mm:ss.fff"), GetValues.AccX, GetValues.AccY, GetValues.AccZ, GetValues.GyroX, GetValues.GyroY, GetValues.GyroZ);
            BlackBoxStream.WriteLine("MAG,{0},{1},{2},{3}", DateTime.Now.ToString("HH:mm:ss.fff"), GetValues.CompassX, GetValues.CompassY, GetValues.CompassZ);
            BlackBoxStream.WriteLine("ATTITUDE,{0},{1},{2},{3},{4}", DateTime.Now.ToString("HH:mm:ss.fff"), GetAccCalibFlag != 63 ? 0 : -ReadRoll, GetAccCalibFlag != 63 ? 0 : -ReadPitch, ReadCompass, label83.Text);
            BlackBoxStream.WriteLine("RADIO,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", DateTime.Now.ToString("HH:mm:ss.fff"), ThrottleData, PitchData, RollData, YawData, Aux1Data, Aux2Data, Aux3Data, Aux4Data, Aux5Data, Aux6Data, Aux7Data, Aux8Data);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            BlackBoxAnalyser BlackBox = new BlackBoxAnalyser();
            BlackBox.ShowDialog();
            BlackBox.Dispose();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (CommandArmDisarm == 1)
                {
                    MessageBox.Show("Não é possível acessar as configurações com a JCFLIGHT em Voo!");
                    return;
                }
                ItsSafeToUpdate = false;
                if (PidAndFiltersCommunicationOpen == true)
                {
                    Serial_Write_To_FC(14);
                    PidAndFiltersCommunicationOpen = false;
                }
                Serial_Write_To_FC(13);
                SerialOpen = true;
                numericUpDown25.Value = Convert.ToDecimal(ThrottleMiddle) / 100;
                numericUpDown26.Value = Convert.ToDecimal(ThrottleExpo) / 100;
                numericUpDown27.Value = Convert.ToDecimal(RCRate) / 100;
                numericUpDown28.Value = Convert.ToDecimal(RcExpo) / 100;
                numericUpDown30.Value = Convert.ToDecimal(YawRate) / 100;
                numericUpDown35.Value = RadioMin < 800 ? 1000 : RadioMin;
                numericUpDown36.Value = RadioMax < 1500 ? 2000 : RadioMax;
                numericUpDown62.Value = ThrottleMin < 800 ? 800 : ThrottleMin;
                numericUpDown61.Value = YawMin < 800 ? 800 : YawMin;
                numericUpDown60.Value = PitchMin < 800 ? 800 : PitchMin;
                numericUpDown59.Value = RollMin < 800 ? 800 : RollMin;
                numericUpDown58.Value = ThrottleMax < 800 ? 800 : ThrottleMax;
                numericUpDown57.Value = YawMax < 800 ? 800 : YawMax;
                numericUpDown56.Value = PitchMax < 800 ? 800 : PitchMax;
                numericUpDown55.Value = RollMax < 800 ? 800 : RollMax;
                numericUpDown66.Value = ThrottleDeadZone;
                numericUpDown65.Value = YawDeadZone;
                numericUpDown64.Value = PitchDeadZone;
                numericUpDown63.Value = RollDeadZone;
                numericUpDown41.Value = Servo1Min < 400 ? 400 : Servo1Min;
                numericUpDown39.Value = Servo2Min < 400 ? 400 : Servo2Min;
                numericUpDown40.Value = Servo3Min < 400 ? 400 : Servo3Min;
                numericUpDown42.Value = Servo4Min < 400 ? 400 : Servo4Min;
                numericUpDown43.Value = Servo1Med < 1000 ? 1000 : Servo1Med;
                numericUpDown44.Value = Servo2Med < 1000 ? 1000 : Servo2Med;
                numericUpDown45.Value = Servo3Med < 1000 ? 1000 : Servo3Med;
                numericUpDown46.Value = Servo4Med < 1000 ? 1000 : Servo4Med;
                numericUpDown50.Value = Servo1Max < 1000 ? 1000 : Servo1Max;
                numericUpDown49.Value = Servo2Max < 1000 ? 1000 : Servo2Max;
                numericUpDown48.Value = Servo3Max < 1000 ? 1000 : Servo3Max;
                numericUpDown47.Value = Servo4Max < 1000 ? 1000 : Servo4Max;
                numericUpDown54.Value = Servo1Rate;
                numericUpDown53.Value = Servo2Rate;
                numericUpDown52.Value = Servo3Rate;
                numericUpDown51.Value = Servo4Rate;
                numericUpDown67.Value = FailSafeValue < 800 ? 800 : FailSafeValue;
                numericUpDown89.Value = MaxRollLevel;
                numericUpDown90.Value = MaxPitchLevel;
            }
            SmallCompass = false;
            panel19.Visible = true;
            tabControl1.SelectTab(tabPage9);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
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
            }
            panel19.Visible = false;
            SmallCompass = false;
            tabControl1.SelectTab(tabPage5);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                if (MessageBox.Show("Clicando em 'Sim' todas as configurações feitas aqui serão apagadas,você realmete deseja fazer isso?",
               "Limpar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(29);
                    numericUpDown35.Value = 1000;
                    numericUpDown36.Value = 1900;
                    numericUpDown67.Value = 975;
                    numericUpDown25.Value = (decimal)0.50;
                    numericUpDown26.Value = (decimal)0.00;
                    numericUpDown27.Value = (decimal)0.90;
                    numericUpDown28.Value = (decimal)0.65;
                    numericUpDown30.Value = 0;
                    numericUpDown62.Value = 1050;
                    numericUpDown61.Value = 1050;
                    numericUpDown60.Value = 1050;
                    numericUpDown59.Value = 1050;
                    numericUpDown58.Value = 1950;
                    numericUpDown57.Value = 1950;
                    numericUpDown56.Value = 1950;
                    numericUpDown55.Value = 1950;
                    numericUpDown66.Value = 45;
                    numericUpDown65.Value = 45;
                    numericUpDown64.Value = 45;
                    numericUpDown63.Value = 45;
                    checkBox2.Checked = false;
                    checkBox3.Checked = false;
                    checkBox4.Checked = false;
                    checkBox5.Checked = false;
                    checkBox6.Checked = false;
                    checkBox7.Checked = false;
                    checkBox8.Checked = false;
                    checkBox9.Checked = false;
                    numericUpDown51.Value = 100;
                    numericUpDown52.Value = 100;
                    numericUpDown53.Value = 100;
                    numericUpDown54.Value = 100;
                    numericUpDown41.Value = 1000;
                    numericUpDown39.Value = 1000;
                    numericUpDown40.Value = 1000;
                    numericUpDown42.Value = 1000;
                    numericUpDown43.Value = 1500;
                    numericUpDown44.Value = 1500;
                    numericUpDown45.Value = 1500;
                    numericUpDown46.Value = 1500;
                    numericUpDown50.Value = 2000;
                    numericUpDown49.Value = 2000;
                    numericUpDown48.Value = 2000;
                    numericUpDown47.Value = 2000;
                    numericUpDown89.Value = 30;
                    numericUpDown90.Value = 30;
                }
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
              "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
                }
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen == true)
            {
                SendConfigurationsToJCFLIHGT(SerialPort, 3);
                Thread.Sleep(250);
                SendConfigurationsToJCFLIHGT(SerialPort, 4);
                Thread.Sleep(250);
                Serial_Write_To_FC(32);
                if (MessageBox.Show("Para aplicar as configurações é necessario reiniciar a controladora de voo.Você deseja reiniciar automaticamente agora?",
                  "Reboot", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Serial_Write_To_FC(28);
                    SmallCompass = false;
                    SerialOpen = false;
                    SerialPort.Close();
                    Reboot = true;
                    comboBox7_SelectedIndexChanged(null, null);
                    button1_Click(null, null);
                    ItsSafeToUpdate = true;
                }
            }
        }

        byte GetChannelsReverse()
        {
            int CheckDevices = Convert.ToByte(checkBox2.Checked) |
                Convert.ToByte(checkBox3.Checked) << 1 |
               Convert.ToByte(checkBox4.Checked) << 2 |
               Convert.ToByte(checkBox5.Checked) << 3;
            return Convert.ToByte(CheckDevices);
        }

        byte GetServosReverse()
        {
            int CheckDevices = Convert.ToByte(checkBox9.Checked) |
                Convert.ToByte(checkBox8.Checked) << 1 |
               Convert.ToByte(checkBox7.Checked) << 2 |
               Convert.ToByte(checkBox6.Checked) << 3;
            return Convert.ToByte(CheckDevices);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            GCSConfigurations GCS_Configurations = new GCSConfigurations();
            GCS_Configurations.ShowDialog();
        }

        int SpeechCount = 0;
        bool AutoPilotDeactived = false;
        bool OkToResetSpeech = false;
        int AutoPilotDeactivedCount = 0;
        private void SpeechRun()
        {
            if (SerialPort.IsOpen == false)
            {
                return;
            }

            if (SpeechCount < 3) //O MAIOR AUDIO TEM 3 SEGUNDOS
            {
                SpeechCount++;
                return;
            }

            if (Math.Abs(ReadRoll / 10) > GetValues.BankAngleRollValue)
            {
                Player.URL = Directory.GetCurrentDirectory() + "\\FlightSounds" + "\\BankAngle.mp3";
                OkToResetSpeech = true;
            }

            if ((ReadPitch / 10) <= -NumericConvert[20])
            {
                Player.URL = Directory.GetCurrentDirectory() + "\\FlightSounds" + "\\DontSink.mp3";
                OkToResetSpeech = true;
            }

            if ((FlightMode != 4 || FlightMode != 12) && !AutoPilotDeactived)
            {
                Player.URL = Directory.GetCurrentDirectory() + "\\FlightSounds" + "\\AutoPilotDisengage.mp3";
                OkToResetSpeech = true;
                AutoPilotDeactived = true;
            }
            else
            {
                if (AutoPilotDeactivedCount >= 60)
                {
                    AutoPilotDeactived = false;
                    AutoPilotDeactivedCount = 0;
                }
                else
                {
                    AutoPilotDeactivedCount++;
                }
            }

            if ((ReadPitch / 10) >= 45) //45 GRAUS JÁ PODE SER CONSIDERADO UMA INCLINAÇÃO DE PITCH PERIGOSA
            {
                Player.URL = Directory.GetCurrentDirectory() + "\\FlightSounds" + "\\Stall.mp3";
                OkToResetSpeech = true;
            }

            if (OkToResetSpeech)
            {
                SpeechCount = 0;
                OkToResetSpeech = false;
            }
        }

        private void HUD1_vibeclick_1(object sender, EventArgs e)
        {
            Vibrations VibrationsOpen = new Vibrations();
            VibrationsOpen.TopMost = true;
            VibrationsOpen.Show();

        }

        private void HUD2_vibeclick(object sender, EventArgs e)
        {
            Vibrations VibrationsOpen = new Vibrations();
            VibrationsOpen.TopMost = true;
            VibrationsOpen.Show();
        }
    }
}
