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
        static List<Tuple<float, float, float>> datacompass1 = new List<Tuple<float, float, float>>();
        List<Vector3> points = new List<Vector3>();
        List<Vector3> aimpoints = new List<Vector3>();
        public Vector3 CenterPoint = new Vector3();
        Vector3 eye = new Vector3(1, 1, 1);

        float rawmx = 0;
        float rawmy = 0;
        float rawmz = 0;

        const float rad2deg = (180.0f / 3.1415926535897932384626433832795f);
        const float deg2rad = (1.0f / rad2deg);
        float minx, maxx, miny, maxy, minz, maxz;
        public float scale = 300;
        private float yaw;

        public bool rotatewithdata = false;

        int SecondsCompass = 0;

        public Compass()
        {
            InitializeComponent();
            Clear();
            SecondsCompass = 0;
            if (CompassCalib.Enabled == false) CompassCalib.Enabled = true;
            metroProgressBar1.Value = 0;
        }

        public void Clear()
        {
            lock (points)
            {
                points.Clear();
            }
        }

        public void AimClear()
        {
            lock (aimpoints)
            {
                aimpoints.Clear();
            }
        }

        public void AimFor(Vector3 point)
        {
            lock (aimpoints)
            {
                aimpoints.Add(point);
            }
        }

        private static void setMinorMax(float value, ref float min, ref float max)
        {
            if (value > max)
                max = value;
            if (value < min)
                min = value;
        }

        private void CompassCalib_Tick(object sender, EventArgs e)
        {
            SecondsCompass++;
            metroProgressBar1.Value++;
            label92.Text = "Tempo corrido da Calibração:" + ((SecondsCompass / 60).ToString("00.") + ":" + (SecondsCompass % 60).ToString("00."));
            if (SecondsCompass / 60 == 1)
            {
                SecondsCompass = 0;
                label92.Text = "Tempo corrido da Calibração:00:00";
                CompassCalib.Enabled = false;
                MessageBox.Show("Calibração do Compass concluída!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            rotatewithdata = checkBox1.Checked;

            rawmx = GCS.CompassRoll;
            rawmy = GCS.CompassPitch;
            rawmz = GCS.CompassYaw;

            metroLabel1.Text = "Compass Roll:" + Convert.ToString(GCS.CompassRoll);
            metroLabel2.Text = "Compass Pitch:" + Convert.ToString(GCS.CompassPitch);
            metroLabel3.Text = "Compass Yaw:" + Convert.ToString(GCS.CompassYaw);
            metroLabel6.Text = "Roll Min:" + Convert.ToString(minx);
            metroLabel5.Text = "Roll Max:" + Convert.ToString(maxx);
            metroLabel8.Text = "Pitch Min:" + Convert.ToString(miny);
            metroLabel7.Text = "Pitch Max:" + Convert.ToString(maxy);
            metroLabel10.Text = "Yaw Min:" + Convert.ToString(minz);
            metroLabel9.Text = "Yaw Max:" + Convert.ToString(maxz);

            datacompass1.Add(new Tuple<float, float, float>(rawmx, rawmy, rawmz));

            Vector3 centre = new Vector3();
            Vector3 point;

            AddPoint(new OpenTK.Vector3(rawmx, rawmy, rawmz));
            AimClear();

            point = new Vector3(rawmx, rawmy, rawmz) + centre;

            rawmx = datacompass1[datacompass1.Count - 1].Item1;
            rawmy = datacompass1[datacompass1.Count - 1].Item2;
            rawmz = datacompass1[datacompass1.Count - 1].Item3;

            setMinorMax(rawmx, ref minx, ref maxx);
            setMinorMax(rawmy, ref miny, ref maxy);
            setMinorMax(rawmz, ref minz, ref maxz);

            float radius = 0;
            for (int i = 0; i < datacompass1.Count; i++)
            {
                point = new Vector3(datacompass1[i].Item1, datacompass1[i].Item2, datacompass1[i].Item3);
                radius += (float)(point + centre).Length;
            }

            radius /= datacompass1.Count;

            int pointshit = 0;
            int factor = 3;
            int factor2 = 4;
            float max_distance = radius / 3;
            for (int j = 0; j <= factor; j++)
            {
                float theta = (3.1415926535897932384626433832795f * (j + 0.5f)) / factor;

                for (int i = 0; i <= factor2; i++)
                {
                    float phi = (2.0f * 3.1415926535897932384626433832795f * i) / factor2;

                    Vector3 point_sphere = new Vector3(
                        (float)(Math.Sin(theta) * Math.Cos(phi) * radius),
                        (float)(Math.Sin(theta) * Math.Sin(phi) * radius),
                        (float)(Math.Cos(theta) * radius)) - centre;

                    bool found = false;
                    for (int k = 0; k < datacompass1.Count; k++)
                    {
                        point = new Vector3(datacompass1[k].Item1, datacompass1[k].Item2, datacompass1[k].Item3);
                        float d = (point_sphere - point).Length;
                        if (d < max_distance)
                        {
                            pointshit++;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        AimFor(new OpenTK.Vector3((float)point_sphere.X, (float)point_sphere.Y, (float)point_sphere.Z));
                    }
                }
            }
            RemoveOutliers(ref datacompass1);
        }

        static void RemoveOutliers(ref List<Tuple<float, float, float>> data)
        {
            data.Sort(
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

            data.RemoveRange(data.Count - (data.Count / 16), data.Count / 16);
        }

        public void AddPoint(Vector3 point)
        {
            minx = (float)Math.Min(minx, point.X);
            maxx = (float)Math.Max(maxx, point.X);
            miny = (float)Math.Min(miny, point.Y);
            maxy = (float)Math.Max(maxy, point.Y);
            minz = (float)Math.Min(minz, point.Z);
            maxz = (float)Math.Max(maxz, point.Z);
            lock (points)
            {
                points.Add(point);
            }
            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.Left)
            {
                yaw += -5 * deg2rad;
                this.Invalidate();
                return true;
            }
            if (keyData == System.Windows.Forms.Keys.Right)
            {
                yaw += 5 * deg2rad;
                this.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                e.Graphics.Clear(Color.Black);
                return;
            }

            GL.Viewport(100, -100, 600, 600);

            if (rotatewithdata)
                yaw += 5 * deg2rad;

            glControl1.MakeCurrent();

            GL.MatrixMode(MatrixMode.Projection);

            float max = Math.Max(Math.Max((maxx - minx) / 2, (maxy - miny) / 2), (maxz - minz) / 2);

            if (max < 300)
                max = 400;

            max *= 1.3f;

            if (points.Count > 0)
            {
                Vector3 current = new Vector3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z);
            }

            OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(45 * deg2rad), 1f, 0.00001f, 5000.0f);
            GL.LoadMatrix(ref projection);

            float eyedist = (float)max * 3;

            eye = Vector3.TransformPosition(eye, Matrix4.CreateRotationZ((float)yaw));

            yaw = 0;

            if (float.IsNaN(eye.X))
                eye = new Vector3(1, 1, 1);

            eye.Normalize();

            eye *= eyedist;
            Matrix4 modelview = Matrix4.LookAt(eye.X, eye.Y, eye.Z, 0, 0, 0, 0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PointSize(8);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.FromArgb(0, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, max);
            GL.Color3(Color.FromArgb(0, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, max, 0);
            GL.Color3(Color.FromArgb(255, 0, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(max, 0, 0);
            GL.Color3(Color.FromArgb(255, 255, 0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, -max);
            GL.Color3(Color.FromArgb(255, 0, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, -max, 0);
            GL.Color3(Color.FromArgb(0, 255, 255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(-max, 0, 0);
            GL.End();
            GL.Begin(PrimitiveType.Points);
            lock (points)
            {
                foreach (var item in points)
                {
                    float rangex = maxx - minx;
                    float rangey = maxy - miny;
                    float rangez = maxz - minz;

                    int valuex = (int)Math.Abs((((item.X) / rangex) * 254)) & 0xff;
                    int valuey = (int)Math.Abs((((item.Y) / rangey) * 254)) & 0xff;
                    int valuez = (int)Math.Abs((((item.Z) / rangez) * 254)) & 0xff;

                    Color col = Color.FromArgb(valuex, valuey, valuez);

                    GL.Color3(col);

                    Vector3 vec = new Vector3(item.X, item.Y, item.Z) + CenterPoint;

                    GL.Vertex3(vec);
                }
                lock (aimpoints)
                {
                    foreach (var aim in aimpoints)
                    {
                        GL.PointSize(8);
                        GL.Color3(Color.White);
                        GL.Vertex3(new Vector3(aim.X, aim.Y, aim.Z) + CenterPoint);
                    }
                }
                GL.End();
                GL.PointSize(12);
                GL.Begin(PrimitiveType.Points);
                GL.Color3(Color.Red);
                if (points.Count > 0)
                    GL.Vertex3(new Vector3(points[points.Count - 1].X, points[points.Count - 1].Y, points[points.Count - 1].Z) + CenterPoint);
            }
            GL.End();
            Console.WriteLine(Math.Atan2(eye.Y, eye.X));
            glControl1.SwapBuffers();
        }
    }
}
