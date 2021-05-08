using System;

namespace JCFLIGHTGCS
{
    class InertialSensor
    {
        static double _clip_limit = 7.9;
        public static UInt32[] _accel_clip_count = new UInt32[3];
        static double[] _accel_vibe_filter = new double[3];

        static double ABS(double inputval)
        {
            return inputval > 0 ? inputval : -inputval;
        }

        static double Constrain_Float(double ValueInput, double ValueInputMin, double ValueInputMax)
        {
            return ((ValueInput) < (ValueInputMin) ? (ValueInputMin) : ((ValueInput) > (ValueInputMax) ? (ValueInputMax) : (ValueInput)));
        }

        static double AccelX;
        static double AccelY;
        static double AccelZ;

        static double AlphaX = 1.0f;
        static double AlphaY = 1.0f;
        static double AlphaZ = 1.0f;
        static double OutputFilteredX;
        static double OutputFilteredY;
        static double OutputFilteredZ;

        private static void ApplyX(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaX = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredX += (Sample - OutputFilteredX) * AlphaX;
        }

        static double GetOutputFilteredX()
        {
            return OutputFilteredX;
        }

        private static void ApplyY(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaY = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredY += (Sample - OutputFilteredY) * AlphaY;
        }

        static double GetOutputFilteredY()
        {
            return OutputFilteredY;
        }

        private static void ApplyZ(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaZ = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredZ += (Sample - OutputFilteredZ) * AlphaZ;
        }

        static double GetOutputFilteredZ()
        {
            return OutputFilteredZ;
        }

        static double AlphaX2 = 1.0f;
        static double AlphaY2 = 1.0f;
        static double AlphaZ2 = 1.0f;
        static double OutputFilteredX2;
        static double OutputFilteredY2;
        static double OutputFilteredZ2;

        private static void ApplyX2(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaX2 = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredX2 += (Sample - OutputFilteredX2) * AlphaX2;
        }

        static double GetOutputFilteredX2()
        {
            return OutputFilteredX2;
        }

        private static void ApplyY2(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaY2 = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredY2 += (Sample - OutputFilteredY2) * AlphaY2;
        }

        static double GetOutputFilteredY2()
        {
            return OutputFilteredY2;
        }

        private static void ApplyZ2(double Sample, double CutOff_Freq, double DeltaTime)
        {
            double RecalculateCutOff = 1.0 / (6.283185307179586476925286766559 * CutOff_Freq);
            AlphaZ2 = Constrain_Float(DeltaTime / (DeltaTime + RecalculateCutOff), 0.0, 1.0);
            OutputFilteredZ2 += (Sample - OutputFilteredZ2) * AlphaZ2;
        }

        static double GetOutputFilteredZ2()
        {
            return OutputFilteredZ2;
        }

        public static void AccCalcVibrationAndClipping()
        {
            AccelX = (double)(Convert.ToDouble(GetValues.AccX) / 2048 * 9.80665);

            AccelY = (double)(Convert.ToDouble(GetValues.AccY) / 2048 * 9.80665);

            AccelZ = (double)(Convert.ToDouble(GetValues.AccZ) / 2048);

            if (ABS(AccelX) > _clip_limit)
            {
                _accel_clip_count[0]++;
            }

            if (ABS(AccelY) > _clip_limit)
            {
                _accel_clip_count[1]++;
            }

            if (ABS(AccelZ) > _clip_limit)
            {
                _accel_clip_count[2]++;
            }

            ApplyX(AccelX, 5, 1000 * 1e-6);
            ApplyY(AccelY, 5, 1000 * 1e-6);
            ApplyZ(AccelZ, 5, 1000 * 1e-6);

            double accel_filtX = GetOutputFilteredX();
            double accel_filtY = GetOutputFilteredY();
            double accel_filtZ = GetOutputFilteredZ();

            double accel_diffX = (AccelX - accel_filtX);
            double accel_diffY = (AccelY - accel_filtY);
            double accel_diffZ = (AccelZ - accel_filtZ);

            accel_diffX *= accel_diffX;
            accel_diffY *= accel_diffY;
            accel_diffZ *= accel_diffZ;

            ApplyX2(accel_diffX, 2, 1000 * 1e-6);
            ApplyY2(accel_diffY, 2, 1000 * 1e-6);
            ApplyZ2(accel_diffZ, 2, 1000 * 1e-6);

            _accel_vibe_filter[0] = GetOutputFilteredX2();
            _accel_vibe_filter[1] = GetOutputFilteredY2();
            _accel_vibe_filter[2] = GetOutputFilteredZ2();
        }

        public static double get_vibration_level_X()
        {
            double vibe;

            vibe = _accel_vibe_filter[0];
            vibe = Math.Sqrt(vibe);

            return vibe;
        }

        public static double get_vibration_level_Y()
        {
            double vibe;

            vibe = _accel_vibe_filter[1];
            vibe = Math.Sqrt(vibe);

            return vibe;
        }

        public static double get_vibration_level_Z()
        {
            double vibe;

            vibe = _accel_vibe_filter[2];
            vibe = Math.Sqrt(vibe);

            return vibe;
        }

    }
}
