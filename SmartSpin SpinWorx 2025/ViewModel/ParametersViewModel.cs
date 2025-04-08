using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SmartSpin.ViewModel
{
    public class ParametersViewModel : ObservableCollection<ParameterViewModel>
    {
        private string XmlPath;

        private XmlNode setupNode;

        public ParametersViewModel(XmlNode _setupNode)
        {
            setupNode = _setupNode;
            XmlPath = GetNodePath(_setupNode);
        }

        private string GetNodePath(XmlNode setupNode)
        {
            if (setupNode is XmlDocument)
            {
                return "";
            }
            else
            {
                return GetNodePath(setupNode.ParentNode) + '/' + setupNode.Name;
            }
        }

        internal string CreateParameter(string Label, string defaultValue)
        {
            string v;
            XmlNode data = setupNode.SelectSingleNode(Label);
            if (data != null)
            {
                v = data.InnerXml;
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
            int v;
            XmlNode data = setupNode.SelectSingleNode(Label);
            if (data != null)
            {
                v = Convert.ToInt32(data.InnerXml);
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
            double v;
            XmlNode data = setupNode.SelectSingleNode(Label);
            if (data != null)
            {
                v = Convert.ToDouble(data.InnerXml);
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
            bool v;
            XmlNode data = setupNode.SelectSingleNode(Label);
            if (data != null)
            {
                v = Convert.ToBoolean(data.InnerXml);
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
