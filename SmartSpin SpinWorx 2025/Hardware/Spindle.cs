using SmartSpin.Support;
using SmartSpin.ViewModel;
using System;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class Spindle
    {
        private readonly Controller? controller;

        private bool IgnoreSpindle = false;
        public double MaxSpeed = 2000;
        public double MinSpeed = 200;
        private double SpeedAt50Hz = 967;
        private double TimeTo100Hz = 5000;
        private double TimeTo0Hz = 3333;

        private int[] Accel = { 150, 100, 120, 150 };
        private int[] Decel = { 450, 100, 250, 450 };
        private int[] Braking = { 400, 0, 200, 400 };

        public bool SpindleAvail;

        public ParametersViewModel Parameters;

        internal void ProcessSetupFile(XmlNode setupNode)
        {
            SpindleAvail = (setupNode != null);

            XmlNode data;
            if (SpindleAvail)
            {
                Parameters = new ParametersViewModel(setupNode);

                IgnoreSpindle = Parameters.CreateParameter("IgnoreSpindle", false);

                MaxSpeed = Parameters.CreateParameter("MaxSpeed", "f0", MaxSpeed);
                MinSpeed = Parameters.CreateParameter("MinSpeed", "f0", MinSpeed);
                SpeedAt50Hz = Parameters.CreateParameter("SpeedAt50Hz", "f2", SpeedAt50Hz);
                // Calculated by 100*P104/P167  eg. 100*3.0secs/60BseFreq = 5000msecs
                TimeTo100Hz = Parameters.CreateParameter("TimeTo100Hz", "f0", TimeTo100Hz);

                // Calculated by 100*P105/P167  eg. 100*2.0secs/60BaseFreq = 3333msecs
                TimeTo0Hz = Parameters.CreateParameter("TimeTo0Hz", "f0", TimeTo0Hz);

                Globals.BaseSpindleSpeed = Parameters.CreateParameter("BaseSpindleSpeed", Globals.BaseSpindleSpeed);

                for (int i = 1; i < 4; i++)
                {
                    data = setupNode.SelectSingleNode(String.Format("Die{0}Accel", i));
                    if (data != null) Accel[i] = (int)(Convert.ToDouble(data.InnerXml) * 10);
                    data = setupNode.SelectSingleNode(String.Format("Die{0}Decel", i));
                    if (data != null) Decel[i] = (int)(Convert.ToDouble(data.InnerXml) * 10);
                    data = setupNode.SelectSingleNode(String.Format("Die{0}Braking", i));
                    if (data != null) Braking[i] = (int)(Convert.ToDouble(data.InnerXml));
                }
            }
        }

        public Spindle(Controller? _controller, XmlNode setupNode)
        {
            controller = _controller;

            ProcessSetupFile(setupNode);
 
            // set default speed
            CommandSpeed = 200;
        }

        public void DownloadParameters()
        {

        }

        private double commandSpeed = 0;
        public double CommandSpeed
        {
            get { return commandSpeed; }
            set
            {
                if (commandSpeed != value)
                {
                    commandSpeed = value;
                    if (value > MaxSpeed) commandSpeed = MaxSpeed;
                    if (value < MinSpeed) commandSpeed = MinSpeed;
                    if (!Machine.Simulation) controller?.MintNetFloat(NetDataParam.SpindleSpeed, (float)(commandSpeed));
                }
            }
        }

        private int dieSize = 0;

        internal void SetDieSize(int DieSize)
        {
            if (DieSize != dieSize)
            {
                dieSize = DieSize;
                if (dieSize >= Accel.Length) dieSize = Accel.Length-1;
                if (!Machine.Simulation)
                {
                    controller?.MintNetFloat(NetDataParam.SpindleAccel, (float)(Accel[dieSize]));
                    controller?.MintNetFloat(NetDataParam.SpindleDecel, (float)(Decel[dieSize]));
                    controller?.MintNetFloat(NetDataParam.SpindleBraking, (float)(Braking[dieSize]));
                }
            }
        }

        public int RPM
        {
            get
            {
                if (Machine.Simulation)
                    return 0;
                else
                {
                    return (int)(controller?.MintNetFloat(NetDataParam.SpindleRPM) ?? 0);
                }
            }
        }

        public int Load
        {
            get
            {
                if (Machine.Simulation)
                    return 0;
                else
                    return (int)(((float)(controller?.MintNetFloat(NetDataParam.SpindleLoad) ?? 0)) / 1.5);
            }
        }

        public int Status
        {
            get
            {
                if (Machine.Simulation)
                    return 0;
                else
                    return controller?.MintNetInteger(NetDataParam.SpindleStatus) ?? 0;
            }
        }

        // Time it takes to change speed in mesecs
        public double SpeedChangeTime(double FromRPM, double ToRPM)
        {
            
            double SpeedChangeInHz = 50 * Math.Abs(ToRPM - FromRPM) / SpeedAt50Hz;
            if (ToRPM > FromRPM)
            {
                return Accel[dieSize] * SpeedChangeInHz / 10;
            }
            else
            {
                return Decel[dieSize] * SpeedChangeInHz / 100;
            }
        }

        public void SetSpindleDirection(bool ReverseDirection)
        {
            if (Machine.Simulation) return;
            if (ReverseDirection)
                controller?.MintNetFloat(NetDataParam.SpindleDirection, -1);
            else
                controller?.MintNetFloat(NetDataParam.SpindleDirection, 1);
        }

        public void Start()
        {
            if (Machine.Simulation) return;
            controller?.MintNetInteger(NetDataParam.SpindleCommand, 1);
        }

        public void Stop()
        {
            if (Machine.Simulation) return;
            controller?.MintNetInteger(NetDataParam.SpindleCommand, 2);
        }

        public bool AtSpeed
        {
            get
            {
                if (IgnoreSpindle)
                {
                    return true;
                }
                if (this.Running)
                {
                    if (Math.Abs(commandSpeed - RPM) < 50)
                    {
                        return true;
                    }
                }
                return false;
                // return ((Machine.SpindleStatus & 0x400) == 0x400);
            }
        }

        /// <summary>
        /// Checks if spindle is actually running
        /// </summary>
        public bool Running
        {
            get
            {
                if (IgnoreSpindle)
                {
                    return true;
                }
                return ((Machine.SpindleStatus & 0xF) == 0x7);
            }
        }
    }
}
