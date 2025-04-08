using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;

namespace SmartSpin.Dialogs
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : Window
    {
        const int MaxDigits = 14;
        const char nullchar = (char)0;

        public double FinalResult = 0;

        bool HasDecimal;
        string ExponentialString = String.Empty;
        string NumberString = String.Empty;
        double Memory = 0;
        double YMemory;
        char LastOperator;
        char StoredOperator;
        double StoredValue;
        bool ReadyToClear = false;
        bool btnInvDown = false;

        public Calculator()
        {
            InitializeComponent();
            Owner = App.Current.MainWindow;
            lblExponential.Content = "";
        }

        void DisplayString()
        {
            if (NumberString.Contains("."))
                lblDisplay.Content = NumberString;
            else
            {
                if (NumberString == String.Empty)
                    lblDisplay.Content = "0.";
                else
                    lblDisplay.Content = NumberString + ".";
            }
            lblExponential.Content = ExponentialString;
        }

        void ErrorMess()
        {
            ClearCurrentValue();
            DisplayString();
            lblDisplay.Content = "ERROR";
            LastOperator = nullchar;
            StoredOperator = nullchar;
            ReadyToClear = true;
        }

        double CurrentValue()
        {
            if (NumberString == String.Empty) return 0;

            double value;
            if (ExponentialString == String.Empty)
                double.TryParse(NumberString, out value);
            else
                double.TryParse(NumberString + 'e' + ExponentialString, out value);

            return value;
        }

        private void DoDigit(char Digit)
        {
            if (ReadyToClear) ClearCurrentValue();
            ReadyToClear = false;
            if (NumberString.Length >= MaxDigits) return;
            if ((Digit == '.') && (HasDecimal)) return;
            if ((NumberString == String.Empty) && (Digit == '.')) NumberString = "0";
            if ((NumberString == "0") && (Digit == '0'))
            {
                DisplayString();
                return;
            }
            if (Digit == '.') HasDecimal = true;
            if ((NumberString == "0") && (char.IsDigit(Digit)))
                NumberString = Digit.ToString();
            else
                NumberString += Digit;
            DisplayString();
        }

        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            DoDigit(Convert.ToChar((sender as Button).Content));
        }

        private Boolean DoOnce = true;

        private void Window_Activated(object sender, EventArgs e)
        {
            StoredOperator = nullchar;
            LastOperator = nullchar;
            ReadyToClear = false;
            ConvertString(FinalResult);
            DisplayString();
            ReadyToClear = true;

            if ((DoOnce) && (flowDocumentScrollViewer1.Document != null))
            {
                Height += flowDocumentScrollViewer1.ActualHeight;
                DoOnce = false;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            StoredOperator = nullchar;
            LastOperator = nullchar;
            ClearCurrentValue();
            DisplayString();
        }

        double Operation(double V1, double V2, char Op)
        {
            switch (Op)
            {
                case '+': return V1 + V2;
                case '-': return V1 - V2;
                case '*': return V1 * V2;
                case '/': return V1 / V2;
                case 'P': return Math.Pow(V1, V2);
                default: return V1 + V2;
            }
        }

        void ConvertString(double Value)
        {
            string S, E;

            S = Value.ToString("G" + MaxDigits.ToString());
            int eindex = S.IndexOf('e');
            if (eindex != -1)
            {
                E = S.Substring(eindex + 1);
                ExponentialString = E;
                S = S.Substring(1, eindex - 1);
            }
            HasDecimal = S.Contains(".");
            NumberString = S;
        }

        void ClearCurrentValue()
        {
            NumberString = String.Empty;
            ExponentialString = String.Empty;
            HasDecimal = false;
        }

        void DoOperator(char Operator)
        {
            double CurrentV;

            ReadyToClear = false;
            if (NumberString == String.Empty) return;
            try
            {
                CurrentV = CurrentValue();
                if (((LastOperator == '+') || (LastOperator == '-')) && ((Operator == '*') || (Operator == '/') || (Operator == 'P')))
                {
                    StoredOperator = LastOperator;
                    StoredValue = YMemory;
                    YMemory = CurrentV;
                    LastOperator = Operator;
                    ClearCurrentValue();
                    return;
                }
                switch (LastOperator)
                {
                    case '+':
                    case '-':
                        CurrentV = Operation(YMemory, CurrentV, LastOperator);
                        if (StoredOperator != nullchar)
                        {
                            CurrentV = Operation(StoredValue, CurrentV, StoredOperator);
                            StoredValue = 0;
                            StoredOperator = nullchar;
                        }
                        ConvertString(CurrentV);
                        DisplayString();
                        break;
                    case '*':
                    case '/':
                    case 'P':
                        CurrentV = Operation(YMemory, CurrentV, LastOperator);
                        ConvertString(CurrentV);
                        DisplayString();
                        break;
                }

                if (Operator == '=')
                {
                    if (StoredOperator != (char)0)
                    {
                        CurrentV = Operation(StoredValue, CurrentV, StoredOperator);
                        StoredValue = 0;
                        StoredOperator = (char)0;
                    }
                    ConvertString(CurrentV);
                    DisplayString();

                    YMemory = 0;
                    LastOperator = (char)0;
                    ReadyToClear = true;
                }
                else
                {
                    YMemory = CurrentValue();
                    LastOperator = Operator;
                    ClearCurrentValue();
                }
            }
            catch
            {
                ErrorMess();
            }
        }

        private void btnOperator_Click(object sender, RoutedEventArgs e)
        {
            DoOperator(Convert.ToChar((sender as Button).Tag));
        }

        private void btnSpecialOp_Click(object sender, RoutedEventArgs e)
        {
            double CurrentV;

            try
            {
                CurrentV = CurrentValue();
                if (sender == btnNeg) CurrentV = -CurrentV;
                if (sender == btnSin)
                {
                    if (btnInvDown) CurrentV = Math.Asin(CurrentV);
                    else CurrentV = Math.Sin(CurrentV);
                }
                if (sender == btnCos)
                {
                    if (btnInvDown) CurrentV = Math.Acos(CurrentV);
                    else CurrentV = Math.Cos(CurrentV);
                }
                if (sender == btnTan)
                {
                    if (btnInvDown) CurrentV = Math.Atan(CurrentV);
                    else CurrentV = Math.Tan(CurrentV);
                }
                if (sender == btnPI) CurrentV = Math.PI;
                if (sender == btn1over) CurrentV = 1 / CurrentV;
                if (sender == btnSqr) CurrentV = CurrentV * CurrentV;
                if (sender == btnSqrt) CurrentV = Math.Sqrt(CurrentV);

                ConvertString(CurrentV);
                DisplayString();
                ReadyToClear = true;
            }
            catch
            {
                ErrorMess();
            }
        }

        private void btnInv_Click(object sender, RoutedEventArgs e)
        {
            btnInvDown = (btnInv.IsChecked ?? false);
            //            btnInvDown = !btnInvDown;
            //            if (btnInvDown)
            //                btnInv.BackColor = SystemColors.ControlDark;
            //            else
            //                btnInv.UseVisualStyleBackColor = true;
        }

        private void btnMemory_Click(object sender, RoutedEventArgs e)
        {
            double CurrentV;

            if (sender == btnMR)
            {
                CurrentV = Memory;
                ConvertString(CurrentV);
                DisplayString();
            }
            if (sender == btnMplus) Memory = Memory + CurrentValue();
            if (sender == btnMminus) Memory = Memory - CurrentValue();
            if (sender == btnMminus) Memory = 0;
            lblInMem.Visibility = ((Memory != 0) ? Visibility.Visible : Visibility.Hidden);
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            DoOperator('=');
            FinalResult = CurrentValue();
            this.DialogResult = true;
        }

        private void btnESC_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnBackSpace_Click(object sender, RoutedEventArgs e)
        {
            char Digit;

            ReadyToClear = false;
            if (NumberString.Length == 0) return;
            Digit = NumberString[NumberString.Length - 1];
            NumberString = NumberString.Substring(0, NumberString.Length - 1);
            if (Digit == '.') HasDecimal = false;
            DisplayString();
        }

        public void ChangeValue(double Value)
        {
            ConvertString(Value);
            DisplayString();
            ReadyToClear = true;
        }

        FlowDocument GetHelpDocument(string helpname)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            /*
            string[] resourceList = ass.GetManifestResourceNames();
            foreach (var s in resourceList)
            {

            }
             */
            Stream flowDocumentStream = ass.GetManifestResourceStream(String.Format("PipeBender.HelpFiles.{0}.xaml", helpname));
            return (flowDocumentStream == null ? null : System.Windows.Markup.XamlReader.Load(flowDocumentStream) as FlowDocument);
        }

        public static bool ShowCalculator(ref double FinalResult, string Caption, string HintMessage, string HelpName)
        {
            Calculator calc = new Calculator();
            calc.FinalResult = FinalResult;
            calc.Title = Caption;
            if (HintMessage != String.Empty)
            {
                calc.lblHintMessage.Text = HintMessage;
            }
            if (HelpName != String.Empty)
            {
                calc.flowDocumentScrollViewer1.Document = calc.GetHelpDocument(HelpName);
            }
            calc.flowDocumentScrollViewer1.Visibility = (calc.flowDocumentScrollViewer1.Document == null ? Visibility.Collapsed : Visibility.Visible);

            bool? dr = calc.ShowDialog();
            if (dr ?? false)
            {
                FinalResult = calc.FinalResult;
                return true;
            }
            return false;
        }

        public static bool ShowCalculator(ref double FinalResult, string Caption, string HintMessage)
        {
            return ShowCalculator(ref FinalResult, Caption, HintMessage, String.Empty);
        }

        public static bool ShowCalculator(ref double FinalResult, string Caption)
        {
            return ShowCalculator(ref FinalResult, Caption, String.Empty, String.Empty);
        }

        public static bool ShowCalculator(ref double FinalResult)
        {
            return ShowCalculator(ref FinalResult, "Calculator", String.Empty, String.Empty);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            // char key = Convert.ToChar(e.Key);
            switch (e.Key)
            {
                case Key.OemPeriod:
                case Key.Decimal: DoDigit('.'); e.Handled = true; break;
                case Key.NumPad0:
                case Key.D0: DoDigit('0'); e.Handled = true; break;
                case Key.NumPad1:
                case Key.D1: DoDigit('1'); e.Handled = true; break;
                case Key.NumPad2:
                case Key.D2: DoDigit('2'); e.Handled = true; break;
                case Key.NumPad3:
                case Key.D3: DoDigit('3'); e.Handled = true; break;
                case Key.NumPad4:
                case Key.D4: DoDigit('4'); e.Handled = true; break;
                case Key.NumPad5:
                case Key.D5: DoDigit('5'); e.Handled = true; break;
                case Key.NumPad6:
                case Key.D6: DoDigit('6'); e.Handled = true; break;
                case Key.NumPad7:
                case Key.D7: DoDigit('7'); e.Handled = true; break;
                case Key.NumPad8:
                case Key.D8: DoDigit('8'); e.Handled = true; break;
                case Key.NumPad9:
                case Key.D9: DoDigit('9'); e.Handled = true; break;
                case Key.Add: DoOperator('+'); e.Handled = true; break;
                case Key.Subtract: DoOperator('-'); e.Handled = true; break;
                case Key.Multiply: DoOperator('*'); e.Handled = true; break;
                case Key.Divide: DoOperator('/'); e.Handled = true; break;
                //case Key.Equals: DoOperator('='); e.Handled = true; break;
                case Key.Enter: btnEnter_Click(sender, RoutedEventArgs.Empty as RoutedEventArgs); e.Handled = true; break;
                case Key.Back: btnBackSpace_Click(sender, RoutedEventArgs.Empty as RoutedEventArgs); e.Handled = true; break;
                default: break;
            }
        }
    }
}
