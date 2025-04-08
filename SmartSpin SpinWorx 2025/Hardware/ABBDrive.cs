using System;
using System.Threading;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class ABBDrive : ServoDrive, IServoDrive
    {
        [Flags]
        enum DStatus
        {
            None = 0,
            ServoReady = 1,
            BrakeControl = 2,
            HomeComplete = 4,
            TargetPositionComplete = 8,
            ServoWarning = 0x10
        }
        enum DriveMode
        {
            ClearFault = -1,
            None = 0,
            Home = 1,
            HomeComplete = 2,
            MoveTo = 3,
            MoveComplete = 4,
            JogLeft = 5,
            JogStop = 6,
            JogRight = 7,
            CancelMove = 9
        }


        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private double JogFactor = 1;
        private bool HomePlus = false;

        private byte NodeId = 1;

        public ABBDrive(Controller _controller, XmlNode setupNode, int axno) : base(_controller, setupNode, axno) { }

        #region Connection Routines

        private bool Open(string IPAddress, byte nodeId)
        {
            NodeId = nodeId;
            _connected = false;
            if (controller.Virtual) return true;

            try
            {
                controller.SetEthernetControllerLink(IPAddress);

                _connected = true;

                if (!controller.MintExecuting)
                {
                    controller.DoMintRun();
                    Thread.Sleep(2000); // give it time to startup
                }

                //MaxVelocity = ReadRAMRegisterFloat(ABBDriveRegisters.MaxVelocity);
                //Accel = ReadRAMRegisterFloat(ABBDriveRegisters.Accel);
                //Decel = ReadRAMRegisterFloat(ABBDriveRegisters.Decel);

                return true;
            }
            catch (System.IO.IOException)
            {
                _connected = false;
                throw;
            }
        }

        public void Open(bool PutIntoSimulationMode)
        {
            if (PutIntoSimulationMode)
            {
                _connected = false;
            }
            else
            {
                Open(Parameters.CreateParameter("IP", ""), (byte)1);
            }

            XmlNode foundNode = SetupNode.SelectSingleNode("MinusLimit");
            if (foundNode != null) this.SetMinusLimit((float)(Parameters.CreateParameter("MinusLimit", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("PlusLimit");
            if (foundNode != null) this.SetPlusLimit((float)(Parameters.CreateParameter("PlusLimit", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("MoveAfterHome");
            if (foundNode != null) this.SetMoveAfterHome((float)(Parameters.CreateParameter("MoveAfterHome", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("HomeOffset");
            if (foundNode != null) this.SetHomeOffset((float)(Parameters.CreateParameter("HomeOffset", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("Accel");
            if (foundNode != null) this.SetAccel((float)(Parameters.CreateParameter("Accel", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("Decel");
            if (foundNode != null) this.SetDecel((float)(Parameters.CreateParameter("Decel", "f1", 0) * DisplayMultiplier));
            foundNode = SetupNode.SelectSingleNode("MaxVelocity");
            if (foundNode != null) this.SetMaxVelocity((float)(Parameters.CreateParameter("MaxVelocity", "f1", 0) * DisplayMultiplier));
            HoldEnable = Parameters.CreateParameter("HoldEnableOn", true);
            JogFactor = Parameters.CreateParameter("JogFactor", 1);
            HomePlus = Parameters.CreateParameter("HomePlus", false);

            this.HoldEnableOn(false);

            DownloadParameters();
        }

        public override void DownloadParameters()
        {
            base.DownloadParameters();

            foreach (var i in Parameters)
            {
                if (i.Label == "MinusLimit")
                {
                    this.SetMinusLimit(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "PlusLimit")
                {
                    this.SetPlusLimit(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "MoveAfterHome")
                {
                    this.SetMoveAfterHome(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "HomeOffset")
                {
                    this.SetHomeOffset(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "Accel")
                {
                    this.SetAccel(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "Decel")
                {
                    this.SetDecel(i.ValueAsDouble * DisplayMultiplier);
                }
                if (i.Label == "MaxVelocity")
                {
                    this.SetMaxVelocity(i.ValueAsDouble * DisplayMultiplier);
                }

                if (i.Label == "MoveBetweenMemories")
                {
                    moveBetweenMemories = i.ValueAsBoolean;
                }
            }
            this.HoldEnableOn(false);
        }

        void AddToFaultLog(string s)
        {
            faultLog.Append(s + '\n');
        }
        #endregion


        private void WriteServoMode(DriveMode dm)
        {
            WriteVariable("ServoMode", (int)dm);
        }
        private DriveMode ReadServoMode()
        {
            return (DriveMode)ReadVariableInt32("ServoMode");
        }

        public int DriveStatus()
        {
            if (_connected)
            {
                return ReadVariableInt32("ServoOutputStatus");    // drive status
            }
            return 0;
        }

        public int ProgramError()
        {
            if (_connected)
            {
                return ReadVariableInt32("ServoAlarm");    // Drive Faults
            }
            return 0;
        }

        public bool IsDriveInFault(int Status)
        {
            return false;
        }

        public double ActualPosition()
        {
            if (_connected)
            {
                return ReadVariableFloat("ServoPosition");
            }
            return 0;
        }

        public double TargetPosition()
        {
            if (_connected)
            {
                return ReadVariableFloat("ServoTargetPosition");
            }
            return 0;
        }

        public int PositionError()
        {
            if (_connected)
            {
                return (int)ReadVariableFloat("ServoPositionError");
            }
            return 0;
        }

        //public int DriveInputs()
        //{
        //    if (!Simulation)
        //    {

        //        return ReadRAMRegisterInt32(ABBDriveRegisters.Inputs);
        //    }
        //    return 0;
        //}

        public void MoveTo(double Destination)
        {
            if (!_connected) return;
            WriteVariable("ServoMovePosition", Destination);
            WriteServoMode(DriveMode.MoveTo);
        }

        public bool MoveComplete()
        {
            if (_connected)
            {
                return ReadServoMode() == DriveMode.MoveComplete;
            }
            return true;
        }

        private void SetMinusLimit(double Pos)
        {
            if (!_connected) return;
            WriteVariable("ServoPositionMinusLimit", Pos);
        }

        private void SetPlusLimit(double Pos)
        {
            if (!_connected) return;
            WriteVariable("ServoPositionPlusLimit", Pos);
        }

        private void SetMoveAfterHome(double Pos)
        {
            if (!_connected) return;
            WriteVariable("ServoMoveAfterHome", Pos);
        }

        private void SetHomeOffset(double Pos)
        {
            if (!_connected) return;
            WriteVariable("ServoHomeOffset", Pos);
        }

        private void SetAccel(double AccelValue)
        {
            if (!_connected) return;
            WriteVariable("ServoAccel", (float)AccelValue);
            this.Accel = AccelValue;
        }

        private void SetDecel(double DecelValue)
        {
            if (!_connected) return;
            MaxVelocity = ReadVariableFloat("ServoMaxVelocity");
            WriteVariable("ServoDecel", (float)DecelValue);
            this.Decel = DecelValue;
        }

        private void SetMaxVelocity(double VelocityValue)
        {
            if (!_connected) return;
            WriteVariable("ServoMaxVelocity", (float)VelocityValue);
            this.MaxVelocity = VelocityValue;
        }


        public void JogPlus(double Speed)
        {
            if (!_connected) return;
            WriteVariable("ServoJogSpeed", Speed * JogFactor);
            WriteServoMode(DriveMode.JogLeft);
        }

        public void JogMinus(double Speed)
        {
            if (!_connected) return;
            WriteVariable("ServoJogSpeed", Speed * JogFactor);
            WriteServoMode(DriveMode.JogRight);
        }

        public void JogStop()
        {
            if (!_connected) return;
            WriteServoMode(DriveMode.JogStop);
        }

        public void HoldEnableOn(bool HoldOn)
        {
            if (!_connected) return;
            if ((HoldOn) && (HoldEnable))
                WriteVariable("ServoHoldEnable", 1);
            else
                WriteVariable("ServoHoldEnable", 0);
        }

        public void HomeAxis()
        {
            if (!_connected) return;
            if (HomePlus)
                WriteVariable("ServoHomePlus", 1);
            else
                WriteVariable("ServoHomePlus", 0);
            WriteServoMode(DriveMode.Home);
        }

        public bool HomeComplete()
        {
            if (_connected)
            {
                return (ReadServoMode() == DriveMode.HomeComplete);
            }
            return true;
        }

        public bool Homed
        {
            get
            {
                if (_connected)
                {
                    return ((DStatus)DriveStatus() & DStatus.HomeComplete) == DStatus.HomeComplete;
                }
                return true;
            }
        }

        public void CancelMove()
        {
            if (!_connected) return;
            WriteServoMode(DriveMode.CancelMove);
        }

        public void ResetErrors()
        {
            if (!_connected) return;
            WriteServoMode(DriveMode.ClearFault);
        }

        private bool TestComms()
        {
            LastCommsOK = true;
            DriveStatus();
            return (LastCommsOK);
        }

        private string ErrorMessage(int d)
        {
            return "Fault:" + ((int)d).ToString();
        }

        public override double TimeToMove(double MoveFrom, double MoveTo)
        {
            // distance in 0.01mm
            // MaxVelocity in RPM
            // Accel in msecs to reach 3000RPM

            double Distance = Math.Abs(MoveFrom - MoveTo); 
            double TimeToAccelAndDecel = (MaxVelocity / Accel) + (MaxVelocity / Decel);
            double DistanceByAccelDecel = ((MaxVelocity * MaxVelocity / Accel) + (MaxVelocity * MaxVelocity / Decel)) / 2;
            double result;
            // Does is get to full speed (trapez move) or is it a (triangle move)
            if (DistanceByAccelDecel > Distance)
            {
                result = TimeToAccelAndDecel + ((Distance - DistanceByAccelDecel) / MaxVelocity);
            }
            else
            {
                // This isn't an exact caculation but it'll do
                result = Distance * TimeToAccelAndDecel / DistanceByAccelDecel;
            }
            return result * TimeToMoveFactor * 1000;  // Convert from secs to msecs

        }
   }
}
