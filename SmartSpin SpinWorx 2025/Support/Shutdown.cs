using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.ComponentModel;

class Shutdown
{
    public static void ShutdownComputer()
    {
        ManagementClass W32_OS = new ManagementClass("Win32_OperatingSystem");
        ManagementBaseObject inParams, outParams;
        int result;
        W32_OS.Scope.Options.EnablePrivileges = true;

        foreach (ManagementObject obj in W32_OS.GetInstances())
        {
            inParams = obj.GetMethodParameters("Win32Shutdown");
            inParams["Flags"] = 12; //Forced Power Off
            inParams["Reserved"] = 0;

            outParams = obj.InvokeMethod("Win32Shutdown", inParams, null);
            result = Convert.ToInt32(outParams["returnValue"]);
            if (result != 0) throw new Win32Exception(result);
        }
        Environment.Exit(0);
    }

}
