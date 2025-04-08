using System;
using System.Windows;

namespace SmartSpin.Dialogs
{
    /// <summary>
    /// Interaction logic for EditDialog.xaml
    /// </summary>
    public partial class EditDialog : Window
    {
        public EditDialog()
        {
            InitializeComponent();
            Owner = App.Current.MainWindow;
            keyboard.EditText = this.textBox;
        }

        static string Result = String.Empty;

        public static bool? Show(string prompt)
        {
            string res;
            return Show(prompt, String.Empty, String.Empty, out res);
        }

        public static bool? Show(string prompt, string caption)
        {
            string res;
            return Show(prompt, caption, String.Empty, out res);
        }

        public static bool? Show(string prompt, string caption, string init, out string result)
        {
            EditDialog ed = new EditDialog();

            ed.Title = caption;
            ed.lblPrompt.Content = prompt;
            ed.textBox.Text = init;
            Result = init;
            ed.textBox.Focus();
            bool? dr = ed.ShowDialog();
            if (dr ?? false) Result = ed.textBox.Text; else Result = init;
            result = Result;
            return dr;
        }

        private void keyboard_EnterClick(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        private void keyboard_ESCClick(object sender, EventArgs e)
        {
            DialogResult = false;
        }

    }
}
