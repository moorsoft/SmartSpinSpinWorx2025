using System;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class DeltaDrive : ServoDrive, IServoDrive
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

        public DeltaDrive(Controller _controller, XmlNode setupNode, int axno) : base(_controller, setupNode, axno) { }

        #region Connection routines

        private bool Open(string IPAddress, byte nodeId)
        {
            try
            {
                controller.SetEthernetControllerLink(IPAddress);

                NodeId = nodeId;
                _connected = true;

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
                Open(Parameters.CreateParameter("IP", ""), (byte)Parameters.CreateParameter("NodeId", 0));
                
            }
            Parameters.CreateParameter("MoveAfterHome", "f1", 0);
            Parameters.CreateParameter("HomeOffset", "f1", 0);
            HoldEnable = Parameters.CreateParameter("HoldEnableOn", true);
            JogFactor = Parameters.CreateParameter("JogFactor", 1);
            HomePlus = Parameters.CreateParameter("HomePlus", false);
            moveBetweenMemories = Parameters.CreateParameter("MoveBetweenMemories", true);

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
                if (i.Label == "MoveBetweenMemories")
                {
                    moveBetweenMemories = i.ValueAsBoolean;
                }
            }
            if (_connected)
            {
                WriteVariable("ServoMaxVelocity", MaxVelocity);
                WriteVariable("ServoAccel", Accel);
                WriteVariable("ServoDecel", Decel);
            }
            this.HoldEnableOn(false);
        }

        public void Close()
        {
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
