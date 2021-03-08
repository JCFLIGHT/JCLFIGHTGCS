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
    public partial class Vibrations : Form
    {
        public Vibrations()
        {
            InitializeComponent();

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            VibBarX.Value = (int)InertialSensor.get_vibration_level_X();
            VibBarY.Value = (int)InertialSensor.get_vibration_level_Y();
            VibBarZ.Value = (int)InertialSensor.get_vibration_level_Z();
            txt_clip0.Text = InertialSensor._accel_clip_count[0].ToString();
            txt_clip1.Text = InertialSensor._accel_clip_count[1].ToString();
            txt_clip2.Text = InertialSensor._accel_clip_count[2].ToString();
        }
    }
}
