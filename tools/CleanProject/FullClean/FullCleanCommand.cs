//------------------------------------------------------------------------------
// <copyright file="FullCleanCommand.cs" company="ICodeNet">
//     Copyright (c) ICodeNet.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CleanProject;
using EnvDTE;
using IcodeNet.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;

namespace IcodeNet
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FullCleanCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("18b69ed2-d08e-403f-ab32-3d16b04794bb");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullCleanCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FullCleanCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.StartClean, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FullCleanCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FullCleanCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void StartClean(object sender, EventArgs e)
        {
            //CreatePane(new Guid(), "Full Clean Command", true, false);

            Process process = new Process();
            process.StartInfo.FileName = typeof(CleanOptions).Assembly.Location;


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
                    // WorkingDirectory = (fromRoot ? SolutionHelpers.GetRootFolder(ToolsCommandPackage.dte) : Path.GetDirectoryName(SolutionHelpers.GetSourceFilePath())),
                    // FileName = "cmd",
                    FileName = typeof(CleanOptions).Assembly.Location
                    //Arguments = argument
                };
                
                /*
                ProcessStartInfo processStartInfo1 = processStartInfo;
                Process process = new Process()
                {
                    StartInfo = processStartInfo1,
                    EnableRaisingEvents = true
                };*/
                
                if (!String.IsNullOrEmpty(ToolsCommandPackage.dte.Solution.FullName))
                {

                    var props = ToolsCommandPackage.dte.Properties["KSS Tools", "General"];

                    string[] directories = (string[])props.Item("Directories").Value;
                    string[] excludedDirectories = (string[])props.Item("ExcludeDirectories").Value;
                    string[] removeDirectories = (string[])props.Item("RemoveDirectories").Value;

                    string[] solutionDir = directories.Any() ? directories.ToArray() : new string[] { };

                    if (!solutionDir.Any())
                    {
                        solutionDir = new[] { System.IO.Path.GetDirectoryName(ToolsCommandPackage.dte.Solution.FullName) };

                    }

                    process.StartInfo.Arguments = @"/v ";
                    var targetDirectoriesArguments = String.Empty;
                    var excludedDirectoriesArguments = String.Empty;
                    var removeDirectoriesArguments = String.Empty;

                    foreach (var targetDirectory in solutionDir)
                    {
                        targetDirectoriesArguments += $@" /D:""{targetDirectory}""   ";
                    }

                    foreach (var excludedDirectory in excludedDirectories)
                    {
                        excludedDirectoriesArguments += $@"/XD:""{excludedDirectory}""   ";
                    }

                    foreach (var removeDirectory in removeDirectories)
                    {
                        removeDirectoriesArguments += $@"/RD:""{removeDirectory}""   ";
                    }

                    process.StartInfo.Arguments += targetDirectoriesArguments;
                    process.StartInfo.Arguments += excludedDirectoriesArguments;
                    process.StartInfo.Arguments += removeDirectoriesArguments;

                    Process process1 = process;


                    OutputHelpers.Output(string.Concat("Executing ", processStartInfo.FileName, " \r\n\r\n"), true);

                    process1.OutputDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), false));
                    process1.ErrorDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), false));

                    process1.Exited += new EventHandler((object x, EventArgs y) =>
                    {/*                ToolsCommandPackage.processes.Remove(cmd);                cmd.Checked = false;                */
                        ToolsCommandPackage.dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationBuild);
                    });


                    //process1.Start();
                    process.Start();

                    process1.BeginOutputReadLine();
                    process1.BeginErrorReadLine();
                    // cmd.Checked = true;
                    //ToolsCommandPackage.processes.Add(cmd, process1);

                }
                else
                {
                    string message = string.Format(CultureInfo.CurrentCulture, "Please Open a solution and invoke the command again!", this.GetType().FullName);
                    string title = "No Solution is open in the IDE.";

                    // Show a message box to prove we were here
                    VsShellUtilities.ShowMessageBox(
                        this.ServiceProvider,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }

            }
            catch (Exception exception)
            {
                OutputHelpers.Output(exception.Message, false);
            }

        }

        private void CreatePane(Guid paneGuid, string title,
            bool visible, bool clearWithSolution)
        {
            IVsOutputWindow output =
                (IVsOutputWindow)ServiceProvider.GetService(typeof(SVsOutputWindow));
            IVsOutputWindowPane pane;

            // Create a new pane.
            output.CreatePane(
                ref paneGuid,
                title,
                Convert.ToInt32(visible),
                Convert.ToInt32(clearWithSolution));

            // Retrieve the new pane.
            output.GetPane(ref paneGuid, out pane);

            pane.OutputString("Includes output from the 'Full Clean Command' \n");
        }

    }
}
