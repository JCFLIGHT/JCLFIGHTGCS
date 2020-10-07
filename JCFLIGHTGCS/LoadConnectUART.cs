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
    public partial class LoadConnectUART : Form
    {
        public LoadConnectUART()
        {
            InitializeComponent();
            progressBar1.Value = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100) progressBar1.Value += 1;
            if (progressBar1.Value == 100)
            {
                /*if (Flag == 2 && progressBar1.Value == 100)
                {
                    this.Close();
                }
                Flag += 1;*/

                progressBar1.Value = 0;
            }
        }

        private void LoadConnectUART_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
            progressBar1.Value = 0;
        }
    }
}
