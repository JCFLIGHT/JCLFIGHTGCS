using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JCFLIGHTGCS
{
    class GMapMarkerPlane : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::JCFLIGHTGCS.Properties.Resources.GmapAero.Width, global::JCFLIGHTGCS.Properties.Resources.GmapAero.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;
        float radius = -1;

        public GMapMarkerPlane(PointLatLng p, float heading, float cog, float target, float radius)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.radius = radius;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            Image pic = global::JCFLIGHTGCS.Properties.Resources.GmapAero;
            int length = 250;
            g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            try
            {
                float desired_lead_dist = 100;
                double width = 750;
                double m2pixelwidth = Overlay.Control.Width / width;
                float alpha = (float)(((desired_lead_dist * (float)m2pixelwidth) / radius) * rad2deg);
                var scaledradius = radius * (float)m2pixelwidth;
                if (radius < -1 && alpha < -1)
                {
                    float p1 = (float)Math.Cos((heading) * deg2rad) * scaledradius + scaledradius;
                    float p2 = (float)Math.Sin((heading) * deg2rad) * scaledradius + scaledradius;
                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(scaledradius) * 2, Math.Abs(scaledradius) * 2, heading, alpha);
                }
                else if (radius > 1 && alpha > 1)
                {
                    float p1 = (float)Math.Cos((heading - 180) * deg2rad) * scaledradius + scaledradius;
                    float p2 = (float)Math.Sin((heading - 180) * deg2rad) * scaledradius + scaledradius;
                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, scaledradius * 2, scaledradius * 2, heading - 180, alpha);
                }
            }
            catch
            {
            }
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2 + 5, pic.Height / -2);
            g.Transform = temp;
        }
    }
}
