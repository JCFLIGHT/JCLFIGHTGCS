using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    public partial class PreArmNotification : UserControl
    {
        public PreArmNotification()
        {
            InitializeComponent();
        }

        private void PreArmNotification_Load(object sender, EventArgs e)
        {
            label2.Text = "Aguardando comunicação serial";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetValues.PreArmMessage != null) label2.Text = GetValues.PreArmMessage;
        }
    }
}
