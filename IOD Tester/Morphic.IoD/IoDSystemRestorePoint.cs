using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace Morphic.IoD
{
    public class IoDSystemRestorePoint
    {
        public static ManagementBaseObject SetStartPoint(string description)
        {
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath path = new ManagementPath("SystemRestore");
            ObjectGetOptions options = new ObjectGetOptions();
            ManagementClass mc = new ManagementClass(scope, path, options);

            ManagementBaseObject input = mc.GetMethodParameters("CreateRestorePoint");
            input["Description"] = description;
            input["RestorePointType"] = 0;  //APPLICATION_INSTALL
            input["EventType"] = 100;   //BEGIN_SYSTEM_CHANGE

            return mc.InvokeMethod("CreateRestorePoint", input, null);
        }
        public static ManagementBaseObject SetEndPoint(string description)
        {
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath path = new ManagementPath("SystemRestore");
            ObjectGetOptions options = new ObjectGetOptions();
            ManagementClass mc = new ManagementClass(scope, path, options);

            ManagementBaseObject input = mc.GetMethodParameters("CreateRestorePoint");
            input["Description"] = description;
            input["RestorePointType"] = 0;  //APPLICATION_INSTALL
            input["EventType"] = 101;   //END_SYSTEM_CHANGE

            return mc.InvokeMethod("CreateRestorePoint", input, null);
        }

        public static void LoadPoint(string description)
        {
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath path = new ManagementPath("SystemRestore");
            ObjectGetOptions options = new ObjectGetOptions();
            ManagementClass mc = new ManagementClass(scope, path, options);

            ManagementBaseObject input = mc.GetMethodParameters("Restore");

            ManagementBaseObject output = mc.InvokeMethod("Restore", input, null);
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
        }
    }
}
