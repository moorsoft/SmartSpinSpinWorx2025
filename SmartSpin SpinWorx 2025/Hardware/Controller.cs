using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MintControls_5864Lib;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SmartSpin.Hardware
{
    public class Controller(ILogger<Controller> logger, IServiceProvider serviceProvider, MintController mintController)
    {
        private const int RetryCount = 5;

        enum ConnectionType
        {
            NotConnected,
            USBConnection,
            EthernetConnect,
            VirtualConnection
        }

        private ConnectionType SavedConnectionType = ConnectionType.NotConnected;
        private string SavedLinkName = String.Empty;
        private short SavedUSBNode = -1;

        public void SetEthernetControllerLink(string sLinkName)
        {
            SavedConnectionType = ConnectionType.EthernetConnect;
            SavedLinkName = sLinkName;
            mintController.SetEthernetControllerLink(sLinkName);
        }

        public void SetUSBControllerLink(int Node)
        {
            SavedConnectionType = ConnectionType.USBConnection;
            SavedUSBNode = (short)Node;
            mintController.SetUSBControllerLink(SavedUSBNode);
        }

        public void SetVirtualControllerLink()
        {
            SavedConnectionType = ConnectionType.VirtualConnection;
            mintController.SetVirtualControllerLink();
        }

        public bool Virtual => SavedConnectionType == ConnectionType.VirtualConnection;
        public void ReOpenController(Exception e, string FunctionName, int passNumber)
        {
            logger.LogError(e, "func: {FunctionName} pass: {passNumber}", FunctionName, passNumber);
            Thread.Sleep(200);
            mintController = serviceProvider.GetRequiredService<MintController>();
            switch (SavedConnectionType)
            {
                case ConnectionType.EthernetConnect:
                    SetEthernetControllerLink(SavedLinkName);
                    break;
                case ConnectionType.USBConnection:
                    SetUSBControllerLink(SavedUSBNode);
                    break;
                case ConnectionType.VirtualConnection:
                    SetVirtualControllerLink();
                    break;
            }
        }

        private void DoActionWithRetry(Action action, string name)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception e)
                {
                    ReOpenController(e, name, i);
                    if (i == RetryCount-1) throw;
                }
            }
        }

        private T DoFunctionWithRetry<T>(Func<T> function, string name)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    return function();
                }
                catch (Exception e)
                {
                    ReOpenController(e, name, i);
                    if (i == RetryCount-1) throw;
                }
            }
            return default;
        }

        public string ActiveXVersion
        {
            get
            {
                return DoFunctionWithRetry(() => mintController.ActiveXVersion, nameof(ActiveXVersion));
            }
        }

        public float getPos(short nAxis)
        {
            return DoFunctionWithRetry(() => mintController.get_Pos(nAxis), nameof(getPos));
        }

        public float getPosTarget(short nAxis)
        {
            return DoFunctionWithRetry(() => mintController.get_PosTarget(nAxis), nameof(getPosTarget));
        }

        public float getPosRemaining(short nAxis)
        {
            return DoFunctionWithRetry(() => mintController.get_PosRemaining(nAxis), nameof(getPosRemaining));
        }

        public bool getHomeStatus(short nAxis)
        {
            return DoFunctionWithRetry(() => mintController.get_HomeStatus(nAxis), nameof(getHomeStatus));
        }

        public int getAxisStatusWord(short nAxis)
        {
            return DoFunctionWithRetry(() => mintController.get_AxisStatusWord(nAxis), nameof(getAxisStatusWord));
        }

        public int getIn(short nBank)
        {
            return DoFunctionWithRetry(() => mintController.get_In(nBank), nameof(getIn));
        }

        public bool MintExecuting
        {
            get
            {
                return DoFunctionWithRetry(() => mintController.MintExecuting, nameof(MintExecuting));
            }
        }

        public void DoMintRun()
        {
            DoActionWithRetry(() => mintController.DoMintRun(), nameof(DoMintRun));
        }

        private string firmwareVersion;
        public string FirmwareVersion
        {
            get
            {
                return DoFunctionWithRetry(() => firmwareVersion ??= mintController.FirmwareVersion, nameof(FirmwareVersion));
            }
        }

        public bool Connected
        {
            get
            {
                try
                {
                    return (mintController.ControllerType > 12);
                }
                catch (COMException)
                {
                    return false;
                }
            }
        }

        private void setNetFloat(short nIndex, float param0)
        {
            DoActionWithRetry(() => mintController.set_NetFloat(nIndex, param0), nameof(setNetFloat));
        }

        private void setNetInteger(short nIndex, int param0)
        {
            DoActionWithRetry(() => mintController.set_NetInteger(nIndex, param0), nameof(setNetInteger));
        }

        private float getNetFloat(short nIndex)
        {
            return DoFunctionWithRetry(() => mintController.get_NetFloat(nIndex), nameof(getNetFloat));
        }

        private int getNetInteger(short nIndex)
        {
            return DoFunctionWithRetry(() => mintController.get_NetInteger(nIndex), nameof(getNetInteger));
        }

        private void setNVLong(short nAddress, int param0)
        {
            DoActionWithRetry(() => mintController.set_NVLong(nAddress, param0), nameof(setNVLong));
        }

        private int getNVLong(short nAddress)
        {
            return DoFunctionWithRetry(() => mintController.get_NVLong(nAddress), nameof(getNVLong));
        }

        private object getVariableData(string bstrTaskName, string bstrVarName)
        {
            return DoFunctionWithRetry(() => mintController.get_VariableData(bstrTaskName, bstrVarName), nameof(getVariableData));
        }

        private void setVariableData(string bstrTaskName, string bstrVarName, object param0)
        {
            DoActionWithRetry(() => mintController.set_VariableData(bstrTaskName, bstrVarName, param0), nameof(setVariableData));
        }

        internal void MintVariableData(string bstrTaskName, string bstrVarName, object param0)
        {
            try
            {
                setVariableData(bstrTaskName, bstrVarName, param0);
            }
            catch (Exception e)
            {
                logger.LogError(e, "MintVariableData writing {bstrVarName}", bstrVarName);
                //MyMessageBox.ShowError($"{e.Message} writing {bstrVarName}", "Mint Writing Error");
            }
        }

        internal object MintVariableData(string bstrTaskName, string bstrVarName)
        {
            try
            {
                return getVariableData(bstrTaskName, bstrVarName);
            }
            catch (Exception e)
            {
                logger.LogError(e, "MintVariableData reading {bstrVarName}", bstrVarName);
                //MyMessageBox.ShowError($"{e.Message} reading {bstrVarName}", "Mint Writing Error");
                return null;
            }
        }

        internal void MintNetFloat(NetDataParam netparam, float val)
        {
            setNetFloat((short)netparam, val);
        }

        internal float MintNetFloat(NetDataParam netparam)
        {
            return getNetFloat((short)netparam);
        }

        internal void MintNetInteger(NetDataParam netparam, int val)
        {
            setNetInteger((short)netparam, val);
        }

        internal int MintNetInteger(NetDataParam netparam)
        {
            return getNetInteger((short)netparam);
        }

        internal void MintNVLong(NVMemory nvAddress, int v)
        {
            setNVLong((short)nvAddress, v);
        }

        //internal void MintNVFloat(NVMemory nvAddress, float v)
        //{
        //    setNVFloat((short)nvAddress, v);
        //}
    }
}
