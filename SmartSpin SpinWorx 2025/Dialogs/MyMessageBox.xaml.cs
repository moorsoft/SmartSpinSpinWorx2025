using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartSpin.Dialogs
{
    /// <summary>
    /// Interaction logic for MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        MessageBoxResult mbResult = MessageBoxResult.None;

        public MyMessageBox()
        {
            InitializeComponent();
            Owner = App.Current.MainWindow;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            mbResult = MessageBoxResult.OK;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            mbResult = MessageBoxResult.Cancel;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            mbResult = MessageBoxResult.Yes;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            mbResult = MessageBoxResult.No;
        }

        public static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public static MessageBoxResult Show(
            Window owner,
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon
        )
        {
            MyMessageBox mb = new MyMessageBox();
            mb.Owner = owner;
            mb.Title = caption;
            mb.txtMessage.Text = messageBoxText;
            mb.btnOk.Visibility = (((button == MessageBoxButton.OK) || (button == MessageBoxButton.OKCancel)) ? Visibility.Visible : Visibility.Collapsed);
            mb.btnCancel.Visibility = (((button == MessageBoxButton.OKCancel) || (button == MessageBoxButton.YesNoCancel)) ? Visibility.Visible : Visibility.Collapsed);
            mb.btnYes.Visibility = (((button == MessageBoxButton.YesNo) || (button == MessageBoxButton.YesNoCancel)) ? Visibility.Visible : Visibility.Collapsed);
            mb.btnNo.Visibility = (((button == MessageBoxButton.YesNo) || (button == MessageBoxButton.YesNoCancel)) ? Visibility.Visible : Visibility.Collapsed);

            switch (icon)
            {
                case MessageBoxImage.Information:
                    mb.imgIcon.Source = ToImageSource(System.Drawing.SystemIcons.Information);
                    break;
                case MessageBoxImage.Warning:
                    mb.imgIcon.Source = ToImageSource(System.Drawing.SystemIcons.Warning);
                    break;
                case MessageBoxImage.Error:
                    mb.imgIcon.Source = ToImageSource(System.Drawing.SystemIcons.Error);
                    break;
                case MessageBoxImage.Question:
                    mb.imgIcon.Source = ToImageSource(System.Drawing.SystemIcons.Question);
                    break;
            }
            
            bool? dr = mb.ShowDialog();
            if (dr == null) return MessageBoxResult.None;
            bool b = (dr ?? false);
            if (b) return mb.mbResult;
            return MessageBoxResult.Cancel;
        }

        public static MessageBoxResult Show(
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon
        )
        {
            return Show(App.Current.MainWindow, messageBoxText, caption, button, icon);
        }

        public static MessageBoxResult Show(
            string messageBoxText,
            string caption,
            MessageBoxButton button
        )
        {
            return Show(App.Current.MainWindow, messageBoxText, caption, button, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(
            string messageBoxText,
            MessageBoxButton button
        )
        {
            return Show(App.Current.MainWindow, messageBoxText, "MessageBox", button, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(
            string messageBoxText
        )
        {
            return Show(App.Current.MainWindow, messageBoxText, "MessageBox", MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult ShowError(
            string messageBoxText,
            string caption
        )
        {
            return MyMessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
