//------------------------------------------------------------------------------
// <copyright file="FullCleanCommandPackage.cs" company="KSS">
//     Copyright (c) KSS.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace FullClean
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
    [Guid(FullCleanCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(CleanSolutionOptions),"KSS Options", "KSS Clean Solution Options",0, 0, true)]
    public sealed class FullCleanCommandPackage : Package
    {
        /// <summary>
        /// FullCleanCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3f20b2b1-69d5-4e30-9426-82900a1b80d5";

        /// <summary>
        /// Initializes a new instance of the <see cref="FullCleanCommand"/> class.
        /// </summary>
        public FullCleanCommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
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

            var dialogPage = GetDialogPage(typeof(CleanSolutionOptions)) as CleanSolutionOptions;

            var dialogDirectories = new List<string>(); 
            var dialogExcludedDirectories = new List<string>(); 

            if (dialogPage != null)
            {
                DTE dte = (DTE)GetService(typeof(DTE));

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

        #endregion
    }
}
