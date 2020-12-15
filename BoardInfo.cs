using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    public partial class BoardInfo : Form
    {
        public BoardInfo()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label7.Text = "Mag X:" + GetValues.CompassRoll;
            label8.Text = "Mag Y:" + GetValues.CompassPitch;
            label9.Text = "Mag Z:" + GetValues.CompassYaw;
            label11.Text = "Altitude(Baro):" + GetValues.Barometer;
            label18.Text = "Tensão:" + GetValues.BattVoltage;
            label27.Text = "Porcentagem:" + GetValues.BattPercentage;
            label19.Text = "Corrente:" + GetValues.BattCurrent;
            label10.Text = "Watts:" + GetValues.BattWatts;
            label30.Text = "Attitude Roll:" + GetValues.AttitudeRoll;
            label29.Text = "Attitude Pitch:" + GetValues.AttitudePitch;
            label28.Text = "Attitude Yaw:" + GetValues.AttitudeYaw;
            label31.Text = "Temperatura(Baro):" + GetValues.Temperature;
            label21.Text = "GroudCourse (GPS):" + GetValues.GroundCourse;
        }
    }
}
