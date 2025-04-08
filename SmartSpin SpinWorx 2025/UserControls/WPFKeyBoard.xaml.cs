using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartSpin.UserControls
{
    /// <summary>
    /// Interaction logic for KeyBoard.xaml
    /// </summary>
    public partial class WPFKeyBoard : UserControl
    {
        private Control editText = null;

        private Window parent = null;

        private bool capsLockOn = false;
        private bool shiftOn = false;
        private bool ctrlOn = false;
        //// private bool altOn = false;

        public WPFKeyBoard()
        {
            InitializeComponent();
            this.UpDateKeyBoard();
        }

        public void HideESC()
        {
            btnESC.Visibility = Visibility.Hidden;
        }

        public void HideEnter()
        {
            btnEnter.Visibility = Visibility.Hidden;
        }

        public event EventHandler EnterClick;

        public event EventHandler ESCClick;

        public Control EditText
        {
            get
            {
                return this.editText;
            }

            set
            {
                this.editText = value;
            }
        }

        public void OnScreen()
        {
            this.UpDateKeyBoard();
            // this.Canvas.Top = 330;
            // this.Canvas.Left = 0;
            // this.Location = new Point(0, 330);
            // this.Show();
            this.Visibility = Visibility.Visible;
        }


        private void UpDateKeyBoard()
        {
            this.capsLockOn = (this.btnCapsLock.IsChecked ?? false);
            this.shiftOn = (this.btnShift.IsChecked ?? false);
            this.ctrlOn = (this.btnCtrl.IsChecked ?? false);
            //// btnAlt.BackColor = UpDownColor(AltOn);

            if (this.capsLockOn ^ this.shiftOn)
            {
                this.btnA.Content = "A";
                this.btnB.Content = "B";
                this.btnC.Content = "C";
                this.btnD.Content = "D";
                this.btnE.Content = "E";
                this.btnF.Content = "F";
                this.btnG.Content = "G";
                this.btnH.Content = "H";
                this.btnI.Content = "I";
                this.btnJ.Content = "J";
                this.btnK.Content = "K";
                this.btnL.Content = "L";
                this.btnM.Content = "M";
                this.btnN.Content = "N";
                this.btnO.Content = "O";
                this.btnP.Content = "P";
                this.btnQ.Content = "Q";
                this.btnR.Content = "R";
                this.btnS.Content = "S";
                this.btnT.Content = "T";
                this.btnU.Content = "U";
                this.btnV.Content = "V";
                this.btnW.Content = "W";
                this.btnX.Content = "X";
                this.btnY.Content = "Y";
                this.btnZ.Content = "Z";
            }
            else
            {
                this.btnA.Content = "a";
                this.btnB.Content = "b";
                this.btnC.Content = "c";
                this.btnD.Content = "d";
                this.btnE.Content = "e";
                this.btnF.Content = "f";
                this.btnG.Content = "g";
                this.btnH.Content = "h";
                this.btnI.Content = "i";
                this.btnJ.Content = "j";
                this.btnK.Content = "k";
                this.btnL.Content = "l";
                this.btnM.Content = "m";
                this.btnN.Content = "n";
                this.btnO.Content = "o";
                this.btnP.Content = "p";
                this.btnQ.Content = "q";
                this.btnR.Content = "r";
                this.btnS.Content = "s";
                this.btnT.Content = "t";
                this.btnU.Content = "u";
                this.btnV.Content = "v";
                this.btnW.Content = "w";
                this.btnX.Content = "x";
                this.btnY.Content = "y";
                this.btnZ.Content = "z";
            }

            if (this.shiftOn)
            {
                this.btnSingleQuote.Content = "~";
                this.btn1.Content = "!";
                this.btn2.Content = "@";
                this.btn3.Content = "#";
                this.btn4.Content = "$";
                this.btn5.Content = "%";
                this.btn6.Content = "^";
                this.btn7.Content = "&";
                this.btn8.Content = "*";
                this.btn9.Content = "(";
                this.btn0.Content = ")";
                this.btnDash.Content = "_";
                this.btnEquals.Content = "+";

                this.btnOpenSquare.Content = "{";
                this.btnCloseSquare.Content = "}";
                this.btnForwardSlash.Content = "|";
                this.btnSemiColon.Content = ":";
                this.btnApostrophe.Content = "\"";
                this.btnComma.Content = "<";
                this.btnPeriod.Content = ">";
                this.btnBackSlash.Content = "?";
            }
            else
            {
                this.btnSingleQuote.Content = "`";
                this.btn1.Content = "1";
                this.btn2.Content = "2";
                this.btn3.Content = "3";
                this.btn4.Content = "4";
                this.btn5.Content = "5";
                this.btn6.Content = "6";
                this.btn7.Content = "7";
                this.btn8.Content = "8";
                this.btn9.Content = "9";
                this.btn0.Content = "0";
                this.btnDash.Content = "-";
                this.btnEquals.Content = "=";

                this.btnOpenSquare.Content = "[";
                this.btnCloseSquare.Content = "]";
                this.btnForwardSlash.Content = @"\";
                this.btnSemiColon.Content = ";";
                this.btnApostrophe.Content = "'";
                this.btnComma.Content = ",";
                this.btnPeriod.Content = ".";
                this.btnBackSlash.Content = "/";
            }
        }

        private void PressTextKey(string key)
        {
            var eventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, Keyboard.FocusedElement, key));
            eventArgs.RoutedEvent = TextInputEvent;
            InputManager.Current.ProcessInput(eventArgs);
        }

        private void PressKeyNew(Key key, RoutedEvent routedEvent)
        {
            var eventArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key);
            eventArgs.RoutedEvent = routedEvent;
            InputManager.Current.ProcessInput(eventArgs);
        }

        private void btnAlphaNumeric_Click(object sender, RoutedEventArgs e)
        {
            if (this.EditText != null)
            {
                this.EditText.Focus();
            }

            Button btn = (sender as Button);
            if (btn != null)
            {
                if (btn.Tag == null)
                {
                    PressTextKey(btn.Content.ToString());
                }
                else
                {
                    PressKeyNew((Key)Convert.ToInt32(btn.Tag), Keyboard.KeyDownEvent);
                }
                if (this.btnShift.IsChecked ?? false)
                {
                    this.btnShift.IsChecked = false;
                    UpDateKeyBoard();
                }
            }
        }

        private void Ctrl_Click(object sender, RoutedEventArgs e)
        {
            this.UpDateKeyBoard();
        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            this.UpDateKeyBoard();
        }

        private void CapsLock_Click(object sender, RoutedEventArgs e)
        {
            this.UpDateKeyBoard();
            this.btnAlphaNumeric_Click(sender, e);
        }

        private void ESC_Click(object sender, RoutedEventArgs e)
        {
            if (this.ESCClick == null)
            {
                if (this.parent == null)
                {
                    return;
                }

                //!! this.parent.SelectNextControl(this.EditText, true, true, false, true);
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                this.ESCClick(sender, e);
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (this.EnterClick == null)
            {
                this.btnAlphaNumeric_Click(sender, e);
            }
            else
            {
                this.EnterClick(sender, e);
            }
        }

    }
}
