using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Data;

namespace JCFLIGHTGCS
{
    class AttitudeIndicatorInstrumentControl : InstrumentControl
    {
        // Parameters
        double PitchAngle = 0; // Phi
        double RollAngle = 0; // Theta

        int GetBMPType = 0;
        Boolean GetLarger = false;
        Boolean GetPrint = false;

        // Images
        Bitmap bmpCadran = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Horizon_Background);
        Bitmap bmpBoule = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Horizon_GroundSky);
        Bitmap bmpAvion = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Maquette_Avion);

        Bitmap bmpArm = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Armado);
        Bitmap bmpDesarm = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Desarmado);
        Bitmap bmpFailSafe = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.FailSafe);

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public AttitudeIndicatorInstrumentControl()
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

            Point ptBoule = new Point(-25, -410);
            Point ptRotation = new Point(150, 150);

            float scale = (float)this.Width / bmpCadran.Width;

            // Affichages - - - - - - - - - - - - - - - - - - - - - - 

            bmpCadran.MakeTransparent(Color.Yellow);
            bmpAvion.MakeTransparent(Color.Yellow);
            bmpArm.MakeTransparent(Color.Yellow);
            bmpDesarm.MakeTransparent(Color.Yellow);
            bmpFailSafe.MakeTransparent(Color.Yellow);

            // display Horizon
            RotateAndTranslate(pe, bmpBoule, RollAngle, 0, ptBoule, (int)(4 * PitchAngle), ptRotation, scale);

            // diplay mask
            Pen maskPen = new Pen(this.BackColor, 30 * scale);
            pe.Graphics.DrawRectangle(maskPen, 0, 0, bmpCadran.Width * scale, bmpCadran.Height * scale);

            // display cadran
            pe.Graphics.DrawImage(bmpCadran, 0, 0, (float)(bmpCadran.Width * scale), (float)(bmpCadran.Height * scale));

            // display aircraft symbol
            pe.Graphics.DrawImage(bmpAvion, (float)((0.5 * bmpCadran.Width - 0.5 * bmpAvion.Width) * scale), (float)((0.5 * bmpCadran.Height - 0.5 * bmpAvion.Height) * scale), (float)(bmpAvion.Width * scale), (float)(bmpAvion.Height * scale));

            if (GetPrint)
            {
                if (GetLarger)
                {
                    if (GetBMPType == 0) pe.Graphics.DrawImage(bmpDesarm, 90, 110, 120, 80);
                    if (GetBMPType == 1) pe.Graphics.DrawImage(bmpArm, 90, 110, 120, 80);
                    if (GetBMPType == 2) pe.Graphics.DrawImage(bmpFailSafe, 90, 110, 120, 80);
                }
                else
                {
                    if (GetBMPType == 0) pe.Graphics.DrawImage(bmpDesarm, 45, 55, 70, 50);
                    if (GetBMPType == 1) pe.Graphics.DrawImage(bmpArm, 45, 55, 70, 50);
                    if (GetBMPType == 2) pe.Graphics.DrawImage(bmpFailSafe, 45, 55, 70, 50);
                }
            }
        }

        /// <summary>
        /// Define the physical value to be displayed on the indicator
        /// </summary>
        /// <param name="aircraftPitchAngle">The aircraft pitch angle in °deg</param>
        /// <param name="aircraftRollAngle">The aircraft roll angle in °deg</param
        public void SetAttitudeIndicatorParameters(double aircraftPitchAngle, double aircraftRollAngle)
        {
            PitchAngle = (aircraftRollAngle);
            RollAngle = (aircraftPitchAngle * Math.PI / 180);
            /*
            PitchAngle = -(aircraftPitchAngle);
            RollAngle = -(aircraftRollAngle * Math.PI / 180);
            */
            this.Refresh();
        }

        public void SetNoticeInArtificialHorizon(int Type, Boolean Larger, Boolean Print)
        {
            GetBMPType = Type;
            GetLarger = Larger;
            GetPrint = Print;
        }
    }
}
