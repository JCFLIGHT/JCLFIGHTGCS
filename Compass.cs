using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace JCFLIGHTGCS
{
    public partial class Compass : Form
    {
        static List<Tuple<float, float, float>> DataCompassRegister = new List<Tuple<float, float, float>>();
        List<Vector3> Points = new List<Vector3>();
        List<Vector3> AimPoints = new List<Vector3>();
        public Vector3 CenterPoint = new Vector3();
        Vector3 Eye = new Vector3(1, 1, 1);

        bool RotateYawData = true;
        bool Cal_Success = false;

        float RawMagX = 0;
        float RawMagY = 0;
        float RawMagZ = 0;
        float MinMagX;
        float MaxMagX;
        float MinMagY;
        float MaxMagY;
        float MinMagZ;
        float MaxMagZ;
        float YawRotation;
        float PitchRotation;

        int SecondsCompass = 0;

        public Compass()
        {
            InitializeComponent();
            Clear();
            SecondsCompass = 0;
            if (CompassCalib.Enabled == false) CompassCalib.Enabled = true;
        }

        public void AddPoint(Vector3 VectorPoint)
        {
            MinMagX = (float)Math.Min(MinMagX, VectorPoint.X);
            MaxMagX = (float)Math.Max(MaxMagX, VectorPoint.X);
            MinMagY = (float)Math.Min(MinMagY, VectorPoint.Y);
            MaxMagY = (float)Math.Max(MaxMagY, VectorPoint.Y);
            MinMagZ = (float)Math.Min(MinMagZ, VectorPoint.Z);
            MaxMagZ = (float)Math.Max(MaxMagZ, VectorPoint.Z);
            lock (Points)
            {
                Points.Add(VectorPoint);
            }
            this.Invalidate();
        }

        public void Clear()
        {
            lock (Points)
            {
                Points.Clear();
            }
        }

        public void AimClear()
        {
            lock (AimPoints)
            {
                AimPoints.Clear();
            }
        }

        public void AimFor(Vector3 VectorPoint)
        {
            lock (AimPoints)
            {
                AimPoints.Add(VectorPoint);
            }
        }

        private static void setMinorMax(float Value, ref float Min, ref float Max)
        {
            if (Value > Max)
                Max = Value;
            if (Value < Min)
                Min = Value;
        }

        private void CompassCalib_Tick(object sender, EventArgs e)
        {
            SecondsCompass++;
            progressBar1.Value++;
            label92.Text = "Tempo corrido da Calibração:" + ((SecondsCompass / 60).ToString("00.") + ":" + (SecondsCompass % 60).ToString("00."));
            if (SecondsCompass / 60 == 1)
            {
                SecondsCompass = 0;
                label92.Text = "Tempo corrido da Calibração:00:00";
                Cal_Success = true;
                MessageBox.Show("Calibração do Compass concluída!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Cal_Success)
            {
                CompassCalib.Enabled = false;
                progressBar1.Value = 60;
                label92.Text = "Tempo corrido da Calibração:01:00";
                RotateYawData = false;
            }
            else
            {
                RawMagX = GetValues.CompassRoll;
                RawMagY = GetValues.CompassPitch;
                RawMagZ = GetValues.CompassYaw;
            }

            metroLabel1.Text = "Compass X:" + Convert.ToString(GetValues.CompassRoll);
            metroLabel2.Text = "Compass Y:" + Convert.ToString(GetValues.CompassPitch);
            metroLabel3.Text = "Compass Z:" + Convert.ToString(GetValues.CompassYaw);
            metroLabel6.Text = "X - Min:" + Convert.ToString(MinMagX);
            metroLabel5.Text = "X - Max:" + Convert.ToString(MaxMagX);
            metroLabel8.Text = "Y - Min:" + Convert.ToString(MinMagY);
            metroLabel7.Text = "Y - Max:" + Convert.ToString(MaxMagY);
            metroLabel10.Text = "Z - Min:" + Convert.ToString(MinMagZ);
            metroLabel9.Text = "Z - Max:" + Convert.ToString(MaxMagZ);

            DataCompassRegister.Add(new Tuple<float, float, float>(RawMagX, RawMagY, RawMagZ));

            Vector3 VectorCentre = new Vector3();
            Vector3 VectorPoint;

            AddPoint(new OpenTK.Vector3(RawMagX, RawMagY, RawMagZ));
            AimClear();

            VectorPoint = new Vector3(RawMagX, RawMagY, RawMagZ) + VectorCentre;

            RawMagX = DataCompassRegister[DataCompassRegister.Count - 1].Item1;
            RawMagY = DataCompassRegister[DataCompassRegister.Count - 1].Item2;
            RawMagZ = DataCompassRegister[DataCompassRegister.Count - 1].Item3;

            setMinorMax(RawMagX, ref MinMagX, ref MaxMagX);
            setMinorMax(RawMagY, ref MinMagY, ref MaxMagY);
            setMinorMax(RawMagZ, ref MinMagZ, ref MaxMagZ);

            float Radius = 0;
            for (int i = 0; i < DataCompassRegister.Count; i++)
            {
                VectorPoint = new Vector3(DataCompassRegister[i].Item1, DataCompassRegister[i].Item2, DataCompassRegister[i].Item3);
                Radius += (float)(VectorPoint + VectorCentre).Length;
            }

            Radius /= DataCompassRegister.Count;

            int Factor = 3;
            int Factor2 = 4;
            float MaxDistance = Radius / 3;
            for (int j = 0; j <= Factor; j++)
            {
                float Theta = (3.1415926535897932384626433832795f * (j + 0.5f)) / Factor;

                for (int i = 0; i <= Factor2; i++)
                {
                    float Phi = (2.0f * 3.1415926535897932384626433832795f * i) / Factor2;

                    Vector3 point_sphere = new Vector3(
                        (float)(Math.Sin(Theta) * Math.Cos(Phi) * Radius),
                        (float)(Math.Sin(Theta) * Math.Sin(Phi) * Radius),
                        (float)(Math.Cos(Theta) * Radius)) - VectorCentre;

                    bool Found = false;
                    for (int k = 0; k < DataCompassRegister.Count; k++)
                    {
                        VectorPoint = new Vector3(DataCompassRegister[k].Item1, DataCompassRegister[k].Item2, DataCompassRegister[k].Item3);
                        float d = (point_sphere - VectorPoint).Length;
                        if (d < MaxDistance)
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        AimFor(new OpenTK.Vector3((float)point_sphere.X, (float)point_sphere.Y, (float)point_sphere.Z));
                    }
                }
            }
            RemoveOutliers(ref DataCompassRegister);
        }

        static void RemoveOutliers(ref List<Tuple<float, float, float>> Data)
        {
            Data.Sort(
                delegate (Tuple<float, float, float> d1, Tuple<float, float, float> d2)
                {
                    double ans1 = Math.Sqrt(d1.Item1 * d1.Item1 + d1.Item2 * d1.Item2 + d1.Item3 * d1.Item3);
                    double ans2 = Math.Sqrt(d2.Item1 * d2.Item1 + d2.Item2 * d2.Item2 + d2.Item3 * d2.Item3);
                    if (ans1 > ans2)
                        return 1;
                    if (ans1 < ans2)
                        return -1;
                    return 0;
                }
                );

            Data.RemoveRange(Data.Count - (Data.Count / 16), Data.Count / 16);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }

            GL.Viewport(100, -100, 600, 600);

            if (RotateYawData)
            {
                YawRotation += 5 * (3.1415926535897931f / 180.0f);
                PitchRotation += -5 * (3.1415926535897931f / 180.0f);
            }

            glControl1.MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            float Max = Math.Max(Math.Max((MaxMagX - MinMagX) / 2, (MaxMagY - MinMagY) / 2), (MaxMagZ - MinMagZ) / 2);
            if (Max < 300) Max = 400;
            Max *= 1.3f;

            OpenTK.Matrix4 Projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(45 * (3.1415926535897931f / 180.0f)), 1f, 0.00001f, 5000.0f);
            GL.LoadMatrix(ref Projection);

            Eye = Vector3.TransformPosition(Eye, Matrix4.CreateRotationZ((float)YawRotation));
            if (Cal_Success) Eye = Vector3.TransformPosition(Eye, Matrix4.CreateRotationX((float)PitchRotation));

            PitchRotation = 0;
            YawRotation = 0;

            if (float.IsNaN(Eye.X))
                Eye = new Vector3(1, 1, 1);

            Eye.Normalize();

            Eye *= 14000;
            Matrix4 modelview = Matrix4.LookAt(Eye.X, Eye.Y, Eye.Z, 0, 0, 0, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PointSize(8);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.FromArgb(0, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, Max);
            GL.Color3(Color.FromArgb(0, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, Max, 0);
            GL.Color3(Color.FromArgb(255, 0, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(Max, 0, 0);
            GL.Color3(Color.FromArgb(255, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -Max);
            GL.Color3(Color.FromArgb(255, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, -Max, 0);
            GL.Color3(Color.FromArgb(0, 255, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(-Max, 0, 0);
            GL.End();
            GL.Begin(PrimitiveType.Points);
            lock (Points)
            {
                foreach (var Item in Points)
                {
                    float RangeX = MaxMagX - MinMagX;
                    float RangeY = MaxMagY - MinMagY;
                    float RangeZ = MaxMagZ - MinMagZ;

                    int ValueX = (int)Math.Abs((((Item.X) / RangeX) * 254)) & 0xff;
                    int ValueY = (int)Math.Abs((((Item.Y) / RangeY) * 254)) & 0xff;
                    int ValueZ = (int)Math.Abs((((Item.Z) / RangeZ) * 254)) & 0xff;

                    Color GetColor = Color.FromArgb(ValueX, ValueY, ValueZ);

                    GL.Color3(GetColor);

                    Vector3 vec = new Vector3(Item.X, Item.Y, Item.Z) + CenterPoint;

                    GL.Vertex3(vec);
                }
                lock (AimPoints)
                {
                    foreach (var Aim in AimPoints)
                    {
                        GL.PointSize(8);
                        GL.Color3(Color.White);
                        GL.Vertex3(new Vector3(Aim.X, Aim.Y, Aim.Z) + CenterPoint);
                    }
                }
                GL.End();
                GL.PointSize(12);
                GL.Begin(PrimitiveType.Points);
                GL.Color3(Color.Red);
                if (Points.Count > 0)
                    GL.Vertex3(new Vector3(Points[Points.Count - 1].X, Points[Points.Count - 1].Y, Points[Points.Count - 1].Z) + CenterPoint);
            }
            GL.End();
            glControl1.SwapBuffers();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message Message, System.Windows.Forms.Keys KeyData)
        {
            if (RotateYawData) return false;
            if (KeyData == System.Windows.Forms.Keys.Up)
            {
                PitchRotation += -5 * (3.1415926535897931f / 180.0f);
                this.Invalidate();
                return true;
            }
            if (KeyData == System.Windows.Forms.Keys.Down)
            {
                PitchRotation += 5 * (3.1415926535897931f / 180.0f);
                this.Invalidate();
                return true;
            }
            if (KeyData == System.Windows.Forms.Keys.Left)
            {
                YawRotation += -5 * (3.1415926535897931f / 180.0f);
                this.Invalidate();
                return true;
            }
            if (KeyData == System.Windows.Forms.Keys.Right)
            {
                YawRotation += 5 * (3.1415926535897931f / 180.0f);
                this.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref Message, KeyData);
        }
    }
}
