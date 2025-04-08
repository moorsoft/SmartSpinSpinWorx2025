using SmartSpin.Converters;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using SmartSpin.Model;
using SmartSpin.Properties;
using SmartSpin.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SmartSpin.UserControls
{
    /// <summary>
    /// Interaction logic for FormerTeach.xaml
    /// </summary>
    public partial class FormerTeach : UserControl
    {
        private readonly ControllerAxis HAxis;
        private readonly ControllerAxis VAxis;
        private MainViewModel _viewModel;
        private readonly FormerTeachViewModel _formerViewModel;

        private int GetJogSpeed(int axisno)
        {
            switch (axisno)
            {
                case 0: return Settings.Default.JogSpeed0;
                case 1: return Settings.Default.JogSpeed1;
                case 2: return Settings.Default.JogSpeed2; 
                case 3: return Settings.Default.JogSpeed3;
                case 4: return Settings.Default.JogSpeed4;
                case 5: return Settings.Default.JogSpeed5;
                case 6: return Settings.Default.JogSpeed6; 
                case 7: return Settings.Default.JogSpeed7; 
                case 8: return Settings.Default.JogSpeed8; 
                case 9: return Settings.Default.JogSpeed9;
                case 10: return Settings.Default.JogSpeed10;
            }
            return 0;
        }

        private void SetJogSpeed(int axisno, int speed)
        {
            switch (axisno)
            {
                case 0: Settings.Default.JogSpeed0 = speed; break;
                case 1: Settings.Default.JogSpeed1 = speed; break;
                case 2: Settings.Default.JogSpeed2 = speed; break;
                case 3: Settings.Default.JogSpeed3 = speed; break;
                case 4: Settings.Default.JogSpeed4 = speed; break;
                case 5: Settings.Default.JogSpeed5 = speed; break;
                case 6: Settings.Default.JogSpeed6 = speed; break;
                case 7: Settings.Default.JogSpeed7 = speed; break;
                case 8: Settings.Default.JogSpeed8 = speed; break;
                case 9: Settings.Default.JogSpeed9 = speed; break;
                case 10: Settings.Default.JogSpeed10 = speed; break;
            }
        }

        public FormerTeach()
        {
            InitializeComponent();

            HAxis = Machine.TopFormer.HAxis;
            VAxis = Machine.TopFormer.VAxis;
            _formerViewModel = new FormerTeachViewModel(HAxis, VAxis, this);

            Loaded += (sender, args) =>
            {
                if (_viewModel == null)
                {
                    _viewModel = this.DataContext as MainViewModel;
                }
                DataContext = _formerViewModel;
                title1.DataContext = _viewModel;
                title2.DataContext = _viewModel;
                spindle.DataContext = _viewModel;

                title1.Header = $"Axis {HAxis.Letter}";
                title2.Header = $"Axis {VAxis.Letter}";
                Binding b;
                b = new Binding($"Position{HAxis.axisno}")
                {
                    Converter = new UnitsFormatConverter()
                };
                txtPosition1.SetBinding(TextBlock.TextProperty, b);
                b = new Binding($"PositionLag{HAxis.axisno}");
                txtLag1.SetBinding(TextBlock.TextProperty, b);

                b = new Binding($"Position{VAxis.axisno}")
                {
                    Converter = new UnitsFormatConverter()
                };
                txtPosition2.SetBinding(TextBlock.TextProperty, b);
                b = new Binding($"PositionLag{VAxis.axisno}");
                txtLag2.SetBinding(TextBlock.TextProperty, b);

                btnSpeed1.Content = $"Speed {GetJogSpeed(HAxis.axisno)}";
                btnSpeed2.Content = $"Speed {GetJogSpeed(VAxis.axisno)}";

                ChangeSelectedForm();
            };
        }


        internal void ChangeSelectedForm()
        {
            double speed = Machine.CurrentProgram.FormerCycles[_formerViewModel.FormSelected].SpindleSpeed;
            txtSpindleSpeed.Text = speed.ToString();
            dataGrid.ItemsSource = Machine.CurrentProgram.FormerCycles[_formerViewModel.FormSelected].FormerMoves;
        }

        internal void UpdateDataGrid()
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Machine.CurrentProgram.FormerCycles[_formerViewModel.FormSelected].FormerMoves;
        }

        private void BtnJog_PreviewMouseUp(int axis)
        {
            if (Machine.Recording) return;

            Machine.Jog(axis, 0);
            _viewModel.DisableMachineDelay = DateTime.Now.AddSeconds(10);
        }

        private void BtnJog_PreviewMouseDown(int axis, int dir)
        {
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
            Machine.Jog(axis, dir * GetJogSpeed(axis));
        }

        private void BtnSpeed_Click(int axis, Button btn)
        {
            {
                double d = 0;

                d = GetJogSpeed(axis);
                if (Calculator.ShowCalculator(ref d, "Enter New Jog Speed"))
                {
                    int speed = (int)d;
                    if (speed > 100) speed = 100;
                    if (speed < 1) speed = 1;
                    SetJogSpeed(axis, speed);
                    btn.Content = $"Speed {speed}";
                }
            }
        }

        private void BtnJog1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseUp(HAxis.axisno);
        }

        private void btnJog1_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseUp(HAxis.axisno);
        }

        private void BtnJogPlus1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseDown(HAxis.axisno, 1);
        }

        private void BtnJogMinus1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseDown(HAxis.axisno, -1);
        }

        private void BtnSpeed1_Click(object sender, RoutedEventArgs e)
        {
            BtnSpeed_Click(HAxis.axisno, btnSpeed1);
        }

        private void BtnJog2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseUp(VAxis.axisno);
        }

        private void btnJog2_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseUp(VAxis.axisno);
        }

        private void BtnJogPlus2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseDown(VAxis.axisno, 1);
        }

        private void BtnJogMinus2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            BtnJog_PreviewMouseDown(VAxis.axisno, -1);
        }

        private void BtnSpeed2_Click(object sender, RoutedEventArgs e)
        {
            BtnSpeed_Click(VAxis.axisno, btnSpeed2);
        }

        private void BtnSpeedS_Click(object sender, RoutedEventArgs e)
        {
            double d = 0;

            d = Machine.CurrentProgram.FormerCycles[_formerViewModel.FormSelected].SpindleSpeed;
            if (Calculator.ShowCalculator(ref d, "Enter New Spindle Speed"))
            {
                int speed = (int)d;
                if (speed > 2000) speed = 2000;
                if (speed < 1) speed = 1;
                Machine.CurrentProgram.FormerCycles[_formerViewModel.FormSelected].SpindleSpeed = speed;
                txtSpindleSpeed.Text = speed.ToString();
            }
        }

    }
}
