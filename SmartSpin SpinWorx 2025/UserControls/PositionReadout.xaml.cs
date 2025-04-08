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
    /// Interaction logic for PositionReadout.xaml
    /// </summary>
    public partial class PositionReadout : UserControl
    {
        public PositionReadout()
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
                        b = new Binding("StartPosition1")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtStartPosition.SetBinding(TextBlock.TextProperty, b);
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
                        b = new Binding("StartPosition2")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtStartPosition.SetBinding(TextBlock.TextProperty, b);
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
                        b = new Binding("StartPositionZ")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtStartPosition.SetBinding(TextBlock.TextProperty, b);
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
                        b = new Binding("StartPositionX")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtStartPosition.SetBinding(TextBlock.TextProperty, b);
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
                        b = new Binding("StartPositionB")
                        {
                            Converter = new UnitsFormatConverter()
                        };
                        txtStartPosition.SetBinding(TextBlock.TextProperty, b);
                        break;
                }
            };
        }

        private MainViewModel _viewModel
        {
            get { return this.DataContext as MainViewModel; }
        }

        public int Axis
        {
            get { return (int)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(int), typeof(PositionReadout));
    }
}
