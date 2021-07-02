using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.Win32;

namespace Morphic.IoD
{
    public class IoDSystemRestorePoint
    {
        public static bool SetStartPoint(string description)
        {
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath path = new ManagementPath("SystemRestore");
            ObjectGetOptions options = new ObjectGetOptions();
            ManagementClass mc = new ManagementClass(scope, path, options);

            ManagementBaseObject input = mc.GetMethodParameters("CreateRestorePoint");
            input["Description"] = description;
            input["RestorePointType"] = 0;  //APPLICATION_INSTALL
            input["EventType"] = 100;   //BEGIN_SYSTEM_CHANGE

            var output = mc.InvokeMethod("CreateRestorePoint", input, null);
            try
            {
                uint exitval = (uint)output["ReturnValue"];
                if (exitval == 0)   //I have found this return value cannot be trusted so must do independent verification
                {
                    mc = new ManagementClass(scope, path, options);
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        var ptdesc = mo["Description"].ToString();

                        if (ptdesc == description)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;   //error codes here
                }
            }
            catch
            {
                return false;   //bad output
            }
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

        public static void EnableRestore()
        {

        }

        public static void DisableRestore()
        {

        }

        public static void EnableMultiPointRestore()
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SystemRestore");
                if (key != null)
                {
                    key.SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord);
                }
            }
            catch
            {
                return;
            }
        }

        public static void DisableMultiPointRestore()
        {
            RegistryKey? key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\SystemRestore");

            if (key != null)
            {
                key.SetValue("SystemRestorePointCreationFrequency", 1440);
            }
        }

        public static bool Restore(string description)
        {
            ManagementScope scope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath path = new ManagementPath("SystemRestore");
            ObjectGetOptions options = new ObjectGetOptions();
            ManagementClass mc = new ManagementClass(scope, path, options);

            uint sequence = 0;

            foreach (ManagementObject mo in mc.GetInstances())
            {
                var ptdesc = mo["Description"].ToString();

                if (ptdesc == description)
                {
                    sequence = (uint)mo["SequenceNumber"];
                    break;
                }
            }

            if (sequence == 0)
            {
                return false;   //restore point not found
            }

            ManagementBaseObject input = mc.GetMethodParameters("Restore");
            input["SequenceNumber"] = sequence;

            ManagementBaseObject output = mc.InvokeMethod("Restore", input, null);
            try
            {
                uint exitval = (uint)output["ReturnValue"];
                if (exitval == 0)
                {
                    return true;
                }
                else
                {
                    return false;   //error codes here
                }
            }
            catch
            {
                return false;   //bad output
            }

            //System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
        }
    }
}
