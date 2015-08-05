// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowCompiled.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XAMLAssemblyResolution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Activities.Extensions;

    /// <summary>
    ///   Provides an alternate constructor to support strict CLR type loading
    /// </summary>
    /// <remarks>
    ///   The default constructor generated for XAML workflows supports version independent loading of referenced types
    ///   If you do not want this behavior you can create a partial class like this one and call the 
    ///   ctor with the XamlAssemblyResolutionOption you want to use
    /// </remarks>
    public partial class WorkflowCompiled
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCompiled"/> class.
        /// </summary>
        /// <param name="assemblyResolutionOption">
        /// The assembly Resolution Option. 
        /// </param>
        /// <remarks>
        /// Provides a constructor overload that allows you to specify a <see cref="XamlAssemblyResolutionOption"/>
        /// </remarks>
        public WorkflowCompiled(XamlAssemblyResolutionOption assemblyResolutionOption)
        {
            switch (assemblyResolutionOption)
            {
                case XamlAssemblyResolutionOption.VersionIndependent:
                    this.InitializeComponent();
                    break;
                case XamlAssemblyResolutionOption.FullName:
                    StrictXamlHelper.InitializeComponent(this, this.FindResource(), ReferencedAssemblies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("assemblyResolutionOption");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the referenced assemblies.
        /// </summary>
        /// <remarks>
        ///   The XamlAppDef build task generates a list of referenced assemblies automatically in the (XamlName).g.cs file 
        ///   You can find the list of assemblies and version that need to be referenced there.
        ///   This property returns a simple string list of the assemblies that will cause them to be loaded using the 
        ///   standard Assembly.Load method
        /// </remarks>
        public static IList<string> ReferencedAssemblies
        {
            get
            {
                // Create a list of activities you want to reference here
                // You must add the currently executing assembly
                var list = new List<string>
                    {
                        "ActivityLibrary1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c18b97d2d48a43ab", 
                        Assembly.GetExecutingAssembly().GetName().FullName
                    };

                // Add the standard list of references
                list.AddRange(StrictXamlHelper.StandardCSharpReferencedAssemblies);
                return list;
            }
        }

        #endregion
    }
}