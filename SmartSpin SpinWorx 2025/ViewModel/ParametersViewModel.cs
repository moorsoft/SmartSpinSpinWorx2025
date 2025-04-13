using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Nodes;

namespace SmartSpin.ViewModel
{
    public class ParametersViewModel(JsonNode _setupNode) : ObservableCollection<ParameterViewModel>
    {
        private readonly string XmlPath = _setupNode.GetPath();

        internal string CreateParameter(string Label, string defaultValue)
        {
            string v = String.Empty;
            if (_setupNode[Label] is JsonNode data)
            {
                v = data.ToString();
            }
            else
            {
                v = defaultValue;
            }
            this.Add(new ParameterViewModel() { Label = Label, SettingsPath = XmlPath, SettingValue = v });
            return v;
        }

        internal int CreateParameter(string Label, int defaultValue)
        {
            int v = 0;
            if (_setupNode[Label] is JsonNode data)
            {
                v = (int)data;
            }
            else
            {
                v = defaultValue;
            }
            this.Add(new ParameterViewModel() { Label = Label, SettingsPath = XmlPath, SettingValue = v });
            return v;
        }


        internal double CreateParameter(string Label, string format, double defaultValue)
        {
            double v = 0;
            if (_setupNode[Label] is JsonNode data)
            {
                v = (double)data;
            }
            else
            {
                v = defaultValue;
            }
            this.Add(new ParameterViewModel() { Label = Label, SettingsPath = XmlPath, Format = format, SettingValue = v });
            return v;
        }

        internal bool CreateParameter(string Label, bool defaultValue)
        {
            bool v = false;
            if (_setupNode[Label] is JsonNode data)
            {
                v = (bool)data;
            }
            else
            {
                v = defaultValue;
            }
            this.Add(new ParameterViewModel() { Label = Label, SettingsPath = XmlPath, Format = "b", SettingValue = v });
            return v;
        }

        internal ParameterViewModel Find(string Label)
        {
            return this.FirstOrDefault(x => x.Label == Label);
        }

    }
}
