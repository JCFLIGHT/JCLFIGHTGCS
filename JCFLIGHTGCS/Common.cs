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
    class Common
    {
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
            //g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
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
            //g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
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
            //g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
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
            //g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length, (float)Math.Sin((cog - 90) * deg2rad) * length);
            //g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length, (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            g.RotateTransform(heading);
            g.DrawImageUnscaled(pic, pic.Width / -2, pic.Height / -2);
            g.Transform = temp;
        }
    }

}
