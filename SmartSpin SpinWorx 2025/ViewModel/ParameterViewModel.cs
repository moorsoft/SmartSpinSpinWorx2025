using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;

namespace SmartSpin.ViewModel
{
    public partial class ParameterViewModel : MyViewModelBase
    {
        [ObservableProperty]
        private string settingsPath;

        [ObservableProperty]
        private string label;

        [ObservableProperty]
        private object settingValue;

        [ObservableProperty]
        private string format;

        [RelayCommand]
        public void ChangeParameter()
        {
            if (SettingValue is bool b)
            {
                SettingValue = !b;
                Machine.Setupfile.SetupWriteValue(this.SettingsPath, this.Label, b);
            }
            else if (SettingValue is int i)
            {
                double d = i;
                if (Calculator.ShowCalculator(ref d, "Enter Value"))
                {
                    SettingValue = (int)d;
                    Machine.Setupfile.SetupWriteValue(this.SettingsPath, this.Label, (int)d);
                }
            }
            else if (SettingValue is double d)
            {
                if (Calculator.ShowCalculator(ref d, "Enter Value"))
                {
                    SettingValue = d;
                    Machine.Setupfile.SetupWriteValue(this.SettingsPath, this.Label, d);
                }
            }
            else if (SettingValue is string s)
            {
                if (EditDialog.Show("Enter value", "Change Parameter", s, out string NewValue) ?? false)
                {
                    SettingValue = NewValue;
                    Machine.Setupfile.SetupWriteValue(this.SettingsPath, this.Label, NewValue);
                }
            }
        }

        public double ValueAsDouble
        {
            get
            {
                return (double)SettingValue;
            }
        }

        public bool ValueAsBoolean
        {
            get
            {
                return (bool)SettingValue;
            }
        }
    }
}