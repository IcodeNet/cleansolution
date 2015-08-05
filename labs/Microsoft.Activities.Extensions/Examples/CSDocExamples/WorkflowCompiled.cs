// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowCompiled.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

using Microsoft.Activities.Extensions;

/// <summary>
/// The WorkflowCompiled class
/// </summary>
// ReSharper disable CheckNamespace
public partial class WorkflowCompiled 
    // ReSharper restore CheckNamespace
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowCompiled"/> class.
    /// </summary>
    /// <param name="assemblyResolutionOption">
    /// The assembly resolution option.
    /// </param>
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
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets ReferencedAssemblies.
    /// </summary>
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