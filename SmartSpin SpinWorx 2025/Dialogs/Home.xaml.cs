using SmartSpin.Hardware;
using SmartSpin.Support;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SmartSpin.Dialogs
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public bool HomingAxis2 = false;
        public bool HomingAxis3 = false;
        public bool HomingAxis4 = false;
        public bool HomingVAxis = false;
        public bool HomingHAxis = false;

        public Home()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //dispatcherTimer_Tick(sender, e);

            lblInOutAxisStatus.Content = "In/Out Axis : Homing";
            lblLeftRightAxisStatus.Content = "Left/Right Axis : Waiting";
            lblBAxisStatus.Content = "B Axis : Waiting";
            if (Globals.TotalAxes > 3) Machine.ServoDrive[3].HomeAxis();
            HomingAxis3 = true;
            HomingAxis2 = false;
            HomingAxis4 = false;
            if (Machine.TopFormer.VAxis != null)
            {
                lblVAxisStatus.Content = "Former Y1 : Homing";
                HomingVAxis = true;
                (Machine.TopFormer.VAxis as IServoDrive).HomeAxis();
            }
            if (Machine.TopFormer.HAxis != null)
            {
                lblHAxisStatus.Content = "Former Z1 : Waiting";
            }
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            try
            {
                if (HomingAxis3)
                {
                    if (Globals.TotalAxes > 3)
                    {
                        if (Machine.ServoDrive[3].HomeComplete())
                        {
                            lblInOutAxisStatus.Content = "In/Out Axis : Home Complete";
                            lblLeftRightAxisStatus.Content = "Left/Right Axis : Homing";

                            if (Globals.TotalAxes > 2) Machine.ServoDrive[2].HomeAxis();
                            HomingAxis3 = false;
                            HomingAxis2 = true;
                        }
                    }
                    else
                    {
                        HomingAxis3 = false;
                        HomingAxis2 = true;
                    }
                }
                if (HomingAxis2)
                {
                    if (Globals.TotalAxes > 2)
                    {
                        if (Machine.ServoDrive[2].HomeComplete())
                        {
                            lblLeftRightAxisStatus.Content = "Left/Right Axis : Home Complete";
                            HomingAxis2 = false;
                            if (Machine.BAxis != null)
                            {
                                lblBAxisStatus.Content = "B Axis : Homing";
                                (Machine.BAxis as IServoDrive).HomeAxis();
                                HomingAxis4 = true;
                            }
                            else
                            {
                                lblBAxisStatus.Content = "B Axis : Home Not required";
                            }
                        }
                    }
                }
                if (HomingAxis4)
                {
                    if (Globals.TotalAxes > 4)
                    {
                        if (Machine.ServoDrive[4].HomeComplete())
                        {
                            lblBAxisStatus.Content = "B Axis : Home Complete";
                            HomingAxis4 = false;
                        }
                    }
                }
                if (HomingVAxis)
                {
                    if ((Machine.TopFormer.VAxis as IServoDrive).HomeComplete())
                    {
                        HomingVAxis = false;
                        lblVAxisStatus.Content = "Former Y1 : Home Complete";
                        if (Machine.TopFormer.HAxis is IServoDrive)
                        {
                            HomingHAxis = true;
                            (Machine.TopFormer.HAxis as IServoDrive).HomeAxis();
                            lblHAxisStatus.Content = "Former Z1 : Homing";
                        }
                    }

                }
                if (HomingHAxis)
                {
                    if ((Machine.TopFormer.HAxis as IServoDrive).HomeComplete())
                    {
                        HomingHAxis = false;
                        lblHAxisStatus.Content = "Former Z1 : Home Complete";
                    }
                }
            }
            finally
            {
                dispatcherTimer.Start();
            }
            if (!HomingAxis2 && !HomingAxis3 && !HomingAxis4 && !HomingHAxis && !HomingVAxis)
            {
                dispatcherTimer.Stop();
                this.DialogResult = true;
            }
        }

        public static bool? ShowModal()
        {
            Home home = new Home();
            return home.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            if (Globals.TotalAxes > 2) Machine.ServoDrive[2].CancelMove();
            if (Globals.TotalAxes > 3) Machine.ServoDrive[3].CancelMove();
            if (Globals.TotalAxes > 4) Machine.ServoDrive[4].CancelMove();
            if (Machine.TopFormer.HAxis is IServoDrive) (Machine.TopFormer.HAxis as IServoDrive).CancelMove();
            if (Machine.TopFormer.VAxis is IServoDrive) (Machine.TopFormer.VAxis as IServoDrive).CancelMove();
            Thread.Sleep(1000);
            this.DialogResult = false;
        }

    }
}
