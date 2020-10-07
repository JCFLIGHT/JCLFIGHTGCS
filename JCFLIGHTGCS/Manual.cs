using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace JCFLIGHTGCS
{
    public partial class Manual : Form
    {
        public Manual()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            StreamReader FileOpen;
            FileOpen = File.OpenText("ManualJCFLIGHT.rtf");
            richTextBox1.Rtf = FileOpen.ReadToEnd();
            FileOpen.Close();
        }
    }
}
