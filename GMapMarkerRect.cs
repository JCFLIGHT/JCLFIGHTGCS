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
    class GMapMarkerRect : GMapMarker
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
}
