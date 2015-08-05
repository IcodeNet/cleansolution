// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CleanProject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    using CmdLine;

    /// <summary>
    /// Program to clean solutions
    /// </summary>
    /// <remarks>
    /// Setup Visual Studio with an external tool Title: Clean, Remove Source Bindings and Zip Solution Command: CleanProject.exe Arguments: /D:$(SolutionDir) /Z /R Use Output Window
    /// </remarks>
    internal class Program
    {
        #region Constants and Fields

        /// <summary>
        /// The default remove dirs.
        /// </summary>
       // private static readonly string[] DefaultRemoveDirs = { "bin", "obj", "TestResults", "_ReSharper*", "sql", "logs", "packages" };
        private static readonly string[] DefaultRemoveDirs = { "bin", "obj", "TestResults", "_ReSharper*" };

        /// <summary>
        /// The default remove files.
        /// </summary>
        private static readonly string[] DefaultRemoveFiles = { "*.ReSharper*", "*.suo" };

        /// <summary>
        /// The win string builder.
        /// </summary>
        private static readonly StringBuilder WinStringBuilder = new StringBuilder();

        /// <summary>
        /// The console window.
        /// </summary>
        private static IntPtr consoleWindow;

        /// <summary>
        /// The win text out.
        /// </summary>
        private static TextWriter winTextOut;

        #endregion

        #region Public Properties

        /// <summary>
        /// The options.
        /// </summary>
        public static CleanOptions Options { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="sClassName">
        /// The s class name. 
        /// </param>
        /// <param name="sAppName">
        /// The s app name. 
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string sClassName, string sAppName);

        #endregion

        #region Methods

        /// <summary>
        /// The write verbose message.
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        internal static void WriteVerboseMessage(string format, params object[] args)
        {
            if (Options.Verbose)
            {
                var msg = string.Format(format, args);
                Trace.WriteLine(msg);
                Console.WriteLine(msg);
            }
        }

        /// <summary>
        /// The confirm options.
        /// </summary>
        private static void ConfirmOptions()
        {
            if (Options.ZipProject)
            {
                Options.QuietMode = true;
                WriteVerboseMessage("Will copy to a temporary directory, clean and zip the project");
            }

            if (Options.RemoveSourceControl && Options.Verbose)
            {
                WriteVerboseMessage("Will remove source control bindings from projects");
            }

            if (!Options.QuietMode)
            {
                Console.WriteLine("Will clean the following directories");
                foreach (var directory in Options.Directories)
                {
                    CommandLine.WriteLineColor(ConsoleColor.Yellow, directory);
                }

                if (Options.WindowsMode)
                {
                    var sb = new StringBuilder(WinStringBuilder.ToString());
                    sb.AppendLine();
                    sb.Append("This will delete files, do you want to continue?");
                    if (MessageBox.Show(
                        sb.ToString(),
                        CleanOptions.Title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2,
                        (MessageBoxOptions)0x40000) == DialogResult.No)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        WinStringBuilder.Clear();
                    }
                }
                else if (Console.In == null || CommandLine.IsInputRedirected)
                {
                    CommandLine.WriteLineColor(ConsoleColor.Yellow, "Input is redirected, command will delete files, processing is aborted");
                    Environment.Exit(0);
                }
                else if (CommandLine.PromptKey("This will delete files, do you want to continue?", 'y', 'n') == 'n')
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// The copy solution to temp dir.
        /// </summary>
        /// <param name="directory">
        /// The directory. 
        /// </param>
        /// <returns>
        /// A solution info 
        /// </returns>
        private static SolutionInfo CopySolutionToTempDir(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new ApplicationException(string.Format("Directory \"{0}\" does not exist", directory));
            }

            if (!Options.WindowsMode)
            {
                Console.WriteLine("Copying solution {0} to temporary directory", directory);
            }

            var solutionInfo = new SolutionInfo { Directory = GetLongDirectoryName(directory) };

            DirectoryHelper.CopyDirectory(directory, solutionInfo.TempPath, true, true);

            return solutionInfo;
        }

        /// <summary>
        /// The enable windows mode.
        /// </summary>
        private static void EnableWindowsMode()
        {
            consoleWindow = FindWindow(null, CleanOptions.Title);
            if (consoleWindow != IntPtr.Zero)
            {
                // Hide the console Window
                ShowWindow(consoleWindow, 0);
            }

            winTextOut = new StringWriter(WinStringBuilder);
            Console.SetOut(winTextOut);
        }

        /// <summary>
        /// The get directories.
        /// </summary>
        /// <returns>
        /// </returns>
        private static List<SolutionInfo> GetDirectories()
        {
            return
                Options.Directories.Select(
                    directory => new SolutionInfo { Directory = GetLongDirectoryName(directory) }).ToList();
        }

        /// <summary>
        /// The get long directory name.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        /// <returns>
        /// The get long directory name.
        /// </returns>
        private static string GetLongDirectoryName(string directory)
        {
            var sb = new StringBuilder(255);
            GetLongPathName(directory, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// The get long path name.
        /// </summary>
        /// <param name="path">
        /// The path. 
        /// </param>
        /// <param name="pszPath">
        /// The psz path. 
        /// </param>
        /// <param name="cchPath">
        /// The cch path. 
        /// </param>
        /// <returns>
        /// The get long path name. 
        /// </returns>
        [DllImport("kernel32.dll")]
        private static extern int GetLongPathName(string path, StringBuilder pszPath, int cchPath);

        /// <summary>
        /// The get temp directories.
        /// </summary>
        /// <returns>
        /// </returns>
        private static List<SolutionInfo> GetTempDirectories()
        {
            return Options.Directories.Select(CopySolutionToTempDir).ToList();
        }

        /// <summary>
        /// The main.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ParseCommandLine();
            Console.Title = CleanOptions.Title;

            if (Options.WindowsMode)
            {
                // Debug registry command line
                // Clipboard.SetText(CommandLine.Text);
                // MessageBox.Show(CommandLine.Text);
                EnableWindowsMode();
            }

            Console.WriteLine(
                "{0} {1} - {2}",
                CleanOptions.Title,
                Assembly.GetEntryAssembly().GetName().Version.ToString(3),
                CleanOptions.Description);

            if (Options.Directories.Count == 0)
            {
                Options.Directories.Add(Directory.GetCurrentDirectory());
            }

            foreach (var dir in DefaultRemoveDirs.Where(dir => !Options.RemoveDirectories.Contains(dir)))
            {
                Options.RemoveDirectories.Add(dir);
            }

            // Find the list of directories to be excluded from processing
            DirectoryHelper.FindExcludedDirectories();

            // Find the directories that will be removed when cleaning
            DirectoryHelper.FindRemoveDirectories();

            foreach (var file in DefaultRemoveFiles.Where(file => !Options.RemoveFiles.Contains(file)))
            {
                Options.RemoveFiles.Add(file);
            }

            FileHelper.FindExcludedFiles();

            ConfirmOptions();

            try
            {
                var directories = Options.ZipProject ? GetTempDirectories() : GetDirectories();

                foreach (var solutionInfo in directories)
                {
                    Console.WriteLine("Cleaning Solution Directory {0}", solutionInfo.WorkingPath);
                }

                SolutionCleaner.CleanDirectories(directories);

                if (Options.ZipProject)
                {
                    ZipHelper.ZipDirectories(directories);
                }
            }
            catch (ApplicationException exception)
            {
                if (Options.WindowsMode)
                {
                    MessageBox.Show(
                        exception.Message,
                        CleanOptions.Title,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        (MessageBoxOptions)0x40000);
                }
                else
                {
                    CommandLine.WriteLineColor(ConsoleColor.Red, "{0}", exception.Message);
                }

                Environment.Exit(1);
            }

            ShowCleaningComplete();
        }

        /// <summary>
        /// The parse command line.
        /// </summary>
        private static void ParseCommandLine()
        {
            try
            {
                Options = CommandLine.Parse<CleanOptions>();
            }
            catch (CommandLineHelpException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
                Environment.Exit(1);
            }
            catch (CommandLineArgumentInvalidException exception)
            {
                CommandLine.WriteLineColor(ConsoleColor.Red, exception.ArgumentHelp.Message);
                CommandLine.WriteLineColor(ConsoleColor.Cyan, exception.ArgumentHelp.GetHelpText(Console.BufferWidth));

                CommandLine.Pause();
                Environment.Exit(1);
            }
            catch (Exception exception)
            {
                CommandLine.WriteLineColor(ConsoleColor.Red, exception.Message);
                CommandLine.Pause();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// The show cleaning complete.
        /// </summary>
        private static void ShowCleaningComplete()
        {
            Console.WriteLine("Cleaning complete");

            if (Options.WindowsMode)
            {
                MessageBox.Show(
                    WinStringBuilder.ToString(), CleanOptions.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (!Options.QuietMode)
                {
                    CommandLine.Pause();
                }
            }
        }

        /// <summary>
        /// The show window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd. 
        /// </param>
        /// <param name="nCmdShow">
        /// The n cmd show. 
        /// </param>
        /// <returns>
        /// The show window. 
        /// </returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion
    }
}