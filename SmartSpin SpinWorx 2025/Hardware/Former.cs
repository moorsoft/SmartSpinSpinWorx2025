using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class Former
    {
        public bool FormerAvail;

        public readonly ControllerAxis HAxis;

        public readonly ControllerAxis VAxis;

        public Former(XmlNode setupNode)
        {
            FormerAvail = (setupNode != null);
            XmlNode data;
            if (FormerAvail)
            {
                data = setupNode.SelectSingleNode("HAxis");
                if (data != null) HAxis = Machine.Axes[Convert.ToInt32(data.InnerXml)];
                data = setupNode.SelectSingleNode("VAxis");
                if (data != null) VAxis = Machine.Axes[Convert.ToInt32(data.InnerXml)];
            }
        }

    }
}
