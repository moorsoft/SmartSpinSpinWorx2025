using SmartSpin.Model;
using SmartSpin.Support;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public HydraulicAxis(Controller _controller, JsonNode setupNode, int axno) : base(_controller, setupNode, axno)
        {
            KP = (float)Parameters.CreateParameter("KP", "f2", 0);
            VFFPlus = (float)Parameters.CreateParameter("VFFPlus", "f2", 0);
            VFFMinus = (float)Parameters.CreateParameter("VFFMinus", "f2", 0);
            NullPlus = (float)Parameters.CreateParameter("NullPlus", "f2", 0);
            NullMinus = (float)Parameters.CreateParameter("NullMinus", "f2", 0);

            InvertEncoder = Parameters.CreateParameter("InvertEncoder", false);
            InvertDAC = Parameters.CreateParameter("InvertDAC", false);

            VFFFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"SmartSpinVFF{axno}.json");

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
                    JsonNode setupfile = JsonNode.Parse(File.ReadAllText(VFFFileName));
                    if ((setupfile["VFFPlus"] is JsonArray arrayPlus) && (setupfile["VFFMinus"] is JsonArray arrayMinus))
                    {
                        VFFPlusTable[0] = VFFPlus;
                        VFFMinusTable[0] = VFFMinus;
                        for (int i = 1; i < VFFPlusTable.Length; i++)
                        {
                            if (i < arrayPlus.Count)
                            {
                                VFFPlusTable[i] = arrayPlus[i]?.GetValue<float>() ?? VFFPlus;
                            }
                            else
                            {
                                VFFPlusTable[i] = VFFPlusTable[i - 1];
                            }
                            if (i < arrayMinus.Count)
                            {
                                VFFMinusTable[i] = arrayMinus[i]?.GetValue<float>() ?? VFFMinus;
                            }
                            else
                            {
                                VFFMinusTable[i] = VFFMinusTable[i - 1];
                            }
                        }
                        VFFPlusTable[0] = VFFPlusTable[1];
                        VFFMinusTable[0] = VFFMinusTable[1];
                    }
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
                var plusArray = new JsonArray();
                var minusArray = new JsonArray();

                for (int i = 1; i < 19; i++)
                {
                    plusArray.Add(VFFPlusTable[i]);
                    minusArray.Add(VFFMinusTable[i]);
                }

                JsonNode setupfile = new JsonObject();
                setupfile["VFFPlus"] = plusArray;
                setupfile["VFFMinus"] = minusArray;

                File.WriteAllText(VFFFileName, setupfile.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
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
                CurrentVel += NewAccel;
                if (CurrentVel > NewVel) CurrentVel = NewVel;
                DistanceToGo = Math.Abs(CurrentPos - MoveTo);
                DecelVel = Math.Sqrt(2 * NewAccel * DistanceToGo);
                if (DecelVel < CurrentVel) CurrentVel = DecelVel;
                if (MoveTo > MoveFrom)
                    CurrentPos += Math.Abs(CurrentVel);
                else
                    CurrentPos -= Math.Abs(CurrentVel);

                if ((MoveTo > MoveFrom) & (CurrentPos > MoveTo)) CurrentPos = MoveTo;
                if ((MoveTo < MoveFrom) & (CurrentPos < MoveTo)) CurrentPos = MoveTo;

                sp.Add(CurrentPos);
            }
            while (CurrentPos != MoveTo);
            return sp;
        }
    }
}
