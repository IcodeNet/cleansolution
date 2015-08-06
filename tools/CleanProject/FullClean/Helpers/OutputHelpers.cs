using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;


namespace IcodeNet.Helpers
{
    public static class OutputHelpers
    {
        private static IVsOutputWindowPane outputWindowPane;

        private static void Init()
        {
            IVsOutputWindow globalService = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            DTE dTE = (DTE)Package.GetGlobalService(typeof(DTE));
            dTE.Windows.Item("{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}").Visible = true;
            Guid generalPaneGuid = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            Guid guid = generalPaneGuid;
            globalService.CreatePane(ref guid, "KSS Tools execution", 1, 0);
            Guid guid1 = generalPaneGuid;
            globalService.GetPane(ref guid1, out OutputHelpers.outputWindowPane);
        }

        public static void Output(string msg, bool focus = false)
        {
            if (OutputHelpers.outputWindowPane == null)
            {
                OutputHelpers.Init();
            }
            if (focus)
            {
                Package.GetGlobalService(typeof(SVsOutputWindow));
                DTE globalService = (DTE)Package.GetGlobalService(typeof(DTE));
                Window window = globalService.Windows.Item("{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}");
                window.Visible = true;
                window.Activate();
                OutputHelpers.outputWindowPane.Activate();
            }
            OutputHelpers.outputWindowPane.OutputString(msg);
        }
    }
}
