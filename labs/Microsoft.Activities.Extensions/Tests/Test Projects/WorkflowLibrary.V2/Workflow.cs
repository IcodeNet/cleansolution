// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workflow.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WorkflowLibrary
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// The activity compiled.
    /// </summary>
    public partial class Workflow : Activity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Workflow"/> class.
        /// </summary>
        /// <param name="xamlAssemblyResolutionOption">
        /// The xaml assembly resolution option.
        /// </param>
        /// <param name="referencedAssemblies">
        /// The Referenced Assemblies.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public Workflow(XamlAssemblyResolutionOption xamlAssemblyResolutionOption, IList<string> referencedAssemblies)
        {
            referencedAssemblies.Add(Assembly.GetExecutingAssembly().GetName().FullName);

            switch (xamlAssemblyResolutionOption)
            {
                case XamlAssemblyResolutionOption.FullName:
                    StrictXamlHelper.InitializeComponent(this, this.FindResource(), referencedAssemblies);
                    ShowAssemblies();
                    break;
                case XamlAssemblyResolutionOption.VersionIndependent:
                    this.InitializeComponent();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("xamlAssemblyResolutionOption");
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// The show assemblies.
        /// </summary>
        [Conditional("DEBUG")]
        private static void ShowAssemblies()
        {
            Debug.WriteLine("Assemblies in domain " + AppDomain.CurrentDomain.FriendlyName);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.WriteLine(asm.GetName().FullName);
            }
        }

        #endregion
    }
}