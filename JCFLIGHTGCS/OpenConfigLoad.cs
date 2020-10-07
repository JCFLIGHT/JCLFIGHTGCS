using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    public partial class OpenConfigLoad : Form
    {
        public OpenConfigLoad()
        {
            InitializeComponent();
        }

        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100) progressBar1.Value += 1;
            if (progressBar1.Value == 100) progressBar1.Value = 0;
        }
    }
}
