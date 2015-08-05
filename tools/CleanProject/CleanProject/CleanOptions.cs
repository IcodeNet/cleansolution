namespace CleanProject
{
    using System.Collections.Generic;

    using CmdLine;

    /// <summary>
    ///   Options for the CleanProject command
    /// </summary>
    [CommandLineArguments(Program = "CleanProject", Title = Title, Description = Description)]
    public class CleanOptions
    {
        #region Constants and Fields

        public const string Description = "Cleans binaries, test results and other debris from your project";

        public const string Title = "Clean Project";

        #endregion

        #region Constructors and Destructors

        public CleanOptions()
        {
            this.Directories = new List<string>();
            this.RemoveDirectories = new List<string>();
            this.RemoveFiles = new List<string>();
        }

        #endregion

        #region Public Properties

        [CommandLineParameter(Command = "D", Description = "Directory to clean (can specify more than one)", Name = "Directory")]
        public List<string> Directories { get; set; }

        [CommandLineParameter(Command = "XD", Description = "Directories to exclude (includes subdirectories)", Name = "Exclude Matching Directories")]
        public List<string> ExcludeDirectories { get; set; }

        [CommandLineParameter(Command = "XF", Description = "File types to exclude (use wildcards)", Name = "Remove Matching Files")]
        public List<string> ExcludeFiles { get; set; }

        [CommandLineParameter(Command = "?", Default = false, Description = "Show Help", Name = "Help", IsHelp = true)]
        public bool Help { get; set; }

        [CommandLineParameter(Command = "Q", Default = false, Description = "Quiet mode - no prompts", Name = "Quiet Mode")]
        public bool QuietMode { get; set; }

        [CommandLineParameter(Command = "RD", Description = "Directories to remove (includes subdirectories)", Name = "Remove Matching Directories")]
        public List<string> RemoveDirectories { get; set; }

        [CommandLineParameter(Command = "RF", Description = "File types to remove (use wildcards)", Name = "Remove Matching Files")]
        public List<string> RemoveFiles { get; set; }

        [CommandLineParameter(Command = "R", Default = false, Description = "Removes source control bindings", Name = "Source Control")]
        public bool RemoveSourceControl { get; set; }

        [CommandLineParameter(Command = "V", Default = false, Description = "Displays lots of messages", Name = "Verbose")]
        public bool Verbose { get; set; }

        [CommandLineParameter(Command = "W", Default = false, Description = "Windows Mode - Displays an output window", Name = "Windows Mode")]
        public bool WindowsMode { get; set; }

        [CommandLineParameter(Command = "ZD", Description = "Zip file directory", Name = "ZipDirectory")]
        public string ZipDirectory { get; set; }

        [CommandLineParameter(Command = "Z", Default = false, Description = "Copy clean and zip the project", Name = "Zip")]
        public bool ZipProject { get; set; }

        #endregion
    }
}