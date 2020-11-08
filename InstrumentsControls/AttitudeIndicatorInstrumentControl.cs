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
        double PitchAngle = 0;
        double RollAngle = 0;

        int GetBMPType = 0;
        Boolean GetLarger = false;
        Boolean GetPrint = false;

        Bitmap bmpCadran = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Horizon_Background);
        Bitmap bmpBoule = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Horizon_GroundSky);
        Bitmap bmpAvion = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Maquette_Avion);

        Bitmap bmpArm = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Armado);
        Bitmap bmpDesarm = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.Desarmado);
        Bitmap bmpFailSafe = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.FailSafe);
        Bitmap bmpBankAngle = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.BankAngle);
        Bitmap bmpIMUBad = new Bitmap(JCFLIGHTGCS.InstrumentsControls.InstrumentsControlsRessources.IMURuim);

        private System.ComponentModel.Container components = null;

        public AttitudeIndicatorInstrumentControl()
        {
            // Double bufferisation
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Point ptBoule = new Point(-25, -410);
            Point ptRotation = new Point(150, 150);

            float scale = (float)this.Width / bmpCadran.Width;

            bmpCadran.MakeTransparent(Color.Yellow);
            bmpAvion.MakeTransparent(Color.Yellow);
            bmpArm.MakeTransparent(Color.Yellow);
            bmpDesarm.MakeTransparent(Color.Yellow);
            bmpFailSafe.MakeTransparent(Color.Yellow);
            bmpBankAngle.MakeTransparent(Color.Yellow);
            bmpIMUBad.MakeTransparent(Color.Yellow);

            RotateAndTranslate(pe, bmpBoule, RollAngle, 0, ptBoule, (int)(4 * PitchAngle), ptRotation, scale);

            Pen maskPen = new Pen(this.BackColor, 30 * scale);
            pe.Graphics.DrawRectangle(maskPen, 0, 0, bmpCadran.Width * scale, bmpCadran.Height * scale);

            pe.Graphics.DrawImage(bmpCadran, 0, 0, (float)(bmpCadran.Width * scale), (float)(bmpCadran.Height * scale));

            pe.Graphics.DrawImage(bmpAvion, (float)((0.5 * bmpCadran.Width - 0.5 * bmpAvion.Width) * scale), (float)((0.5 * bmpCadran.Height - 0.5 * bmpAvion.Height) * scale), (float)(bmpAvion.Width * scale), (float)(bmpAvion.Height * scale));

            if (GetPrint)
            {
                if (GetLarger)
                {
                    if (GetBMPType == 0) pe.Graphics.DrawImage(bmpDesarm, 90, 110, 120, 80);
                    if (GetBMPType == 1) pe.Graphics.DrawImage(bmpArm, 90, 110, 120, 80);
                    if (GetBMPType == 2) pe.Graphics.DrawImage(bmpFailSafe, 90, 110, 120, 80);
                    if (GetBMPType == 3) pe.Graphics.DrawImage(bmpBankAngle, 90, 110, 120, 80);
                    if (GetBMPType == 4) pe.Graphics.DrawImage(bmpIMUBad, 90, 110, 120, 80);
                }
                else
                {
                    if (GetBMPType == 0) pe.Graphics.DrawImage(bmpDesarm, 45, 55, 70, 50);
                    if (GetBMPType == 1) pe.Graphics.DrawImage(bmpArm, 45, 55, 70, 50);
                    if (GetBMPType == 2) pe.Graphics.DrawImage(bmpFailSafe, 45, 55, 70, 50);
                    if (GetBMPType == 3) pe.Graphics.DrawImage(bmpBankAngle, 45, 55, 70, 50);
                    if (GetBMPType == 4) pe.Graphics.DrawImage(bmpIMUBad, 45, 55, 70, 50);
                }
            }
        }

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
