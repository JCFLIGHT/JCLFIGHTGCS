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
    class CreateGMapMarker : GMapMarker
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
}
