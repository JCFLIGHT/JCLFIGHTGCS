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
    public partial class Descriptions : UserControl
    {
        public Descriptions()
        {
            InitializeComponent();
            label6.Text = "Plataforma:" + GetValues.GetPlatformName;
            label1.Text = "Nome do Firmware:" + GetValues.GetFirwareName;
            label4.Text = "Versão do Firmware:" + GetValues.GetFirwareVersion;
            label5.Text = "Versão do Compilador:" + GetValues.GetCompilerVersion;
            label2.Text = "Data de compilação do Firmware:" + GetValues.GetBuildDate;
            label3.Text = "Horario de compilação do Firmware:" + GetValues.GetBuildTime;
        }
    }
}
