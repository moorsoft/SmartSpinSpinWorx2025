using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (SettingValue is bool)
            {
                SettingValue = !(bool)SettingValue;
                Machine.setupWriteValue(this.SettingsPath, this.Label, SettingValue.ToString());
                Machine.SaveSetupFile();
            }
            else if (SettingValue is int)
            {
                double d = (int)SettingValue;
                if (Calculator.ShowCalculator(ref d, "Enter Value"))
                {
                    SettingValue = (int)d;
                    Machine.setupWriteValue(this.SettingsPath, this.Label, SettingValue.ToString());
                    Machine.SaveSetupFile();
                }
            }
            else if (SettingValue is double)
            {
                double d = (double)SettingValue;
                if (Calculator.ShowCalculator(ref d, "Enter Value"))
                {
                    SettingValue = d;
                    Machine.setupWriteValue(this.SettingsPath, this.Label, SettingValue.ToString());
                    Machine.SaveSetupFile();
                }
            }
            else if (SettingValue is string)
            {
                if (EditDialog.Show("Enter value", "Change Parameter", SettingValue as string, out string NewValue) ?? false)
                {
                    SettingValue = NewValue;
                    Machine.setupWriteValue(this.SettingsPath, this.Label, SettingValue.ToString());
                    Machine.SaveSetupFile();
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