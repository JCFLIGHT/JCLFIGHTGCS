using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.NetworkInformation;

namespace JCFLIGHTGCS
{
    class Common
    {
    }

    public class Stuff
    {
        public static bool PingNetwork(string hostNameOrAddress)
        {
            bool pingStatus = false;

            using (Ping p = new Ping())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                int timeout = 4444;

                try
                {
                    PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }
            return pingStatus;
        }
    }

    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 3);
        public Color Color { get { return Pen.Color; } set { Pen.Color = value; } }
        public GMapMarker InnerMarker;
        public int WPRadius = 0;
        public GMapControl MainMap;

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dot;
            Size = new Size(50, 50);
            Offset = new Point(-Size.Width / 2, -Size.Height / 2 - 20);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (WPRadius == 0 || MainMap == null)
                return;

            GPoint loc = new GPoint((int)(LocalPosition.X - (WPRadius * 2)), LocalPosition.Y);
            try
            {
                g.DrawArc(Pen, new Rectangle((int)(LocalPosition.X - Offset.X - (Math.Abs(loc.X - LocalPosition.X) / 2)), (int)(LocalPosition.Y - Offset.Y - Math.Abs(loc.X - LocalPosition.X) / 2), (int)(Math.Abs(loc.X - LocalPosition.X)), (int)(Math.Abs(loc.X - LocalPosition.X))), 0, 360);
            }
            catch { }
        }
    }

    public class CreateGMapMarker : GMapMarker
    {
        private byte WPNumber;
        public CreateGMapMarker(PointLatLng Point, byte ID)
            : base(Point)
        {
            WPNumber = ID;
        }

        public override void OnRender(Graphics g)
        {
            Image pic;
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            pic = Properties.Resources.marker_01;
            g.DrawImageUnscaled(pic, pic.Width / -2 - 1, -pic.Height + 3);
            Font drawFont = new Font(FontFamily.GenericMonospace, 9.0F, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            drawBrush.Color = Color.Yellow;
            g.DrawString("WP", drawFont, drawBrush, -10, -54);
            drawBrush.Color = Color.White;
            if (WPNumber < 10) g.DrawString(String.Format("{0:0}", WPNumber), drawFont, drawBrush, -6, -37);
            if (WPNumber < 100 && WPNumber > 9) g.DrawString(String.Format("{0:0}", WPNumber), drawFont, drawBrush, -10, -37);
            if (WPNumber > 100) g.DrawString(String.Format("{0:0}", WPNumber), drawFont, drawBrush, -12, -40);
            g.Transform = temp;

        }
    }

    public class GMapMarkerQuad : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::JCFLIGHTGCS.Properties.Resources.GmapQuad.Width, global::JCFLIGHTGCS.Properties.Resources.GmapQuad.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerQuad(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            Image pic = global::JCFLIGHTGCS.Properties.Resources.GmapQuad;

            int length = 250;
            // anti NaN
            g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2 - 5, pic.Height / -2);
            g.Transform = temp;
        }
    }

    public class GMapMarkerAero : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::JCFLIGHTGCS.Properties.Resources.GmapAero.Width, global::JCFLIGHTGCS.Properties.Resources.GmapAero.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerAero(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            Image pic = global::JCFLIGHTGCS.Properties.Resources.GmapAero;

            int length = 250;
            // anti NaN
            g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2 + 5, pic.Height / -2);
            g.Transform = temp;
        }
    }

    public class GMapMarkerHexaX : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::JCFLIGHTGCS.Properties.Resources.HexaX.Width, global::JCFLIGHTGCS.Properties.Resources.HexaX.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerHexaX(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            Image pic = global::JCFLIGHTGCS.Properties.Resources.HexaX;

            int length = 250;
            // anti NaN
            g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2 - 4, pic.Height / -2);
            g.Transform = temp;
        }
    }

    public class GMapMarkerHexaI : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static readonly System.Drawing.Size SizeSt = new System.Drawing.Size(global::JCFLIGHTGCS.Properties.Resources.HexaI.Width, global::JCFLIGHTGCS.Properties.Resources.HexaI.Height);
        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerHexaI(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            Size = SizeSt;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);
            Image pic = global::JCFLIGHTGCS.Properties.Resources.HexaI;

            int length = 250;
            // anti NaN
            g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length, (float)Math.Sin((heading - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2, pic.Height / -2);
            g.Transform = temp;
        }
    }

}
