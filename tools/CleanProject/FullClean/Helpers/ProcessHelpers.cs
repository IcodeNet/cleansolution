using System;
using System.Diagnostics;
using System.Management;

namespace IcodeNet.Helpers
{
    public static class ProcessHelpers
    {
        public static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(string.Concat("Select * From Win32_Process Where ParentProcessID=", pid));
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
                ProcessHelpers.KillProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"]));
            }
            try
            {
                Process.GetProcessById(pid).Kill();
            }
            catch (ArgumentException argumentException)
            {
            }
        }
    }
}