using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCFLIGHTGCS
{
    class GetValues
    {
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

        public static int ReadGroundCourse = 0;

        public static int CompassRoll
        { get { return CompassX; } }

        public static int CompassPitch
        { get { return CompassY; } }

        public static int CompassYaw
        { get { return CompassZ; } }

        public static double Barometer
        { get { return ReadBarometer; } }

        public static double BattVoltage
        { get { return ReadBattVoltage; } }

        public static double BattCurrent
        { get { return ReadBattCurrent; } }

        public static byte BattPercentage
        { get { return ReadBattPercentage; } }

        public static double BattWatts
        { get { return ReadBattWatts; } }

        public static int AttitudeRoll
        { get { return ReadAttitudeRoll; } }

        public static int AttitudePitch
        { get { return ReadAttitudePitch; } }

        public static int AttitudeYaw
        { get { return ReadAttitudeYaw; } }

        public static double Temperature
        { get { return ReadTemperature; } }

        public static int GroundCourse
        { get { return ReadGroundCourse; } }
    }
}
