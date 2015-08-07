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
    public class CleanSolutionOptionPage : DialogPage
    {
        [Category("CleanSolutionOptionPage")]
       
        [DisplayName("Directories to clean Option")]
        [Description(@" Directories - full path e.g. 'c:\dev\Reviewer' to clean under.(can specify more than one). By Default it will start from the root of the currently open solutionn.")]
        public string[] Directories { get; set; }


        [Category("CleanSolutionOptionPage")]
        [DisplayName("Exclude Directories Option")]
        [Description(" Directories to exclude  from cleanup.")]
        public string[] ExcludeDirectories { get; set; }


        [Category("CleanSolutionOptionPage")]
        [DisplayName("Quiet mode")]
        [Description(@"  Quiet mode - no prompts. It will start the CleanProject exe in the background and redirect output to Viusal Studio output.")]
        public bool QuietMode { get; set; }

        [Category("CleanSolutionOptionPage")]
        [DisplayName("Directories to remove(includes their subdirectories")]
        [Description(@"There is a Fixed list of the folders bin, obj, TestResults, to remove under the Directories specified with' Directories' Option and we can add more to the array. For Example 'Bower_Components'.
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

        [Category("CleanSolutionOptionPage")]
        [DisplayName("Verbose")]
        [Description(@"  Displays lots of messages ")]
        public bool Verbose { get; set; }


        #region FUTURE Functionality


        [Category("ToDo in the future")]
        [DisplayName("Excluded Files")]
        [Description(@" File types to exclude (use wildcards) - Remove Matching Files .")]
        public string[] ExcludeFiles { get; set; }

        [Category("ToDo in the future")]
        [Description(@" Not Used.")]
        public bool Help { get; set; }

        [Category("ToDo in the future")]
        [DisplayName("RemoveFiles")]
        [Description(@"  File types to remove (use wildcards) ")]
        public List<string> RemoveFiles { get; set; }


        [Category("ToDo in the future")]
        [DisplayName("RemoveSourceControl")]
        [Description(@"   Removes source control bindings  ")]
        public bool RemoveSourceControl { get; set; }


        [Category("ToDo in the future")]
        [DisplayName("WindowsMode")]
        [Description(@"  Windows Mode - Displays an output window .")]
        public bool WindowsMode { get; set; }


        [Category("ToDo in the future")]
        [DisplayName("ZipDirectory")]
        [Description(@" Zip file directory.")]
        public string ZipDirectory { get; set; }

        [Category("ToDo in the future")]
        [DisplayName("ZipProject")]
        [Description(@" Copy clean and zip the project .")]
        public bool ZipProject { get; set; }

        [Category("Build")]
        [Description("Stop running any tasks when a (re)build is started. For instance, this can be useful when running 'watch' task from Visual Studio and executing 'npm install' when building.")]
        [DisplayName("Stop processes before building")]
        public bool StopProcessesOnBuild
        {
            get;
            set;
        }


        #endregion

    }
}
