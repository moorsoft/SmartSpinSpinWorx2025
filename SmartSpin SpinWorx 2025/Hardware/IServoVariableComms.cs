using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSpin.Hardware
{
    public interface IServoVariableComms
    {
        bool WriteVariable(string paramName, int value);

        bool WriteVariable(string paramName, double value);

        int ReadVariableInt32(string paramName);

        double ReadVariableFloat(string paramName);

        void SetVariableData(string paramName, object value);

        object GetVariableData(string paramName);

        void MintNetFloat(NetDataParam netparam, float val);

        float MintNetFloat(NetDataParam netparam);

        void MintNetInteger(NetDataParam netparam, int val);

        int MintNetInteger(NetDataParam netparam);

        uint GetInputs();
    }
}
