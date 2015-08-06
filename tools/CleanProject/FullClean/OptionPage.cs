using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace IcodeNet
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class OptionPage : DialogPage
    {
        [Category("OptionPage")]
        [DisplayName("Directories to clean Option")]
        [Description(@" Directories - full path e.g. 'c:\dev\Reviewer' to clean under.(can specify more than one). By Default it will start from the root of the currently open solutionn.")]
        public string[] Directories { get; set; }


        [Category("OptionPage")]
        [DisplayName("Exclude Directories Option")]
        [Description(" Directories to exclude  from cleanup.")]
        public string[] ExcludeDirectories { get; set; }

        /// <summary>
        ///  File types to exclude (use wildcards) - Remove Matching Files 
        /// </summary>
        public string[] ExcludeFiles { get; set; }


        public bool Help { get; set; }

        /// <summary>
        /// Quiet mode - no prompts
        /// </summary>
        public bool QuietMode { get; set; }

        [Category("OptionPage")]
        [DisplayName("Directories to remove(includes their subdirectories")]
        [Description(@"There is a Fixed list of the folders bin, obj, TestResults, _ReSharper* to remove under the Directories specified with' Directories' Option and we can add more to the array. For Example 'Bower_Components'.
                    Note : For 'node_modules'  I am planning of publishing an update as the node_modules folders are so deeeply nested that we will need to use rimraf for the deletion.
                     I have tested this visx againsta node_modules and it works but the temp dirctory is throwing an error. So in teh mean time please use rimraf as in teh directions below.
                    =======================
                    (rimraf is Node Module). 
                    Due to its folder nesting Windows can’t delete the folder as its name is too long. To solve this, install RimRaf: 

                    npm install rimraf -g

                    and delete the node_modules folder easily with:

                    rimraf node_modules

        ")]
        public string[] RemoveDirectories { get; set; }


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


        [Category("Build")]
        [Description("Stop running any tasks when a (re)build is started. For instance, this can be useful when running 'watch' task from Visual Studio and executing 'npm install' when building.")]
        [DisplayName("Stop processes before building")]
        public bool StopProcessesOnBuild
        {
            get;
            set;
        }


    }
}
