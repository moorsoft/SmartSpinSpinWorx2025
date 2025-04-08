using SmartSpin.ViewModel;
using System;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class ControllerAxis
    {
        protected readonly Controller controller;

        protected readonly XmlNode SetupNode;

        public readonly int axisno;

        public readonly string Letter;
        public readonly double Tolerance = 1;
        public double Accel = 1;
        public double Decel = 1;
        public double MaxVelocity = 1;
        public readonly bool SwapJogButtonLocations = false;
        public readonly bool SwapJogButtons = false;
        public readonly bool RotaryAxis = false;
        protected double TimeToMoveFactor;

        public Errors ErrorFlag = Errors.NoErrors;

        public readonly ParametersViewModel Parameters;

        public ControllerAxis(Controller _controller, XmlNode setupNode, int axno)
        {
            controller = _controller;
            axisno = axno;
            SetupNode = setupNode;
            if (setupNode == null) return;

            Parameters = new ParametersViewModel(setupNode);
            Letter = Parameters.CreateParameter("Letter", "A");
            _displayMultiplier = Parameters.CreateParameter("DisplayMultiplier", "f2", 1);
            Accel = Parameters.CreateParameter("Accel", "f2", 1);
            Decel = Parameters.CreateParameter("Decel", "f2", Accel);
            MaxVelocity = Parameters.CreateParameter("Vel", "f2", 1);
            MaxVelocity = Parameters.CreateParameter("MaxVelocity", "f2", MaxVelocity);
            Tolerance = Parameters.CreateParameter("Tolerance", "f2", 1);
            SwapJogButtons = Parameters.CreateParameter("SwapJogButtons", false);
            SwapJogButtonLocations = Parameters.CreateParameter("SwapJogButtonLocations", false);
            RotaryAxis = Parameters.CreateParameter("Rotary", false);

            ErrorFlag = Errors.NoErrors;

            TimeToMoveFactor = 1;
        }

        public virtual void DownloadParameters()
        {

        }

        private double _displayMultiplier = 1;

        public double DisplayMultiplier
        {
            get
            {
                return _displayMultiplier*(((RotaryAxis) || Machine.Metric) ? 1 : 25.4);
            }
        }

        public virtual double TimeToMove(double MoveFrom, double MoveTo)
        {
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
