//------------------------------------------------------------------------------
// <copyright file="ToolsCommandPackage.cs" company="IcodeNet">
//     Copyright (c) IcodeNet.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using EnvDTE80;
using IcodeNet;
using IcodeNet.Helpers;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Process = System.Diagnostics.Process;

namespace IcodeNet
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(ToolsCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(OptionPage),"KSS Tools", "General",0, 0, true)]
    public sealed class ToolsCommandPackage : Package
    {
        /// <summary>
        /// ToolsCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3f20b2b1-69d5-4e30-9426-82900a1b80d5";

        private static List<OleMenuCommand> commands;

        private static OleMenuCommand baseCommand;

        private static Dictionary<OleMenuCommand, Process> processes;

        private string lastFile;

        public static DTE2 dte;

        private string packageFile;

        private bool isParent;

        private bool isChild;

        private OptionPage Options
        {
            get
            {
                return (OptionPage)base.GetDialogPage(typeof(OptionPage));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullCleanCommand"/> class.
        /// </summary>
        public ToolsCommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            ToolsCommandPackage.processes = new Dictionary<OleMenuCommand, Process>();
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            FullCleanCommand.Initialize(this);
            base.Initialize();


            ToolsCommandPackage.dte = base.GetService(typeof(DTE)) as DTE2;
            DTE service = (DTE)base.GetService(typeof(DTE));
            service.Events.BuildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(this.OnOnBuildBegin);
            OleMenuCommandService oleMenuCommandService = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (oleMenuCommandService != null)
            {
               // ToDO
            }


            var dialogPage = GetDialogPage(typeof(OptionPage)) as OptionPage;

            var dialogDirectories = new List<string>(); 
            var dialogExcludedDirectories = new List<string>(); 

            if (dialogPage != null)
            {
                DTE dte = (DTE)GetService(typeof(DTE));

                dialogPage.Directories = new string[0];
                dialogPage.RemoveDirectories = new string[0];

                if (!String.IsNullOrEmpty(dte.Solution.FullName))
                {
                    string solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);
                    dialogDirectories.Add(solutionDir);
                    dialogPage.Directories = dialogDirectories.ToArray();
                }

                dialogPage.Verbose = true;

                dialogExcludedDirectories.Add("sql");
                dialogExcludedDirectories.Add("packages");
                dialogExcludedDirectories.Add("logs");

                dialogPage.ExcludeDirectories = dialogExcludedDirectories.ToArray();
            }

        }

        private static void RunProcess(OleMenuCommand cmd, string argument, bool fromRoot)
        {
            ToolsCommandPackage.dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationBuild);
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = (fromRoot ? SolutionHelpers.GetRootFolder(ToolsCommandPackage.dte) : Path.GetDirectoryName(SolutionHelpers.GetSourceFilePath())),
                    FileName = "cmd",
                    Arguments = argument
                };
                ProcessStartInfo processStartInfo1 = processStartInfo;
                Process process = new Process()
                {
                    StartInfo = processStartInfo1,
                    EnableRaisingEvents = true
                };
                Process process1 = process;
                OutputHelpers.Output(string.Concat("Executing ", cmd.Text, " \r\n\r\n"), true);
                process1.OutputDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), false));
                process1.ErrorDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), false));
                process1.Exited += new EventHandler((object x, EventArgs y) => {
                    ToolsCommandPackage.processes.Remove(cmd);
                    cmd.Checked = false;
                    ToolsCommandPackage.dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationBuild);
                });
                process1.Start();
                process1.BeginOutputReadLine();
                process1.BeginErrorReadLine();
                cmd.Checked = true;
                ToolsCommandPackage.processes.Add(cmd, process1);
            }
            catch (Exception exception)
            {
                OutputHelpers.Output(exception.Message, false);
            }
        }

        private void OnOnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            if (!this.Options.StopProcessesOnBuild)
            {
                return;
            }
            foreach (KeyValuePair<OleMenuCommand, Process> process in ToolsCommandPackage.processes)
            {
                ProcessHelpers.KillProcessAndChildren(process.Value.Id);
            }
            ToolsCommandPackage.processes = new Dictionary<OleMenuCommand, Process>();
        }

        #endregion
    }
}
