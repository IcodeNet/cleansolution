using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace FullClean
{
    [CLSCompliant(false), ComVisible(true)]
    public class CleanSolutionOptions : DialogPage
    {
        [Category("CleanSolutionOptions")]
        [DisplayName("Directories to clean Option")]
        [Description(" Directories to clean (can specify more than one). By Default it will start from the root of the currently open solutionn.")]
        public string[] Directories { get; set; }


        [Category("CleanSolutionOptions")]
        [DisplayName("Exclude Directories Option")]
        [Description(" Directories to exclude  from cleanup.")]
        public string[] ExcludeDirectories { get; set; }

        /// <summary>
        ///  File types to exclude (use wildcards) - Remove Matching Files 
        /// </summary>
        public List<string> ExcludeFiles { get; set; }


        public bool Help { get; set; }

        /// <summary>
        /// Quiet mode - no prompts
        /// </summary>
        public bool QuietMode { get; set; }

        /// <summary>
        ///  Directories to remove (includes subdirectories) 
        /// </summary>
        public List<string> RemoveDirectories { get; set; }


        /// <summary>
        ///  "File types to remove (use wildcards) 
        /// </summary>
        public List<string> RemoveFiles { get; set; }

        /// <summary>
        ///   Removes source control bindings 
        /// </summary>
        public bool RemoveSourceControl { get; set; }

        /// <summary>
        ///    Displays lots of messages
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        ///   Windows Mode - Displays an output window 
        /// </summary>
        public bool WindowsMode { get; set; }

        /// <summary>
        ///     Zip file directory 
        /// </summary>
        public string ZipDirectory { get; set; }

        /// <summary>
        ///    Copy clean and zip the project 
        /// </summary>
        public bool ZipProject { get; set; }

    }
}
