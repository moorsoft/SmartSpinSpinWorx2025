using SmartSpin.Converters;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;
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
    /// Interaction logic for TeachPositionReadout.xaml
    /// </summary>
    public partial class TeachPositionReadout : UserControl
    {
        public TeachPositionReadout()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                Binding b;
                switch (this.Axis)
                {
                    case 0:
                        title.Header = "Axis 1";
                        b = new Binding("Position0")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtPosition.SetBinding(TextBlock.TextProperty, b);
                        b = new Binding("PositionLag0")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtLag.SetBinding(TextBlock.TextProperty, b);
                        btnSpeed.Content = $"Speed {Settings.Default.JogSpeed0}";
                        if (_viewModel != null)
                        {
                            btnJogPlus.Content = _viewModel.ButtonLabelPlus1;
                            btnJogMinus.Content = _viewModel.ButtonLabelMinus1;
                        }
                        break;
                    case 1:
                        title.Header = "Axis 2";
                        b = new Binding("Position1")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtPosition.SetBinding(TextBlock.TextProperty, b);
                        b = new Binding("PositionLag1")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtLag.SetBinding(TextBlock.TextProperty, b);
                        btnSpeed.Content = $"Speed {Settings.Default.JogSpeed1}";
                        if (_viewModel != null)
                        {
                            btnJogPlus.Content = _viewModel.ButtonLabelPlus2;
                            btnJogMinus.Content = _viewModel.ButtonLabelMinus2;
                        }
                        break;
                    case 2:
                        title.Header = "Axis Z";
                        b = new Binding("PositionZ")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtPosition.SetBinding(TextBlock.TextProperty, b);
                        b = new Binding("PositionLagZ")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtLag.SetBinding(TextBlock.TextProperty, b);
                        btnSpeed.Content = $"Speed {Settings.Default.JogSpeed2}";
                        if (_viewModel != null)
                        {
                            btnJogPlus.Content = _viewModel.ButtonLabelPlusZ;
                            btnJogMinus.Content = _viewModel.ButtonLabelMinusZ;
                        }
                        break;
                    case 3:
                        title.Header = "Axis X";
                        b = new Binding("PositionX")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtPosition.SetBinding(TextBlock.TextProperty, b);
                        b = new Binding("PositionLagX")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtLag.SetBinding(TextBlock.TextProperty, b);
                        btnSpeed.Content = $"Speed {Settings.Default.JogSpeed3}";
                        if (_viewModel != null)
                        {
                            btnJogPlus.Content = _viewModel.ButtonLabelPlusX;
                            btnJogMinus.Content = _viewModel.ButtonLabelMinusX;
                        }
                        break;
                    case 4:
                        title.Header = "Axis B";
                        b = new Binding("PositionB")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtPosition.SetBinding(TextBlock.TextProperty, b);
                        b = new Binding("PositionLagB")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtLag.SetBinding(TextBlock.TextProperty, b);
                        btnSpeed.Content = $"Speed {Settings.Default.JogSpeed4}";
                        if (_viewModel != null)
                        {
                            btnJogPlus.Content = _viewModel.ButtonLabelPlusB;
                            btnJogMinus.Content = _viewModel.ButtonLabelMinusB;
                        }
                        break;
                }
            };
        }

        //internal void Update(PositionChangedMessage message)
        //{
        //    txtPosition.Text = message.Position;
        //    txtLag.Text = message.Lag;
        //}

        private MainViewModel _viewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        public int Axis
        {
            get { return (int)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(int), typeof(TeachPositionReadout));

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            double d = 0;

            switch (Axis)
            {
                case 0: d = Settings.Default.JogSpeed0; break;
                case 1: d = Settings.Default.JogSpeed1; break;
                case 2: d = Settings.Default.JogSpeed2; break;
                case 3: d = Settings.Default.JogSpeed3; break;
                case 4: d = Settings.Default.JogSpeed4; break;
            }
            if (Calculator.ShowCalculator(ref d, "Enter New Jog Speed"))
            {
                int speed = (int)d;
                if (speed > 100) speed = 100;
                if (speed < 1) speed = 1;
                switch (Axis)
                {
                    case 0: Settings.Default.JogSpeed0 = speed; break;
                    case 1: Settings.Default.JogSpeed1 = speed; break;
                    case 2: Settings.Default.JogSpeed2 = speed; break;
                    case 3: Settings.Default.JogSpeed3 = speed; break;
                    case 4: Settings.Default.JogSpeed4 = speed; break;
                }
                btnSpeed.Content = $"Speed {speed}";
            }
        }

        private void btnJogPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

            switch (Axis)
            {
                case 0:
                    Machine.Jog(0, Settings.Default.JogSpeed0);
                    break;
                case 1:
                    Machine.Jog(1, Settings.Default.JogSpeed1);
                    break;
                case 2:
                    Machine.Jog(2, Settings.Default.JogSpeed2);
                    break;
                case 3:
                    Machine.Jog(3, Settings.Default.JogSpeed3);
                    break;
                case 4:
                    Machine.Jog(4, Settings.Default.JogSpeed4);
                    break;
            }
        }

        private void btnJogMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

            switch (Axis)
            {
                case 0:
                    Machine.Jog(0, -Settings.Default.JogSpeed0);
                    break;
                case 1:
                    Machine.Jog(1, -Settings.Default.JogSpeed1);
                    break;
                case 2:
                    Machine.Jog(2, -Settings.Default.JogSpeed2);
                    break;
                case 3:
                    Machine.Jog(3, -Settings.Default.JogSpeed3);
                    break;
                case 4:
                    Machine.Jog(4, -Settings.Default.JogSpeed4);
                    break;
            }
        }

        private void btnJog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            if (Machine.Recording) return;

            switch (Axis)
            {
                case 0:
                    Machine.Jog(0, 0);
                    break;
                case 1:
                    Machine.Jog(1, 0);
                    break;
                case 2:
                    Machine.Jog(2, 0);
                    break;
                case 3:
                    Machine.Jog(3, 0);
                    break;
                case 4:
                    Machine.Jog(4, 0);
                    break;
            }
            _viewModel.DisableMachineDelay = DateTime.Now.AddSeconds(10);
        }

        private void btnJog_MouseLeave(object sender, MouseEventArgs e)
        {
            btnJog_PreviewMouseUp(sender, null);
        }
    }

 }
