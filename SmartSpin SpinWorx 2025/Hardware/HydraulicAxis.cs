using SmartSpin.Model;
using SmartSpin.Support;
using System;
using System.IO;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class HydraulicAxis : ControllerAxis
    {
        private readonly string VFFFileName;

        public readonly float KP = 0;
        public readonly float VFFPlus = 0;
        public readonly float VFFMinus = 0;
        public readonly float NullPlus = 0;
        public readonly float NullMinus = 0;

        public float[] VFFPlusTable = new float[26];
        public float[] VFFMinusTable = new float[26];

        public bool InvertEncoder = false;

        public bool InvertDAC = false;

        public HydraulicAxis(Controller _controller, XmlNode setupNode, int axno) : base(_controller, setupNode, axno)
        {
            KP = (float)Parameters.CreateParameter("KP", "f2", 0);
            VFFPlus = (float)Parameters.CreateParameter("VFFPlus", "f2", 0);
            VFFMinus = (float)Parameters.CreateParameter("VFFMinus", "f2", 0);
            NullPlus = (float)Parameters.CreateParameter("NullPlus", "f2", 0);
            NullMinus = (float)Parameters.CreateParameter("NullMinus", "f2", 0);

            InvertEncoder = Parameters.CreateParameter("InvertEncoder", false);
            InvertDAC = Parameters.CreateParameter("InvertDAC", false);

            VFFFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"SmartSpinVFF{axno}.xml");

            ReadVFFTable();

            TimeToMoveFactor = Globals.RecordSample;
        }

        public void MintNetInteger(NetDataParam param, int value)
        {
            controller.MintNetInteger(param, value);
        }

        public void MintNetFloat(NetDataParam param, float value)
        {
            controller.MintNetFloat(param, value);
        }

        internal void DownloadParameters(bool HoldHeadSignal)
        {
            if (!controller.Connected) return;
            controller.MintVariableData("ParentTask", "sKP", KP);
            controller.MintVariableData("ParentTask", "VFFPlusTable", VFFPlusTable);
            controller.MintVariableData("ParentTask", "VFFMinusTable", VFFMinusTable);
            controller.MintVariableData("ParentTask", "sNullPlus", NullPlus);
            controller.MintVariableData("ParentTask", "sNullMinus", NullMinus);

            controller.MintNVLong(NVMemory.Encoder1Direction, (InvertEncoder ? 1 : 0));
            controller.MintNVLong(NVMemory.DAC1Direction, (InvertDAC ? 1 : 0));
            controller.MintNVLong(NVMemory.HoldHeadSignal, (HoldHeadSignal ? 1 : 0));
        }

        internal void DownloadSamples(float[] samples)
        {
            if (!controller.Connected) return;
            controller.MintVariableData("ParentTask", "AxisSamples", samples);
        }

        internal float[] UploadSamples()
        {
            return controller.Connected ? (float[])controller.MintVariableData("ParentTask", "AxisSamples") : [];
        }

        internal double ActualPosition => controller.MintNetFloat(NetDataParam.HydraulicEncoder);
        internal double CommandPosition => controller.MintNetFloat(NetDataParam.HydraulicCommandPosition);

        internal void ReadVFFTable()
        {
            if (File.Exists(VFFFileName))
            {
                try
                {
                    XmlNode data;
                    XmlDocument setupfile = new XmlDocument();
                    setupfile.Load(VFFFileName);
                    VFFPlusTable[0] = VFFPlus;
                    VFFMinusTable[0] = VFFMinus;
                    for (int i = 1; i < VFFPlusTable.Length; i++)
                    {
                        data = setupfile.SelectSingleNode("VFFTable/VFFPlus/Value" + i.ToString());
                        if (data == null)
                            VFFPlusTable[i] = VFFPlusTable[i - 1];
                        else
                            VFFPlusTable[i] = Convert.ToSingle(data.InnerXml);
                        data = setupfile.SelectSingleNode("VFFTable/VFFMinus/Value" + i.ToString());
                        if (data == null)
                            VFFMinusTable[i] = VFFMinusTable[i - 1];
                        else
                            VFFMinusTable[i] = Convert.ToSingle(data.InnerXml);
                    }
                    VFFPlusTable[0] = VFFPlusTable[1];
                    VFFMinusTable[0] = VFFMinusTable[1];
                }
                finally
                {

                }
            }
            else
            {   // if no VFF table then just fill it with the VFFPlus/VFFMinus
                for (int i = 0; i < VFFPlusTable.Length; i++)
                {
                    VFFPlusTable[i] = VFFPlus;
                    VFFMinusTable[i] = VFFMinus;
                }
            }

        }

        internal void UploadNewVFFTable()
        {
            float[] VFFPlus = (float[])controller.MintVariableData("ParentTask", "NewVFFPlusTable");
            float[] VFFMinus = (float[])controller.MintVariableData("ParentTask", "NewVFFMinusTable");
            VFFPlusTable[0] = VFFPlus[axisno + 5];
            VFFMinusTable[0] = VFFMinus[axisno + 5];
            for (int j = 1; j < 20; j++)
            {
                VFFPlusTable[j] = VFFPlus[axisno + j * 5];
                VFFMinusTable[j] = VFFMinus[axisno + j * 5];
            }
            for (int j = 20; j < 26; j++)
            {
                VFFPlusTable[j] = VFFPlus[axisno + 19 * 5];
                VFFMinusTable[j] = VFFMinus[axisno + 19 * 5];
            }
            WriteVFFTable();
        }

        internal void WriteVFFTable()
        {
            try
            {
                XmlDocument setupfile = new XmlDocument();
                XmlElement elemDocument = setupfile.CreateElement("VFFTable");
                XmlElement elemPlus = setupfile.CreateElement("VFFPlus");
                XmlElement elemMinus = setupfile.CreateElement("VFFMinus");
                for (int i = 1; i < 19; i++)
                {

                    XmlElement elem = setupfile.CreateElement("Value" + i.ToString());
                    XmlText text = setupfile.CreateTextNode(VFFPlusTable[i].ToString("F4"));
                    elemPlus.AppendChild(elem);
                    elemPlus.LastChild.AppendChild(text);

                    elem = setupfile.CreateElement("Value" + i.ToString());
                    text = setupfile.CreateTextNode(VFFMinusTable[i].ToString("F4"));
                    elemMinus.AppendChild(elem);
                    elemMinus.LastChild.AppendChild(text);
                }
                elemDocument.AppendChild(elemPlus);
                elemDocument.AppendChild(elemMinus);
                setupfile.AppendChild(elemDocument);
                setupfile.Save(VFFFileName);
            }
            finally
            {

            }
        }

        internal SampledPoints MakeAMove(double MoveFrom, double MoveTo, double OverrideFactor = 1)
        {
            return MakeAMove(MoveFrom, MoveTo, MaxVelocity * OverrideFactor, Accel * OverrideFactor);
        }

        internal SampledPoints MakeAMove(double MoveFrom, double MoveTo, double NewVel, double NewAccel)
        {
            SampledPoints sp = new SampledPoints();

            double DistanceToGo;
            double DecelVel;

            double CurrentPos = MoveFrom;
            double CurrentVel = 0;
            do
            {
                CurrentVel = CurrentVel + NewAccel;
                if (CurrentVel > NewVel) CurrentVel = NewVel;
                DistanceToGo = Math.Abs(CurrentPos - MoveTo);
                DecelVel = Math.Sqrt(2 * NewAccel * DistanceToGo);
                if (DecelVel < CurrentVel) CurrentVel = DecelVel;
                if (MoveTo > MoveFrom)
                    CurrentPos = CurrentPos + Math.Abs(CurrentVel);
                else
                    CurrentPos = CurrentPos - Math.Abs(CurrentVel);

                if ((MoveTo > MoveFrom) & (CurrentPos > MoveTo)) CurrentPos = MoveTo;
                if ((MoveTo < MoveFrom) & (CurrentPos < MoveTo)) CurrentPos = MoveTo;

                sp.Add(CurrentPos);
            }
            while (CurrentPos != MoveTo);
            return sp;
        }
    }
}
