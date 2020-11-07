using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Data;

namespace JCFLIGHTGCS
{
    class HeadingIndicatorInstrumentControl : InstrumentControl
    {

        // Parameters
        int Heading;
        Boolean Small = false;

        // Images
        Bitmap bmpCadran = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.HeadingIndicator_Background);
        Bitmap bmpHedingWeel = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.HeadingWeel);
        Bitmap bmpAircaft = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.HeadingIndicator_Aircraft);

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public HeadingIndicatorInstrumentControl()
        {
            // Double bufferisation
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            // Pre Display computings
            Point ptRotation = new Point(150, 150);
            Point ptImgAircraft = new Point(52, 11);
            Point ptImgHeadingWeel = new Point(13, 13);

            bmpCadran.MakeTransparent(Color.Yellow);
            bmpHedingWeel.MakeTransparent(Color.Yellow);
            bmpAircaft.MakeTransparent(Color.Yellow);

            double alphaHeadingWeel = InterpolPhyToAngle(Heading, 0, 360, 360, 0);

            float scale = (float)this.Width / bmpCadran.Width;

            // diplay mask
            Pen maskPen = new Pen(this.BackColor, 30 * scale);
            pe.Graphics.DrawRectangle(maskPen, 0, 0, bmpCadran.Width * scale, bmpCadran.Height * scale);

            // display cadran
            pe.Graphics.DrawImage(bmpCadran, 0, 0, (float)(bmpCadran.Width * scale), (float)(bmpCadran.Height * scale));

            // display HeadingWeel
            RotateImage(pe, bmpHedingWeel, alphaHeadingWeel, ptImgHeadingWeel, ptRotation, scale);

            // display aircraft
            if (Small == false)
            {
                pe.Graphics.DrawImage(bmpAircaft, (int)(ptImgAircraft.X * scale), (int)(ptImgAircraft.Y * scale), (float)(bmpAircaft.Width * scale + 65), (float)(bmpAircaft.Height * scale + 65));
            }
            else
            {
                pe.Graphics.DrawImage(bmpAircaft, (int)(ptImgAircraft.X * scale), (int)(ptImgAircraft.Y * scale), (float)(bmpAircaft.Width * scale + 35), (float)(bmpAircaft.Height * scale + 35));
            }
        }

        /// <summary>
        /// Define the physical value to be displayed on the indicator
        /// </summary>
        /// <param name="aircraftHeading">The aircraft heading in °deg</param>
        public void SetHeadingIndicatorParameters(int aircraftHeading, Boolean SmallType)
        {
            Heading = aircraftHeading;
            Small = SmallType;
            this.Refresh();
        }
    }
}
