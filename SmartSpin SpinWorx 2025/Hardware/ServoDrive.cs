using System.Text;
using System.Xml;

namespace SmartSpin.Hardware
{
    public class ServoDrive : ControllerAxis, IServoVariableComms
    {
        protected bool _connected = false;
        protected bool LastCommsOK = true;

        protected bool HoldEnable = true;

        protected StringBuilder faultLog = new StringBuilder(30000);

        protected bool moveBetweenMemories = true;

        public ServoDrive(Controller _controller, XmlNode setupNode, int axno) : base(_controller, setupNode, axno)
        {
            Parameters.CreateParameter("MinusLimit", "f1", 0);
            Parameters.CreateParameter("PlusLimit", "f1", 0);
        }

        public bool Simulation
        {
            get
            {
                return (!_connected);
            }
        }

        public bool MoveBetweenMemories
        {
            get
            {
                return moveBetweenMemories;
            }
        }

        #region Register reading-writing
        public bool WriteVariable(string paramName, int value)
        {
            controller.MintVariableData("ParentTask", paramName, value);
            return true;
        }

        public bool WriteVariable(string paramName, double value)
        {
            controller.MintVariableData("ParentTask", paramName, value);
            return true;
        }

        public int ReadVariableInt32(string paramName)
        {
            return (int)controller.MintVariableData("ParentTask", paramName);
        }

        public double ReadVariableFloat(string paramName)
        {
            return (float)controller.MintVariableData("ParentTask", paramName);
        }

        public void SetVariableData(string paramName, object value)
        {
            controller.MintVariableData("ParentTask", paramName, value);
        }

        public object GetVariableData(string paramName)
        {
            return controller.MintVariableData("ParentTask", paramName);
        }


        public void MintNetFloat(NetDataParam netparam, float val)
        {
            controller.MintNetFloat(netparam, val);
        }

        public float MintNetFloat(NetDataParam netparam)
        {
            return controller.MintNetFloat(netparam);
        }

        public void MintNetInteger(NetDataParam netparam, int val)
        {
            controller.MintNetInteger(netparam, val);
        }

        public int MintNetInteger(NetDataParam netparam)
        {
            return controller.MintNetInteger(netparam);
        }

        public uint GetInputs()
        {
            return (uint)controller.getIn(0);
        }
        #endregion
    }
}
