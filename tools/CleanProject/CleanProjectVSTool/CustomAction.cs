namespace CleanProjectVSTool
{
    using System.IO;

    using Microsoft.Deployment.WindowsInstaller;
    using Microsoft.Win32;

    public class CustomActions
    {
        #region Public Methods

        [CustomAction]
        public static ActionResult AddCleanProjectTool(Session session)
        {
            session.Log("Begin AddCleanProjectTool");

            // Read HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\10.0\External Tools
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0\External Tools", true);
            if (key != null)
            {
                // The number has the value for the next tool 
                // If there are 5 tools, the number will be 6
                var numTools = (int)key.GetValue("ToolNumKeys", 0);

                var tool = new ExternalTool
                    {
                        Args = "/D:\"$(SolutionDir)\" /Z /R",
                        Command = Path.Combine(session["INSTALLLOCATION"], "CleanProject.exe"),
                        Directory = string.Empty,
                        Opt = 24,
                        SourceKey = string.Empty,
                        Title = "Clean, Remove Source Bindings and Zip Solution"
                    };
                ExternalTool.Write(key, numTools, tool);

                // Setup for the next tool
                key.SetValue("ToolNumKeys", numTools + 1);
            }
            session.Log("End AddCleanProjectTool");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveCleanProjectTool(Session session)
        {
            session.Log("Begin RemoveCleanProjectTool");

            // Read HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\10.0\External Tools
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0\External Tools", true);
            if (key != null)
            {
                // The number has the value for the next tool 
                // If there are 5 tools, the number will be 6
                ExternalTool.Remove(key, (int)key.GetValue("ToolNumKeys", 0));
            }
            session.Log("End RemoveCleanProjectTool");
            return ActionResult.Success;
        }

        #endregion

        #region Methods

        #endregion
    }

    internal class ExternalTool
    {
        #region Constants and Fields

        private const string CleanProjectSig = "CLEAN-PROJECT";

        #endregion

        #region Properties

        internal string Args { get; set; }

        internal string Command { get; set; }

        internal string Directory { get; set; }

        internal string SourceKey { get; set; }

        internal string Title { get; set; }

        internal long Opt { get; set; }

        #endregion

        #region Public Methods

        public static void Remove(RegistryKey key, int numTools)
        {
            var installedTool = FindTool(key, numTools);

            if (installedTool == -1)
            {
                return;
            }

            Delete(key, installedTool);

            // Adjust the remaining tools
            for (var i = installedTool + 1; i < numTools; i++)
            {
                MoveTool(key, i, i - 1);
            }

            key.SetValue("ToolNumKeys", numTools - 1);
        }

        #endregion

        #region Methods

        internal static ExternalTool Read(RegistryKey key, int num)
        {
            var et = new ExternalTool
                {
                    Title = (string)key.GetValue(ToolTitle(num)),
                    SourceKey = (string)key.GetValue(ToolSourceKey(num)),
                    Directory = (string)key.GetValue(ToolDir(num)),
                    Command = (string)key.GetValue(ToolCmd(num)),
                    Args = (string)key.GetValue(ToolArg(num)),
                };

            return et;
        }

        internal static string ToolArg(int num)
        {
            return string.Format("ToolArg{0}", num);
        }

        internal static string ToolCmd(int num)
        {
            return string.Format("ToolCmd{0}", num);
        }

        internal static string ToolDir(int num)
        {
            return string.Format("ToolDir{0}", num);
        }

        internal static string ToolOpt(int num)
        {
            return string.Format("ToolOpt{0}", num);
        }

        internal static string ToolSourceKey(int num)
        {
            return string.Format("ToolSourceKey{0}", num);
        }

        internal static string ToolTitle(int num)
        {
            return string.Format("ToolTitle{0}", num);
        }

        internal static void Write(RegistryKey key, int num, ExternalTool et)
        {
            key.SetValue(ToolTitle(num), et.Title);
            key.SetValue(ToolSourceKey(num), CleanProjectSig);
            key.SetValue(ToolDir(num), et.Directory);
            key.SetValue(ToolCmd(num), et.Command);
            key.SetValue(ToolArg(num), et.Args);
            key.SetValue(ToolOpt(num), et.Opt, RegistryValueKind.DWord);
        }

        private static void Delete(RegistryKey key, int num)
        {
            key.DeleteValue(ToolTitle(num));
            key.DeleteValue(ToolSourceKey(num));
            key.DeleteValue(ToolDir(num));
            key.DeleteValue(ToolCmd(num));
            key.DeleteValue(ToolArg(num));
            key.DeleteValue(ToolOpt(num));
        }

        private static int FindTool(RegistryKey key, int numTools)
        {
            for (var i = 0; i < numTools; i++)
            {
                var tool = Read(key, i);
                if (tool.SourceKey == CleanProjectSig)
                {
                    return i;
                }
            }

            return -1;
        }

        private static void MoveTool(RegistryKey key, int source, int dest)
        {
            var et = Read(key, source);
            Write(key, dest, et);
            Delete(key, source);
        }

        #endregion
    }
}