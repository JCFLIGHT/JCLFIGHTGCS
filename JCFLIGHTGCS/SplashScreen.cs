using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace JCFLIGHTGCS
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            //TXT_version.Text = "Versão:" + "1.2";

            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string About = string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2} (Rev:{3})", v.Major, v.Minor, v.Build, v.Revision);

            TXT_version.Text = "Versão:" + About;
        }
    }
}
