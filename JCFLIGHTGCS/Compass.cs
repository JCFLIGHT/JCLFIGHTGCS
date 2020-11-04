using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    public partial class Compass : Form
    {
        int MagX = 0;
        int MagY = 0;

        int constrain(int amt, int low, int high)
        {
            return ((amt) < (low) ? (low) : ((amt) > (high) ? (high) : (amt)));
        }

        public Compass()
        {
            InitializeComponent();
        }

        private List<Point> GetRandomPoints()
        {
            MagX = constrain(GCS.CompassRoll, -180, 180);
            MagY = constrain(GCS.CompassPitch, -180, 180);
            metroLabel1.Text = "Compass Roll:" + Convert.ToString(GCS.CompassRoll);
            metroLabel2.Text = "Compass Pitch:" + Convert.ToString(GCS.CompassPitch);
            var randPoints = new List<Point>();

            randPoints.Add(new Point((int)(MagX), (int)(-MagY)));

            randPoints.Add(new Point((int)(MagX + 2), (int)(-MagY + 2)));

            randPoints.Add(new Point((int)(MagX + 4), (int)(-MagY + 4)));

            randPoints.Add(new Point((int)(MagX + 6), (int)(-MagY + 6)));

            return randPoints;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (Graphics g = this.CreateGraphics())
            {
                using (Pen p = new Pen(Color.Blue, 8))
                {
                    var left = 150; //POSIÇÃO X
                    var top = 5;  //POSIÇÃO Y
                    var r = 200;   //RAIO
                    g.DrawEllipse(p, new Rectangle(left, top, r * 2, r * 2));

                    var randomPoints = GetRandomPoints();

                    if (MagX < 0)
                    {
                        Pen Pen = new Pen(Color.Violet, 5);
                        g.DrawEllipse(Pen, new Rectangle(randomPoints[0].X + left + r, randomPoints[0].Y + top + r, 5, 5));
                    }

                    if (MagX > 0)
                    {
                        Pen Pen = new Pen(Color.Red, 5);
                        g.DrawEllipse(Pen, new Rectangle(randomPoints[1].X + left + r, randomPoints[1].Y + top + r, 5, 5));
                    }

                    if (MagY < 0)
                    {
                        Pen Pen = new Pen(Color.DarkGreen, 5);
                        g.DrawEllipse(Pen, new Rectangle(randomPoints[2].X + left + r, randomPoints[2].Y + top + r, 5, 5));
                    }

                    if (MagY > 0)
                    {
                        Pen Pen = new Pen(Color.Yellow, 5);
                        g.DrawEllipse(Pen, new Rectangle(randomPoints[3].X + left + r, randomPoints[3].Y + top + r, 5, 5));
                    }

                }
            }
        }


    }
}
