using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace JCFLIGHTGCS
{
    class PingTest
    {
        public static bool PingNetwork(string HostNameOrAddress)
        {
            bool PingStatus = false;
            using (Ping Ping = new Ping())
            {
                byte[] Buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                int TimeOut = 4444;

                try
                {
                    PingReply Reply = Ping.Send(HostNameOrAddress, TimeOut, Buffer);
                    PingStatus = (Reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    PingStatus = false;
                }
            }
            return PingStatus;
        }
    }
}
