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
    public partial class GCSConfigurations : Form
    {
        bool MessageRead = false;

        public GCSConfigurations()
        {
            InitializeComponent();
        }

        private void GCSConfigurations_Load(object sender, EventArgs e)
        {
            MessageRead = false;
            numericUpDown1.Value = GetValues.GCSFrequency;
            checkBox1.Checked = Convert.ToBoolean(GetValues.GCSSpeech);
            checkBox2.Checked = Convert.ToBoolean(GetValues.GCSRebootBoard);
            checkBox3.Checked = Convert.ToBoolean(GetValues.GCSAutoWP);
            numericUpDown2.Value = GetValues.GCSTrackLength;
            checkBox4.Checked = Convert.ToBoolean(GetValues.GCSAirPorts);
            numericUpDown3.Value = GetValues.GCSTrackSize;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (numericUpDown1.Value == 5)
            {
                GCSSettings.GCSRate = 200;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 10)
            {
                GCSSettings.GCSRate = 100;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 15)
            {
                GCSSettings.GCSRate = 66;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 20)
            {
                GCSSettings.GCSRate = 50;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 25)
            {
                GCSSettings.GCSRate = 40;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 30)
            {
                GCSSettings.GCSRate = 33;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 35)
            {
                GCSSettings.GCSRate = 28;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 40)
            {
                GCSSettings.GCSRate = 25;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 45)
            {
                GCSSettings.GCSRate = 22;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }
            else if (numericUpDown1.Value == 50)
            {
                GCSSettings.GCSRate = 20;
                GCSSettings.GCSFrequency = (byte)numericUpDown1.Value;
            }

            if (checkBox1.Checked == true)
            {
                GCSSettings.GCSSpeech = 1;
            }
            else
            {
                GCSSettings.GCSSpeech = 0;
            }

            if (checkBox2.Checked == true)
            {
                GCSSettings.GCSRebootBoard = 1;
            }
            else
            {
                GCSSettings.GCSRebootBoard = 0;
            }

            if (checkBox3.Checked == true)
            {
                GCSSettings.GCSAutoWP = 1;
            }
            else
            {
                GCSSettings.GCSAutoWP = 0;
            }

            GCSSettings.GCSTrackLength = (int)numericUpDown2.Value;
        
            if (checkBox4.Checked == true)
            {
                GCSSettings.GCSAirPorts = 1;
            }
            else
            {
                GCSSettings.GCSAirPorts = 0;
            }

            GCSSettings.GCSTrackSize = (int)numericUpDown3.Value;

            Save_To_XML();
        }

        private void Save_To_XML()
        {
            GCSSettings.Save_To_XML(Settings.GetRunningDirectory() + "GCSSettings.xml");
        }

        private void GCSConfigurations_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!MessageRead)
            {
                MessageBox.Show("É necessario reiniciar o GCS para aplicar os novos parâmetros");
            }
            MessageRead = !MessageRead;
        }
    }
}
