//------------------------------------------------------------------------------
// <copyright file="FullCleanCommand.cs" company="KSS">
//     Copyright (c) KSS.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using CleanProject;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace FullClean
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

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
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
        public static FullCleanCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
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
            CreatePane(new Guid(), "Full Clean Command", true, false);

            Process proc = new Process();
            proc.StartInfo.FileName = typeof(CleanOptions).Assembly.Location;


            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            if (!String.IsNullOrEmpty(dte.Solution.FullName))
            {

                var props = dte.Properties["KSS Options", "KSS Clean Solution Options"];
                // EnvDTE.Properties props = dte.Properties["TextEditor", "CSharp"];


                string[] directories = (string[])props.Item("Directories").Value;
                string[] excludedDirectories = (string[])props.Item("ExcludeDirectories").Value;

                string[] solutionDir =  directories.Any()? directories.ToArray() : new string[]{};

                if (!solutionDir.Any())
                {
                    solutionDir = new[] { System.IO.Path.GetDirectoryName(dte.Solution.FullName) };
                }

                proc.StartInfo.Arguments = String.Format(@"/v /D:""{0}""  ", String.Join(" ", solutionDir));

                var excludedDirectoriesArguments = String.Empty;

                foreach (var excludedDirectory in excludedDirectories)
                {
                    excludedDirectoriesArguments += String.Format(@"/XD:""{0}""   ", excludedDirectory);
                }

                proc.StartInfo.Arguments += excludedDirectoriesArguments;

                proc.Start();


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

        void CreatePane(Guid paneGuid, string title,
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
