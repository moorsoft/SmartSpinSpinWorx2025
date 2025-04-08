using CommunityToolkit.Mvvm.ComponentModel;
using SmartSpin.Hardware;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SmartSpin.View
{
    public partial class DisplayItem : ObservableRecipient
    {
        [ObservableProperty]
        private string text;

        public static SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush greenBrush = new SolidColorBrush(Colors.Lime);
        public static SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);

        [ObservableProperty]
        private SolidColorBrush background;
    }

    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        public ObservableCollection<DisplayItem> Items { get; set; } = new ObservableCollection<DisplayItem>();

        public Startup()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Establishing the connection to the controller and initialising is done
        /// in a background thread.
        /// </summary>
        public void TheMachineConnect(IServiceProvider _serviceProvider)
        {
            bool AnyFailed = false;
            Items.Add(new DisplayItem() { Text = "Reading Machine configuration", Background = DisplayItem.greenBrush });
            ProcessPendingEvents(); // Simulate DoEvents
            string SettingsFileName = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            // SettingsFileName+Path.DirectorySeparatorChar+

            Items.Add(new DisplayItem() { Text = "Initialising Controller", Background = DisplayItem.greenBrush });
            ProcessPendingEvents(); // Simulate DoEvents
            Machine.SetupLink(_serviceProvider, "setup.xml");

            Items.Add(new DisplayItem() { Text = "Startup Complete", Background = (AnyFailed ? DisplayItem.redBrush : DisplayItem.greenBrush) });
            ProcessPendingEvents(); // Simulate DoEvents

            //while (!OKPressed)
            //{
            //    Thread.Sleep(50);
            //}
        }

        private void ProcessPendingEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
