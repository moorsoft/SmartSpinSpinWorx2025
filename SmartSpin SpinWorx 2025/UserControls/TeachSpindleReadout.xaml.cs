using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using SmartSpin.Properties;
using SmartSpin.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmartSpin.UserControls
{
    /// <summary>
    /// Interaction logic for TeachSpindleReadout.xaml
    /// </summary>
    public partial class TeachSpindleReadout : UserControl
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool DecreaseSpindleSpeed = false;
        private bool IncreaseSpindleSpeed = false;

        public TeachSpindleReadout()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                btnSpeed.Content = $"Speed {Settings.Default.JogSpeedSpindle}";
            };

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private MainViewModel _viewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            double d = Settings.Default.JogSpeedSpindle;

            if (Calculator.ShowCalculator(ref d, "Enter New Spindle Speed"))
            {
                if (d > Machine.SpindleAxis.MaxSpeed) d = Machine.SpindleAxis.MaxSpeed;
                if (d < Machine.SpindleAxis.MinSpeed) d = Machine.SpindleAxis.MinSpeed;
                int speed = (int)d;
                Settings.Default.JogSpeedSpindle = speed;
                btnSpeed.Content = $"Speed {speed}";
                Machine.SpindleAxis.CommandSpeed = Settings.Default.JogSpeedSpindle;
            }
        }

        private void btnJogFaster_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Machine.Recording) return;

            _viewModel.DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show("e3 : " + Machine.ErrorMessage);
                return;
            }

            _viewModel.SetSpindleDirection(false);
            IncreaseSpindleSpeed = true;
            CheckSpindleJog();
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        private void btnJogSlower_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Machine.Recording) return;

            _viewModel.DisableMachineDelay = DateTime.MaxValue;
            Machine.EnableMachine();
            Machine.UpdateStatus();
            Machine.CheckForErrors(true);
            if (!Machine.MachineOK)
            {
                MyMessageBox.Show("e3 : " + Machine.ErrorMessage);
                return;
            }

            _viewModel.SetSpindleDirection(false);
            DecreaseSpindleSpeed = true;
            CheckSpindleJog();
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        private void btnJog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            IncreaseSpindleSpeed = false;
            DecreaseSpindleSpeed = false;
            dispatcherTimer.Stop();

            if (Machine.Recording) return;
            _viewModel.DisableMachineDelay = DateTime.Now.AddSeconds(10);
        }

        private void CheckSpindleJog()
        {
            if (IncreaseSpindleSpeed | DecreaseSpindleSpeed)
            {
                if (IncreaseSpindleSpeed)
                    Settings.Default.JogSpeedSpindle += 10;
                else
                    Settings.Default.JogSpeedSpindle -= 10;

                if (Settings.Default.JogSpeedSpindle > Machine.SpindleAxis.MaxSpeed) Settings.Default.JogSpeedSpindle = (int)Machine.SpindleAxis.MaxSpeed;
                if (Settings.Default.JogSpeedSpindle < Machine.SpindleAxis.MinSpeed) Settings.Default.JogSpeedSpindle = (int)Machine.SpindleAxis.MinSpeed;
                Machine.SpindleAxis.CommandSpeed = Settings.Default.JogSpeedSpindle;
                btnSpeed.Content = $"Speed {Settings.Default.JogSpeedSpindle}";
            }
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            try
            {
                CheckSpindleJog();
            }
            finally
            {
                dispatcherTimer.Start();
            }
        }

    }

}
