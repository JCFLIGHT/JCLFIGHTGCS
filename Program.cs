using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    static class Program
    {
        public static SplashScreen Splash;
        public static LoadConnectUART WaitUart;
        public static Reboot RebootBoard;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Splash = new SplashScreen();
            WaitUart = new LoadConnectUART();
            RebootBoard = new Reboot();
            Splash.Show();
            Application.DoEvents();
            Application.DoEvents();
            Application.Run(new GCS());
        }
    }
}