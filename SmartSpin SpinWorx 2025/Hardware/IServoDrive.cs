namespace SmartSpin.Hardware
{
    public interface IServoDrive
    {
        bool Simulation { get; }

        void Open(bool PutIntoSimulationMode);

        int DriveStatus();

        int ProgramError();

        bool IsDriveInFault(int Status);

        double ActualPosition();

        double TargetPosition();

        int PositionError();

        void MoveTo(double Destination);

        bool MoveComplete();

        void JogPlus(double Speed);

        void JogMinus(double Speed);

        void JogStop();

        void HoldEnableOn(bool HoldOn);

        void HomeAxis();

        bool HomeComplete();

        bool Homed { get; }

        bool MoveBetweenMemories { get; }

        void CancelMove();

        void ResetErrors();

        double TimeToMove(double MoveFrom, double MoveTo);
    }
}
