//------------------------------------------------------------------------------
// <copyright file="FullCleanCommand.cs" company="ICodeNet">
//     Copyright (c) ICodeNet.  All rights reserved.
// </copyright>

//  Commands are functions that accomplish tasks, such as printing a document, refreshing a view, or creating a new file.
//  When you extend Visual Studio, you can create commands and register them with the Visual Studio shell.
//  You can specify how these commands will appear in the IDE, for example,
//  on a menu or toolbar.Typically a custom command appears on the Tools menu, 
//  and a command for displaying a tool window would appear on the Other Windows submenu of the View menu.

//  When you create a command, you must also create an event handler for it.
//  The event handler determines when the command is visible or enabled, 
//  lets you modify its text, and guarantees that the command responds appropriately when it is activated.

//  In most instances,
//  the IDE handles commands by using the IOleCommandTarget interface. 
//  Commands in Visual Studio are handled starting with the innermost command context,
//  based on the local selection, and proceeding to the outermost context,
//  based on the global selection.

//  Commands added to the main menu are immediately available for scripting.
//  ------------------------------------------------------------------------------

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

            // Process process = new Process();
            // process.StartInfo.FileName = typeof(CleanOptions).Assembly.Location;


            ToolsCommandPackage.dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral);
            try
            {
                if (!String.IsNullOrEmpty(ToolsCommandPackage.dte.Solution.FullName))
                {

                    var props = ToolsCommandPackage.dte.Properties["KSS Tools", "Full Clean Solution"];

                    string[] directories = (string[])props.Item("Directories").Value;
                    string[] excludedDirectories = (string[])props.Item("ExcludeDirectories").Value;
                    string[] removeDirectories = (string[])props.Item("RemoveDirectories").Value;
                    bool quietMode = (bool)props.Item("QuietMode").Value;
                    bool verbose = (bool)props.Item("Verbose").Value;
                    bool windowsMode = (bool)props.Item("WindowsMode").Value;

                    string[] solutionDir = directories.Any() ? directories.ToArray() : new string[] { };

                    if (!solutionDir.Any())
                    {
                        solutionDir = new[] { System.IO.Path.GetDirectoryName(ToolsCommandPackage.dte.Solution.FullName) };

                    }

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

                    // declare the process
                    Process process = new Process();

                    if (verbose)
                    {
                        process.StartInfo.Arguments = @" /V ";
                    }

                    if (windowsMode)
                    {
                        process.StartInfo.Arguments += @" /W ";
                    }


                    if (quietMode)
                    {
                        //https://stackoverflow.com/questions/1700695/getting-output-from-one-executable-in-an-other-one/1700708#1700708
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

                        ProcessStartInfo processStartInfo1 = processStartInfo;



                        process.StartInfo = processStartInfo1;
                        process.EnableRaisingEvents = true;

                        process.StartInfo.Arguments += @" /Q";
                    }
                    else
                    {
                        process.StartInfo.FileName = typeof(CleanOptions).Assembly.Location;
                    }



                    process.StartInfo.Arguments += targetDirectoriesArguments;
                    process.StartInfo.Arguments += excludedDirectoriesArguments;
                    process.StartInfo.Arguments += removeDirectoriesArguments;

                    OutputHelpers.Output(string.Concat("Executing ", process.StartInfo.FileName, " \r\n\r\n"), true);

                    process.OutputDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), false));
                    process.ErrorDataReceived += new DataReceivedEventHandler((object sendingProcess, DataReceivedEventArgs outLine) => OutputHelpers.Output(string.Concat(outLine.Data, "\r\n"), true));

                    process.Exited += new EventHandler((object x, EventArgs y) =>
                    {
                        try
                        {
                            ToolsCommandPackage.dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationGeneral);

                        }
                        catch (Exception)
                        {
                            // silent
                        }
                    });

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

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
