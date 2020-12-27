using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCFLIGHTGCS
{
    class GetValues
    {
        public static string GetPlatformName;
        public static string GetFirwareName;
        public static string GetFirwareVersion;
        public static string GetCompilerVersion;
        public static string GetBuildDate;
        public static string GetBuildTime;
        public static string PreArmMessage;

        public static int AccNotFilteredX = 0;
        public static int AccNotFilteredY = 0;
        public static int AccNotFilteredZ = 0;
        public static int AccFilteredX = 0;
        public static int AccFilteredY = 0;
        public static int AccFilteredZ = 0;
        public static int GyroNotFilteredX = 0;
        public static int GyroNotFilteredY = 0;
        public static int GyroNotFilteredZ = 0;
        public static int GyroFilteredX = 0;
        public static int GyroFilteredY = 0;
        public static int GyroFilteredZ = 0;
        public static int CompassX = 0;
        public static int CompassY = 0;
        public static int CompassZ = 0;
        public static double ReadBarometer = 0;
        public static double ReadBattVoltage = 0;
        public static double ReadBattCurrent = 0;
        public static byte ReadBattPercentage = 0;
        public static double ReadBattWatts = 0;
        public static int ReadAttitudeRoll = 0;
        public static int ReadAttitudePitch = 0;
        public static int ReadAttitudeYaw = 0;
        public static double ReadTemperature = 0;
        public static int ReadAirSpeed = 0;
        public static int ReadI2CError = 0;
        public static int ReadGroundSpeed = 0;
        public static int ReadGroundCourse = 0;
    }
}
