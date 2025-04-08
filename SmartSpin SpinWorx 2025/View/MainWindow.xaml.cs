using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using SmartSpin.Properties;
using SmartSpin.Support;
using SmartSpin.ViewModel;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SmartSpin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel model)
        {
            InitializeComponent();
            DataContext = model;

            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length > 0) lblAppName.Content = ((AssemblyProductAttribute)attributes[0]).Product;
            lblAppVersion.Content = String.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version);
            attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0) lblAppCopyright.Content = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;

            //this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.SourceInitialized += MainWindow_SourceInitialized;
            this.Left = 0;
            this.Top = 0;
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 40;

            this.Closed += Window_Closed;
            this.Closing += Window_Closing;
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
        }

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Settings.Default.PartsCounter = (DataContext as MainViewModel).PartsCounter;
            Settings.Default.Save();
            Machine.Shutdown();
            Application.Current.Shutdown();
        }

        private void Shutdown_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Globals.DoShutDown)
            {
                Shutdown.ShutdownComputer();
            }
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Machine.CurrentProgram.ProgramChanged)
            {
                if (MyMessageBox.Show("Your program has not been saved.  Are you sure you want to Shutdown?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
