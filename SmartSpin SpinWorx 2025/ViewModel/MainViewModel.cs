using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
//using SharpDX.DirectInput;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using SmartSpin.Model;
using SmartSpin.Properties;
using SmartSpin.Support;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmartSpin.ViewModel
{
    public partial class MainViewModel : MyViewModelBase
    {
        //private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ILogger logger;

        internal DateTime DisableMachineDelay = DateTime.MaxValue;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private bool ContinuousPlay = false;

        private DateTime ProgressBarStartTime;
        private DateTime StartRecordTime;

        private bool NewStartButton = false;
        private bool LastStartButton = false;

        private bool DecreaseSpindleSpeedO = false;
        private bool IncreaseSpindleSpeedO = false;

        //private readonly Guid joystickGuid = Guid.Empty;
        //private readonly DirectInput directInput = new DirectInput();
        //private readonly Joystick joystick;


        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private string firmware;

        [ObservableProperty]
        private bool inCycle;

        [ObservableProperty]
        private double samplesRecorded;

        [ObservableProperty]
        private double progressValue;
        [ObservableProperty]
        private double progressMaximum;
        [ObservableProperty]
        private int nextMemoryToRecord;

        [ObservableProperty]
        private int memoriesUsed;

        [ObservableProperty]
        private double playingTime;

        [ObservableProperty]
        private int spindleRPM;

        [ObservableProperty]
        private int spindleLoad;

        [ObservableProperty]
        private string statusLabel = "Machine Connected";

        [ObservableProperty]
        private long loopTime;

        [ObservableProperty]
        private string playingStatus = "Idle";
        [ObservableProperty]
        private string unitsSelectedName;


        [ObservableProperty]
        private double startPosition1;

        [ObservableProperty]
        private double startPosition2;

        [ObservableProperty]
        private double startPositionZ;

        [ObservableProperty]
        private double startPositionX;

        [ObservableProperty]
        private double startPositionB;


        [ObservableProperty]
        private double position0;

        [ObservableProperty]
        private double position1;

        [ObservableProperty]
        private double position2;

        [ObservableProperty]
        private double position3;

        [ObservableProperty]
        private double position4;

        [ObservableProperty]
        private double position5;

        [ObservableProperty]
        private double position6;

        [ObservableProperty]
        private double position7;

        [ObservableProperty]
        private double position8;

        [ObservableProperty]
        private double position9;

        [ObservableProperty]
        private double positionZ;

        [ObservableProperty]
        private double positionX;

        [ObservableProperty]
        private double positionB;


        [ObservableProperty]
        private double positionLag0;

        [ObservableProperty]
        private double positionLag1;

        [ObservableProperty]
        private double positionLag2;

        [ObservableProperty]
        private double positionLag3;

        [ObservableProperty]
        private double positionLag4;

        [ObservableProperty]
        private double positionLag5;

        [ObservableProperty]
        private double positionLag6;

        [ObservableProperty]
        private double positionLag7;

        [ObservableProperty]
        private double positionLag8;

        [ObservableProperty]
        private double positionLag9;

        [ObservableProperty]
        private double positionLagZ;

        [ObservableProperty]
        private double positionLagX;

        [ObservableProperty]
        private double positionLagB;

        [ObservableProperty]
        private int settingsSelected;

        [ObservableProperty]
        private double settingTotalPlaybackTime;

        [ObservableProperty]
        private double settingPlaybackSpeed;

        [ObservableProperty]
        private double settingSpindleSpeed;

        [ObservableProperty]
        private int settingFormerCycle;

        [ObservableProperty]
        private double settingOffset1;

        [ObservableProperty]
        private double settingOffset2;

        [ObservableProperty]
        private double settingOffsetZ;

        [ObservableProperty]
        private double settingOffsetX;

        [ObservableProperty]
        private double settingOffsetB;

        [ObservableProperty]
        private bool useCentreDevice;

        [ObservableProperty]
        private bool useBackStopDevice;


        [ObservableProperty]
        private int formSelected;

        [ObservableProperty]
        private int headSelected;


        [ObservableProperty]
        private string buttonLabelPlus1;

        [ObservableProperty]
        private string buttonLabelMinus1;

        [ObservableProperty]
        private string buttonLabelPlus2;

        [ObservableProperty]
        private string buttonLabelMinus2;

        [ObservableProperty]
        private string buttonLabelPlusZ;

        [ObservableProperty]
        private string buttonLabelMinusZ;

        [ObservableProperty]
        private string buttonLabelPlusX;

        [ObservableProperty]
        private string buttonLabelMinusX;

        [ObservableProperty]
        private string buttonLabelPlusB;

        [ObservableProperty]
        private string buttonLabelMinusB;


        [ObservableProperty]
        private bool zAxisVisible;

        [ObservableProperty]
        private bool xAxisVisible;

        [ObservableProperty]
        private bool bAxisVisible;

        [ObservableProperty]
        private bool canChangeHead;

        [ObservableProperty]
        private bool centreDeviceAttached;

        [ObservableProperty]
        private bool formerAttached;

        [ObservableProperty]
        private bool frontFormerAttached;

        [ObservableProperty]
        private bool topFormerAttached;

        [ObservableProperty]
        private bool backStopAttached;


        [ObservableProperty]
        private bool isSpindleReversed;

        [ObservableProperty]
        private bool parametersVisible;


        [ObservableProperty]
        private string programNote;


        [ObservableProperty]
        private MachineParameterViewModel parameters = new MachineParameterViewModel();

        public int PartsCounter
        {
            get => Machine.CurrentProgram.PartsCounter;
            set => SetProperty(Machine.CurrentProgram.PartsCounter, value, Machine.CurrentProgram, (u, n) => u.PartsCounter = n);
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ILogger<MainViewModel> logger)
        {
            this.logger = logger;
            buttonLabelPlus1 = (Machine.Axes[0]?.SwapJogButtonLocations ?? false ? "-" : "+");
            buttonLabelMinus1 = (Machine.Axes[0]?.SwapJogButtonLocations ?? false ? "+" : "-");
            buttonLabelPlus2 = (Machine.Axes[1]?.SwapJogButtonLocations ?? false ? "-" : "+");
            buttonLabelMinus2 = (Machine.Axes[1]?.SwapJogButtonLocations ?? false ? "+" : "-");
            buttonLabelPlusZ = (Machine.ZAxis?.SwapJogButtonLocations ?? false ? "L" : "R");
            buttonLabelMinusZ = (Machine.ZAxis?.SwapJogButtonLocations ?? false ? "R" : "L");
            buttonLabelPlusX = (Machine.XAxis?.SwapJogButtonLocations ?? false ? "I" : "O");
            buttonLabelMinusX = (Machine.XAxis?.SwapJogButtonLocations ?? false ? "O" : "I");
            buttonLabelPlusB = (Machine.BAxis?.SwapJogButtonLocations ?? false ? "CW" : "CCW");
            buttonLabelMinusB = (Machine.BAxis?.SwapJogButtonLocations ?? false ? "CCW" : "CW");

            zAxisVisible = Machine.ZAxis != null;
            xAxisVisible = Machine.XAxis != null;
            bAxisVisible = Machine.BAxis != null;
            canChangeHead = Machine.CanChangeHead;
            centreDeviceAttached = Machine.CenterAttached;
            formerAttached = Machine.FrontFormerAttached || Machine.TopFormerAttached;
            topFormerAttached = Machine.TopFormerAttached;
            frontFormerAttached = Machine.FrontFormerAttached;
            backStopAttached = Machine.BackStopAttached;
            parameters.Machine = Machine.Parameters;
            parameters.Axis1 = Machine.Axes[0].Parameters;
            parameters.Axis2 = Machine.Axes[1].Parameters;
            parameters.AxisZ = Machine.ZAxis.Parameters;
            parameters.AxisX = Machine.XAxis.Parameters;
            parameters.AxisB = Machine.BAxis?.Parameters;
            parameters.AxisX1 = Machine.X1Axis?.Parameters;
            parameters.AxisY1 = Machine.Y1Axis?.Parameters;
            parameters.AxisZ1 = Machine.Z1Axis?.Parameters;
            parameters.Spindle = Machine.SpindleAxis.Parameters;

            Title = "Smart Spin";

            //if (IsInDesignMode)
            //{
            //}
            //else
            {
                Machine.FinishedPlayBack += new EventHandler(PlayBackComplete);

                logger.LogInformation("Program Start");

                UpdateUnitsName();
                UpdateSettingsPage();

                Machine.TransferProgramSettings();

                //foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                //    joystickGuid = deviceInstance.InstanceGuid;

                //// If Gamepad not found, look for a Joystick
                //if (joystickGuid == Guid.Empty)
                //{
                //    foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                //        joystickGuid = deviceInstance.InstanceGuid;
                //}

                //// If Joystick not found, throws an error
                //if (joystickGuid != Guid.Empty)
                //{

                //    // Instantiate the joystick
                //    joystick = new Joystick(directInput, joystickGuid);

                //    // Set BufferSize in order to use buffered data.
                //    //joystick.Properties.BufferSize = 128;

                //    // Acquire the joystick
                //    joystick.Acquire();

                //    // Poll events from joystick
                //    //while (true)
                //    //{
                //    //joystick.Poll();
                //    //var datas = joystick.GetBufferedData();
                //    //foreach (var state in datas)
                //    //    Console.WriteLine(state);
                //    //}
                //}


                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                dispatcherTimer.Start();
            }
        }

        void UpdateUnitsName()
        {
            UnitsSelectedName = (Machine.Metric ? "Metric" : "Inch");
            OnPropertyChanged(nameof(SettingOffset1));
            OnPropertyChanged(nameof(SettingOffset2));
            OnPropertyChanged(nameof(SettingOffsetZ));
            OnPropertyChanged(nameof(SettingOffsetX));
            OnPropertyChanged(nameof(SettingOffsetB));
        }

        private double SamplesToSecs(double samples, int mem = 0)
        {
            return samples * Globals.RecordSample * (100 / Machine.CurrentProgram.MemS[0].PlaybackSpeed) * (mem > 0 ? (100 / Machine.CurrentProgram.MemS[mem].PlaybackSpeed) : 1);
        }

        private void UpdateSampleTimes()
        {
            if (SettingsSelected == 0)
            {
                double T = 0;
                for (int i = 1; i <= 10; i++)
                {
                    T += ((double)Machine.CurrentProgram.MemS[i].TotalSamples * (100 / Machine.CurrentProgram.MemS[i].PlaybackSpeed));
                    SettingTotalPlaybackTime = SamplesToSecs(T);
                }
            }
            else
                SettingTotalPlaybackTime = SamplesToSecs(Machine.CurrentProgram.MemS[SettingsSelected].TotalSamples, SettingsSelected);
        }

        //private bool JoystickJoggingZ = false;
        //private bool JoystickJoggingX = false;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                Machine.UpdateStatus();
                //if (joystick != null)
                //{
                //    JoystickState joystate = joystick.GetCurrentState();
                //    int Z = (int)Math.Round((joystate.X - 32767) / 327.67);
                //    int X = (int)Math.Round((joystate.RotationY - 32767) / 327.67);
                //    if (Z != 0)
                //    {
                //        Machine.Jog(2, Z);
                //        JoystickJoggingZ = true;
                //    }
                //    else
                //    {
                //        if (JoystickJoggingZ)
                //        {
                //            Machine.Jog(2, 0);
                //            JoystickJoggingZ = false;
                //        }
                //    }
                //    if (X != 0)
                //    {
                //        Machine.Jog(3, X);
                //        JoystickJoggingX = true;
                //    }
                //    else
                //    {
                //        if (JoystickJoggingX)
                //        {
                //            Machine.Jog(3, 0);
                //            JoystickJoggingX = false;
                //        }
                //    }
                //}

                if (!Machine.Playing)
                {
                    CheckSpindleJog();

                    ////if (tabDiagnostics.Visible)
                    ////{
                    ////    lblInputs.Text = Machine.Inputs.ToString("X6");
                    ////    lblDriveStatM.Text = ((int)Machine.DriveStatus[2]).ToString("X8");
                    ////    lblDriveInputsM.Text = Machine.DriveInputs[2].ToString("X4");
                    ////    if (Globals.TotalAxes > 3)
                    ////    {
                    ////        lblDriveStatM2.Text = ((int)Machine.DriveStatus[3]).ToString("X8");
                    ////        lblDriveInputsM2.Text = Machine.DriveInputs[3].ToString("X4");
                    ////    }
                    ////    if (Globals.TotalAxes > 3)
                    ////    {
                    ////        lblDriveStatM3.Text = ((int)Machine.DriveStatus[4]).ToString("X8");
                    ////        lblDriveInputsM3.Text = Machine.DriveInputs[4].ToString("X4");
                    ////    }
                    ////    lblSpindleStatus.Text = Machine.SpindleStatus.ToString("X4");
                    ////}
                    if (Machine.Recording)
                    {
                        Machine.CheckRecording();
                        SamplesRecorded = SamplesToSecs(Machine.RecordPointer);
                    }

                    NewStartButton = Machine.StartButtonPressed();
                    if ((NewStartButton) & (!LastStartButton) & (!Machine.Playing)) Play();
                    LastStartButton = NewStartButton;

                    if (ContinuousPlay) Play();
                }

                if (DateTime.Now.CompareTo(DisableMachineDelay) > 0)
                {
                    DisableMachineDelay = DateTime.MaxValue;
                    Machine.DisableMachine();
                }

                InCycle = Machine.Playing;
                if (Machine.Playing)
                {
                    double i = DateTime.Now.Subtract(ProgressBarStartTime).TotalMilliseconds;
                    PlayingTime = i / 1000;
                    //string.Format("Playing Time {0:F1} secs", (i) / 1000);
                    if (i > ProgressMaximum) i = ProgressMaximum;
                    ProgressValue = i;
                }

                double pos = Machine.CurPosition(0);
                Position0 = pos / Machine.Axes[0].DisplayMultiplier;
                PositionLag0 = (pos - Machine.CommandPosition(0));

                if (Globals.TotalAxes > 1)
                {
                    pos = Machine.CurPosition(1);
                    Position1 = pos / Machine.Axes[1].DisplayMultiplier;
                    PositionLag1 = (pos - Machine.CommandPosition(1));
                }
                if (Machine.ServoDrive[2] != null)
                {
                    pos = Machine.CurPosition(2);
                    Position2 = pos / Machine.Axes[2].DisplayMultiplier;
                    PositionLag2 = (pos - Machine.CommandPosition(2));
                    PositionZ = Position2;
                    PositionLagZ = PositionLag2;
                }
                if (Machine.ServoDrive[3] != null)
                {
                    pos = Machine.CurPosition(3);
                    Position3 = pos / Machine.Axes[3].DisplayMultiplier;
                    PositionLag3 = (pos - Machine.CommandPosition(3));
                    PositionX = Position3;
                    PositionLagX = PositionLag3;
                }
                if (Machine.ServoDrive[4] != null)
                {
                    pos = Machine.CurPosition(4);
                    Position4 = pos / Machine.Axes[4].DisplayMultiplier;
                    PositionLag4 = (pos - Machine.CommandPosition(4));
                    PositionB = Position4;
                    PositionLagB = PositionLag4;
                }
                if (Machine.ServoDrive[5] != null)
                {
                    pos = Machine.CurPosition(5);
                    Position5 = pos / Machine.Axes[5].DisplayMultiplier;
                    PositionLag5 = (pos - Machine.CommandPosition(5));
                }
                if (Machine.ServoDrive[6] != null)
                {
                    pos = Machine.CurPosition(6);
                    Position6 = pos / Machine.Axes[6].DisplayMultiplier;
                    PositionLag6 = (pos - Machine.CommandPosition(6));
                }
                if (Machine.ServoDrive[7] != null)
                {
                    pos = Machine.CurPosition(7);
                    Position7 = pos / Machine.Axes[7].DisplayMultiplier;
                    PositionLag7 = (pos - Machine.CommandPosition(7));
                }
                if (Machine.ServoDrive[8] != null)
                {
                    pos = Machine.CurPosition(8);
                    Position8 = pos / Machine.Axes[8].DisplayMultiplier;
                    PositionLag8 = (pos - Machine.CommandPosition(8));
                }
                if (Machine.ServoDrive[9] != null)
                {
                    pos = Machine.CurPosition(9);
                    Position9 = pos / Machine.Axes[9].DisplayMultiplier;
                    PositionLag9 = (pos - Machine.CommandPosition(9));
                }

                if (Machine.SpindleAxis.SpindleAvail)
                {
                    SpindleRPM = Machine.SpindleAxis.RPM;
                    SpindleLoad = Machine.SpindleAxis.Load;
                    if (SpindleLoad > 100) SpindleLoad = 100;
                }

                if (!Machine.MachineOK)
                {
                    if (Machine.Playing) StopPlay();
                }

                StatusLabel = Machine.ErrorMessage;

                MemoriesUsed = Machine.CurrentProgram.MemoriesUsed;

                PlayingStatus = Machine.PlayingStatusString();
                LoopTime = sw.ElapsedMilliseconds;
                if ((!Machine.LongOperation) && (LoopTime > 500))
                {
                    logger.LogError("timer time {LoopTime}", LoopTime);
                }
            }
            finally
            {
                dispatcherTimer.Start();
            }
        }

        public void LoadProgram(string FileName)
        {
            if (String.IsNullOrEmpty(FileName))
            {
                // Machine.CurrentProgram.NewProgram();
                Machine.CurrentProgram = new SpinProgram();
            }
            else
            {
                // Save File Path for next time
                Settings.Default.FileOpenPath = Path.GetDirectoryName(FileName);
                Settings.Default.LastFileName = FileName;
                Machine.CurrentProgram = SpinProgram.LoadProgram(FileName);
                //Machine.CurrentProgram.SaveProgramAsXml(Path.ChangeExtension(FileName, ".xml"));
                Machine.CurrentProgram.ProgramFileName = FileName;
                Machine.TransferProgramSettings();
                //!btnPlay.Enabled = true;
            }
            OnPropertyChanged(nameof(PartsCounter));
            Title = "Smart Spin - " + Machine.CurrentProgram.ProgramFileName;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void New()
        {
            if (MyMessageBox.Show("Are you sure you want to clear your current program from memory.", "Query", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                logger.LogInformation("New Program.");
                Machine.CurrentProgram = new SpinProgram(); // .NewProgram();
                Machine.TransferProgramSettings();
                Title = "Smart Spin - " + Machine.CurrentProgram.ProgramFileName;

                OnPropertyChanged(nameof(PartsCounter));
                SamplesRecorded = 0;

                UpdateSettingsPage();
            }
        }

        [RelayCommand]
        public void Open()
        {
            OpenSaveDialog osd = new OpenSaveDialog
            {
                DialogType = OpenSaveDialogType.Open,
                DefaultExt = "spn"
            };
            osd.SetOpeningFolder(Settings.Default.FileOpenPath);
            osd.FileName = Machine.CurrentProgram.ProgramFileName;
            if (osd.ShowDialog() ?? false)
            {
                logger.LogInformation("Open Program : {FileName}", osd.FileName);
                LoadProgram(osd.FileName);
                if (MyMessageBox.Show("Do you want to clear the Parts Counter.", "Query", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ResetPartsCounter();
                }
            }
        }

        [RelayCommand]
        public void Save()
        {
            OpenSaveDialog osd = new OpenSaveDialog
            {
                DialogType = OpenSaveDialogType.Save,
                DefaultExt = "spn"
            };
            osd.SetOpeningFolder(Settings.Default.FileOpenPath);
            osd.FileName = Machine.CurrentProgram.ProgramFileName;

            if (osd.ShowDialog() ?? false)
            {
                logger.LogInformation("Save Program : {FileName}", osd.FileName);
                // Save File Path for next time
                Settings.Default.FileOpenPath = Path.GetDirectoryName(osd.FileName);
                Settings.Default.LastFileName = osd.FileName;
                try
                {
                    Machine.CurrentProgram.SaveProgram(osd.FileName);
                }
                catch (Exception e) 
                {
                    MyMessageBox.Show($"Unable to save file. {e.Message}");

                }
                Title = "Smart Spin - " + Machine.CurrentProgram.ProgramFileName;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecutePlay))]
        public void Play()
        {
            // This is to make sure it doesn't try and Play again until Start Button released.
            LastStartButton = true;

            if (!Machine.MachineHomed)
            {
                MyMessageBox.Show("The machine has not been homed.");
                return;
            }

            if (Machine.Playing)
            {
                MyMessageBox.Show("Already Playing.");
                return;
            }

            if ((!Machine.SpindleAxis.Running))
            {
                MyMessageBox.Show("Spindle not running.");
                return;
            }

            if (!ContinuousPlay)
            {
                ContinuousPlay = (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl));
            }

            if (Machine.CurrentProgram.MemoriesUsed == 0)
            {
                MyMessageBox.Show("No program to play.");
                return;
            }

            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show("e1 : " + Machine.ErrorMessage);
                return;
            }

            SetSpindleDirection(false);

            ProgressValue = 0;
            ProgressMaximum = Machine.CalculateProgramTime();

            Machine.StartPosOnly = false;
            Machine.ResetPlaybackSequence();
            Machine.CheckPlayBack();
            ProgressBarStartTime = DateTime.Now;
            //EnableSettings(false);
            Thread.Sleep(250);
            InCycle = true;
        }

        private bool CanExecutePlay()
        {
            return !Machine.Playing && !Machine.Recording;
        }

        public void StopPlayBack()
        {
            Machine.DisableMachine();
            InCycle = false;
            //EnableSettings(true);
            Machine.CancelPlay();
            Machine.SpindleAxis.CommandSpeed = Machine.CurrentProgram.MemS[0].SpindleSpeed;
            double i = DateTime.Now.Subtract(ProgressBarStartTime).TotalMilliseconds;
            if (i > ProgressMaximum) i = ProgressMaximum;
            ProgressValue = i;
            CommandManager.InvalidateRequerySuggested();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteStopPlay))]
        public void StopPlay()
        {
            ContinuousPlay = false;
            StopPlayBack();
        }

        public bool CanExecuteStopPlay()
        {
            return Machine.Playing;
        }

        private void PlayBackComplete(object sender, EventArgs e)
        {
            if (!Machine.StartPosOnly)
            {
                PartsCounter = PartsCounter + 1;
            }
            ProgressValue = ProgressMaximum;

            StopPlayBack();
            if (Machine.CurrentProgram.UseCentreDevice) Machine.CentreUp();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRecord))]
        private void Record()
        {
            if (!Machine.MachineHomed)
            {
                MyMessageBox.Show("The machine has not been homed.");
                return;
            }

            if (Machine.CurrentProgram.MemS[Machine.CurrentProgram.MemoriesUsed + 1].TotalSamples > 0)
            {
                if (MyMessageBox.Show("This memory has already been recorded in.  Are you sure you want to record over this?", "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show("e2 : " + Machine.ErrorMessage);
                return;
            }

            StartRecordTime = DateTime.Now;
            Machine.StartRecord();
            InCycle = true;
            //  EnableSettings(False);
            UpdateSettingsPage();
        }

        private bool CanExecuteRecord()
        {
            return !Machine.Playing && !Machine.Recording;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteStopRecord))]
        public void StopRecord()
        {
            Machine.StopRecord();  // allow time to actually stop

            Machine.DisableMachine();

            int RecordMem = Machine.CurrentProgram.MemoriesUsed + 1;
            Machine.UploadRecording(Machine.CurrentProgram.MemS[RecordMem]);
            Machine.CurrentProgram.MemoriesUsed = Machine.CurrentProgram.MemoriesUsed + 1;

            Machine.CurrentProgram.MemS[RecordMem].SpindleSpeed = Settings.Default.JogSpeedSpindle;
            Machine.CurrentProgram.MemS[RecordMem].HeadNumber = Machine.SelectedHeadNo;
            if (RecordMem == 1) Machine.CurrentProgram.MemS[0].HeadNumber = Machine.SelectedHeadNo;
            // Make the position of the 3rd and 4th axes the same for the whole mem
            for (int ax = 2; ax < Globals.TotalAxes; ax++)
            {
                Machine.CurrentProgram.MemS[RecordMem].TargetPositions[ax] = Machine.CurPosition(ax);
            }

            Machine.CurrentProgram.MemS[RecordMem].PlaybackSpeed = 100;
            Machine.CurrentProgram.MemS[RecordMem].FormerCycle = 0;
            SamplesRecorded = SamplesToSecs(Machine.CurrentProgram.MemS[RecordMem].TotalSamples);
            NextMemoryToRecord = RecordMem;

            UpdateSettingsPage();

            InCycle = false;
            //!EnableSettings(true);

            int RecordTimeInSamples = (int)(DateTime.Now.Subtract(StartRecordTime).TotalMilliseconds / 20);
            if (Math.Abs(RecordTimeInSamples - Machine.RecordedSamples) > 100)
            {
                MyMessageBox.Show("Record Failed.");
                logger.LogError("%%%%% Record Failed %%%%%%");
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanExecuteStopRecord()
        {
            return Machine.Recording;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteStartPos))]
        public void StartPos()
        {
            if (!Machine.MachineHomed)
            {
                MyMessageBox.Show("The machine has not been homed.");
                return;
            }

            if (Machine.Playing)
            {
                MyMessageBox.Show("Already Playing.");
                return;
            }

            if (Machine.CurrentProgram.MemoriesUsed == 0) return;

            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e4 : {Machine.ErrorMessage}");
                return;
            }

            Thread.Sleep(250);
            InCycle = true;
            //  EnableSettings(False);
            ProgressValue = 0;
            ProgressMaximum = 1; // Controller.CalculateProgramTime; // don't use ProgressBar
            ProgressBarStartTime = DateTime.Now;
            Machine.StartPosOnly = true;
            Machine.ResetPlaybackSequence();
            Machine.CheckPlayBack();
        }

        public bool CanExecuteStartPos()
        {
            return !Machine.Playing && !Machine.Recording;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteHome))]
        public void Home()
        {
            if (Machine.Playing)
            {
                MyMessageBox.Show("Already Playing.");
                return;
            }

            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show("e6 : " + Machine.ErrorMessage);
                return;
            }

            logger.LogInformation("Home Machine");
            if (Dialogs.Home.ShowModal() ?? false)
            {
                Machine.MachineHomed = true;
                logger.LogInformation("Home Complete");
            }
            Machine.DisableMachine();
        }

        
        public bool CanExecuteHome()
        {
            return !Machine.Playing && !Machine.Recording;
        }

        [RelayCommand]
        public void DeleteMemory()
        {
            Machine.CurrentProgram.DeleteLastMemory();

            SamplesRecorded = 0;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void ResetError()
        {
            Machine.ResetDriveErrors();
        }

        [RelayCommand]
        public void Diagnose()
        {
            //TODO: Diagnose
            //throw new NotImplementedException();
        }

        [RelayCommand]
        public void Support()
        {
            string teamviewer = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TeamViewerQS_en.exe");
            if (File.Exists(teamviewer))
            {
                Process.Start(teamviewer);
                // this.WindowState = FormWindowState.Minimized;
            }
        }

        [RelayCommand]
        public void ResetPartsCounter()
        {
            PartsCounter = 0;
        }

        [RelayCommand]
        public void ChangePartsCounter()
        {
            double d = PartsCounter;
            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                PartsCounter = (int)d;
            }
        }


        private void UpdateSettingsPage()
        {
            SampledProfile profile = Machine.CurrentProgram.MemS[SettingsSelected];

            if (Globals.TotalAxes > 0) SettingOffset1 = (profile.Offsets[0] / Machine.Axes[0].DisplayMultiplier);
            if (Globals.TotalAxes > 1) SettingOffset2 = (profile.Offsets[1] / Machine.Axes[1].DisplayMultiplier);
            if (Globals.TotalAxes > 2) SettingOffsetZ = (profile.Offsets[2] / Machine.Axes[2].DisplayMultiplier);
            if (Globals.TotalAxes > 3) SettingOffsetX = (profile.Offsets[3] / Machine.Axes[3].DisplayMultiplier);
            if (Globals.TotalAxes > 4) SettingOffsetB = (profile.Offsets[4] / Machine.Axes[4].DisplayMultiplier);

            UpdateSampleTimes();

            SettingPlaybackSpeed = profile.PlaybackSpeed;
            SettingSpindleSpeed = profile.SpindleSpeed;

            SettingFormerCycle = Machine.CurrentProgram.MemS[SettingsSelected].FormerCycle;

            if (Machine.CurrentProgram.MemS[1].TotalSamples > 0)
            {
                if (Globals.TotalAxes > 0) StartPosition1 = (Machine.CurrentProgram.MemS[1].AxisStorage[0][0] / Machine.Axes[0].DisplayMultiplier);
                if (Globals.TotalAxes > 1) StartPosition2 = (Machine.CurrentProgram.MemS[1].AxisStorage[1][0] / Machine.Axes[1].DisplayMultiplier);
                if (Globals.TotalAxes > 2) StartPositionZ = (Machine.CurrentProgram.MemS[1].TargetPositions[2] / Machine.Axes[2].DisplayMultiplier);
                if (Globals.TotalAxes > 3) StartPositionX = (Machine.CurrentProgram.MemS[1].TargetPositions[3] / Machine.Axes[3].DisplayMultiplier);
                if (Globals.TotalAxes > 4) StartPositionB = (Machine.CurrentProgram.MemS[1].TargetPositions[4] / Machine.Axes[4].DisplayMultiplier);
            }
            else
            {
                StartPosition1 = 0;
                StartPosition2 = 0;
                StartPositionZ = 0;
                StartPositionX = 0;
                StartPositionB = 0;
            }
            NextMemoryToRecord = (Machine.CurrentProgram.MemoriesUsed + 1);
            UseCentreDevice = Machine.CurrentProgram.UseCentreDevice;
            UseBackStopDevice = Machine.CurrentProgram.UseBackStopDevice;

            ProgramNote = Machine.CurrentProgram.Note;

            //switch (profile.FormerCycle)
            //{
            //    case FormerCycleEnum.None: radioFormerCycleNone.Checked = true; break;
            //    case FormerCycleEnum.Front: radioFormerCycleFront.Checked = true; break;
            //    case FormerCycleEnum.Top: radioFormerCycleTop.Checked = true; break;
            //    case FormerCycleEnum.FrontFirst: radioFormerCycleFrontFirst.Checked = true; break;
            //    case FormerCycleEnum.TopFirst: radioFormerCycleTopFirst.Checked = true; break;
            //}

            //switch (Machine.CurrentProgram.SpindleDieSize)
            //{
            //    case 1: radioSpindleDieSize1.Checked = true; break;
            //    case 2: radioSpindleDieSize2.Checked = true; break;
            //    case 3: radioSpindleDieSize3.Checked = true; break;
            //}

            //lblFrontFormerSpeed.Text = Machine.CurrentProgram.FrontFormerSpeed.ToString("F0");
            //lblFrontFormerTime.Text = Machine.CurrentProgram.FrontFormerTime.ToString("F1");
            //lblTopFormerSpeed.Text = Machine.CurrentProgram.TopFormerSpeed.ToString("F0");
            //lblTopFormerTime.Text = Machine.CurrentProgram.TopFormerTime.ToString("F1");
            //lblFrontFormerSpindleRPM.Text = Machine.CurrentProgram.FrontFormerSpindleRPM.ToString();
            //lblTopFormerSpindleRPM.Text = Machine.CurrentProgram.TopFormerSpindleRPM.ToString();
        }

        [RelayCommand]
        public void SelectSettings(int x)
        {
            SettingsSelected = x;
            UpdateSettingsPage();
        }

        //!![RelayCommand]
        public void SelectForm(int x) => FormSelected = x;

        private void AddOffset(int axisno, double value)
        {
            double ofs = Machine.CurrentProgram.MemS[SettingsSelected].Offsets[axisno];
            double stepValue = 0;
            double maxValue = 0;
            if (Machine.Axes[axisno].SwapJogButtons)
            {
                value = -value;
            }

            if (Machine.Metric || (axisno == 4))
            {
                stepValue = value * Machine.Axes[axisno].DisplayMultiplier / 50;
                maxValue = 2 * Machine.Axes[axisno].DisplayMultiplier;
            }
            else
            {
                stepValue = value * Machine.Axes[axisno].DisplayMultiplier / 1000;
                maxValue = 0.08 * Machine.Axes[axisno].DisplayMultiplier;
            }
            ofs += stepValue;
            if (ofs > maxValue)
            {
                ofs = maxValue;
            }
            if (ofs < -maxValue)
            {
                ofs = -maxValue;
            }
            Machine.CurrentProgram.MemS[SettingsSelected].Offsets[axisno] = ofs;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void Axis1OffsetDown() => AddOffset(0, 1);
        [RelayCommand]
        public void Axis1OffsetUp() => AddOffset(0, -1);

        [RelayCommand]
        public void Axis2OffsetDown() => AddOffset(1, 1);
        [RelayCommand]
        public void Axis2OffsetUp() => AddOffset(1, -1);

        [RelayCommand]
        public void AxisZOffsetDown() => AddOffset(2, 1);
        [RelayCommand]
        public void AxisZOffsetUp() => AddOffset(2, -1);

        [RelayCommand]
        public void AxisXOffsetDown() => AddOffset(3, 1);
        [RelayCommand]
        public void AxisXOffsetUp() => AddOffset(3, -1);

        [RelayCommand]
        public void AxisBOffsetDown() => AddOffset(4, 1);
        [RelayCommand]
        public void AxisBOffsetUp() => AddOffset(4, -1);

        private void CheckSpindleJog()
        {
            if (IncreaseSpindleSpeedO || DecreaseSpindleSpeedO)
            {
                if (IncreaseSpindleSpeedO & (Machine.CurrentProgram.MemS[SettingsSelected].SpindleSpeed < Machine.SpindleAxis.MaxSpeed)) Machine.CurrentProgram.MemS[SettingsSelected].SpindleSpeed += 10;
                if (DecreaseSpindleSpeedO & (Machine.CurrentProgram.MemS[SettingsSelected].SpindleSpeed > Machine.SpindleAxis.MinSpeed)) Machine.CurrentProgram.MemS[SettingsSelected].SpindleSpeed -= 10;
                Machine.CurrentProgram.ProgramChanged = true;
                UpdateSettingsPage();
            }
        }

        [RelayCommand]
        public void SpindleOffsetDownDown()
        {
            DecreaseSpindleSpeedO = true;
            IncreaseSpindleSpeedO = false;
            CheckSpindleJog();
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        [RelayCommand]
        public void SpindleOffsetUpDown()
        {
            DecreaseSpindleSpeedO = false;
            IncreaseSpindleSpeedO = true;
            CheckSpindleJog();
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        [RelayCommand]
        public void SpindleOffsetUp()
        {
            DecreaseSpindleSpeedO = false;
            IncreaseSpindleSpeedO = false;
        }

        [RelayCommand]
        public void PlaybackSpeedUp()
        {
            if (Machine.CurrentProgram.MemS[SettingsSelected].PlaybackSpeed < 200) Machine.CurrentProgram.MemS[SettingsSelected].PlaybackSpeed += 10;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void PlaybackSpeedDown()
        {
            if (Machine.CurrentProgram.MemS[SettingsSelected].PlaybackSpeed > 30) Machine.CurrentProgram.MemS[SettingsSelected].PlaybackSpeed -= 10;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void FormerCycleUp()
        {
            if (Machine.CurrentProgram.MemS[SettingsSelected].FormerCycle < 11) Machine.CurrentProgram.MemS[SettingsSelected].FormerCycle += 1;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void FormerCycleDown()
        {
            if (Machine.CurrentProgram.MemS[SettingsSelected].FormerCycle > 0) Machine.CurrentProgram.MemS[SettingsSelected].FormerCycle -= 1;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void ChangeUnits()
        {
            Machine.Metric = !Machine.Metric;
            UpdateUnitsName();
        }

        [RelayCommand]
        public void ChangeParameter(ParameterViewModel x)
        {
            x.ChangeParameter();
            if (Parameters.Machine != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.Machine).Refresh();
                Machine.DownloadParameters();
            }
            if (Parameters.Axis1 != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.Axis1).Refresh();
                Machine.DownloadParameters();
            }
            if (Parameters.Axis2 != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.Axis2).Refresh();
                Machine.DownloadParameters();
            }
            if (Parameters.AxisX != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisX).Refresh();
                Machine.XAxis.DownloadParameters();
            }
            if (Parameters.AxisZ != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisZ).Refresh();
                Machine.ZAxis.DownloadParameters();
            }
            if (Parameters.AxisB != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisB).Refresh();
                Machine.BAxis.DownloadParameters();
            }
            if ((Parameters.AxisX1 != null) && (Machine.X1Axis != null))
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisX1).Refresh();
                Machine.X1Axis.DownloadParameters();
            }
            if ((Parameters.AxisY1 != null) && (Machine.Y1Axis != null))
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisY1).Refresh();
                Machine.Y1Axis.DownloadParameters();
            }
            if ((Parameters.AxisZ1 != null) && (Machine.Z1Axis != null))
            {
                CollectionViewSource.GetDefaultView(Parameters.AxisZ1).Refresh();
                Machine.Z1Axis.DownloadParameters();
            }
            if (Parameters.Spindle != null)
            {
                CollectionViewSource.GetDefaultView(Parameters.Spindle).Refresh();
                Machine.SpindleAxis.DownloadParameters();
            }
        }

        [RelayCommand]
        public void ManualSpindleStart()
        {
            Machine.SpindleAxis.Start();
        }

        [RelayCommand]
        public void ManualSpindleStop()
        {
            Machine.SpindleAxis.Stop();
        }

        public void SetSpindleDirection(bool Direction)
        {
            IsSpindleReversed = Direction;
            if (IsSpindleReversed)
            {
                Machine.SpindleAxis.SetSpindleDirection(true);
            }
            else
            {
                Machine.SpindleAxis.SetSpindleDirection(false);
                Machine.SpindleAxis.CommandSpeed = Settings.Default.JogSpeedSpindle;
            }
        }

        [RelayCommand]
        public void ManualSpindleReverse()
        {
            SetSpindleDirection(!IsSpindleReversed);
        }

        [RelayCommand]
        public void ManualRollerHead(int headNumber)
        {
            if (Machine.Recording)
            {
                MyMessageBox.Show("You can not perform this operation while recording.");
                return;
            }
            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e7 : {Machine.ErrorMessage}");
                return;
            }
            Machine.ChangeHead(headNumber);
            HeadSelected = headNumber;
        }

        [RelayCommand]
        public void ManualCentreUp()
        {
            Machine.CentreUp();
        }

        [RelayCommand]
        public void SelectUseCentre(bool x)
        {
            Machine.CurrentProgram.UseCentreDevice = x;
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void SelectUseBackStop(bool x)
        {
            Machine.CurrentProgram.UseBackStopDevice = x;
            Machine.UseBackStop(x);
            Machine.CurrentProgram.ProgramChanged = true;
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void ManualBackStopCycle()
        {
            if (Machine.Recording)
            {
                MyMessageBox.Show("You can not perform this operation while recording.");
                return;
            }
            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e1 : {Machine.ErrorMessage}");
                return;
            }

            Machine.BackStopCycle();
        }

        [RelayCommand]
        public void ManualBackStopIn()
        {
            if (Machine.Recording)
            {
                MyMessageBox.Show("You can not perform this operation while recording.");
                return;
            }
            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e1 : {Machine.ErrorMessage}");
                return;
            }

            Machine.BackStopIn();
        }

        [RelayCommand]
        public void ManualFrontFormerCycle()
        {
            if (Machine.Recording)
            {
                MyMessageBox.Show("You can not perform this operation while recording.");
                return;
            }
            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e1 : {Machine.ErrorMessage}");
                return;
            }

            Machine.FrontFormerCycle();
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void ManualFrontFormerTeach()
        {
            //FormerTeach dlg = new FormerTeach(Machine.TopFormer.HAxis, Machine.TopFormer.VAxis, this);
            //dlg.ShowDialog();
        }

        [RelayCommand]
        public void ManualTopFormerCycle()
        {
            if (Machine.Recording)
            {
                MyMessageBox.Show("You can not perform this operation while recording.");
                return;
            }
            DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show($"e1 : {Machine.ErrorMessage}");
                return;
            }

            Machine.TopFormerCycle();
            UpdateSettingsPage();
        }

        [RelayCommand]
        public void ManualTopFormerTeach()
        {
            //FormerTeach dlg = new FormerTeach(Machine.TopFormer.HAxis, Machine.TopFormer.VAxis, this);
            //dlg.ShowDialog();
        }

        [RelayCommand]
        public void Params()
        {
            if (ParametersVisible)
            {
                ParametersVisible = false;
            }
            else
            {
                double d = 0;
                if (Calculator.ShowCalculator(ref d, "Enter Password"))
                {
                    if (d == 3723)
                    {
                        ParametersVisible = true;
                    }
                }
            }
        }

        [RelayCommand]
        public void TuneAxis1()
        {
            Machine.TuneAxis(0);
        }

        [RelayCommand]
        public void TuneAxis2()
        {
            Machine.TuneAxis(1);
        }

        [RelayCommand]
        public void SaveVFFTable1()
        {
            Machine.UploadNewVFFTable(0);
            Machine.DownloadParameters();
        }

        [RelayCommand]
        public void SaveVFFTable2()
        {
            Machine.UploadNewVFFTable(1);
            Machine.DownloadParameters();
        }

        [RelayCommand]
        public void ReloadVFFTables()
        {
            Machine.ReadVFFTables();
        }

        [RelayCommand]
        public void EditNote()
        {
            if (EditDialog.Show("Enter Program Note", "Program Note", Machine.CurrentProgram.Note, out string NewValue) ?? false)
            {
                Machine.CurrentProgram.Note = NewValue;
                Machine.CurrentProgram.ProgramChanged = true;
                UpdateSettingsPage();
            }
        }
    }
}