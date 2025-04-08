namespace SmartSpin.Support
{
    static class Globals
    {
        public const int MAX_AXES = 10;
        public const int MAX_SAMPLES = 30000;  // at 20msec sample rate = 10minutes

        public const double RecordSample = 0.02;   //  20 msec sample rate
        public static int BaseSpindleSpeed = 200;

        // Total axes on system (default = 2) (max = 10)
        public static int TotalAxes = 5;

        public static bool DoShutDown = false;
    }
}
