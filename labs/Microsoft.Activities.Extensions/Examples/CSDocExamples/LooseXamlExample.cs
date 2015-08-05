// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LooseXamlExample.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xaml;

using Microsoft.Activities.Extensions;

/// <summary>
/// Loose XAML Example
/// </summary>
// ReSharper disable CheckNamespace
public class LooseXamlExample
// ReSharper restore CheckNamespace
{
    #region Public Methods and Operators

    /// <summary>
    /// Returns a list of assemblies
    /// </summary>
    /// <returns>
    /// The list of assemblies
    /// </returns>
    public static IList<string> GetWorkflowLooseReferencedAssemblies()
    {
        // Create a list of activities you want to reference here
        var list = new List<string>
            {
                "ActivityLibrary1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c18b97d2d48a43ab", 
                Assembly.GetExecutingAssembly().GetName().FullName
            };

        // Add the standard list of references
        list.AddRange(StrictXamlHelper.StandardCSharpReferencedAssemblies);
        return list;
    }

    /// <summary>
    /// Invokes the loose xaml
    /// </summary>
    /// <param name="resolutionOption">
    /// The resolution option.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The option is unknown
    /// </exception>
    public static void InvokeLooseXaml(XamlAssemblyResolutionOption resolutionOption)
    {
        Console.WriteLine();
        Console.WriteLine("Loose XAML");
        try
        {
            switch (resolutionOption)
            {
                case XamlAssemblyResolutionOption.VersionIndependent:
                    WorkflowInvoker.Invoke(ActivityXamlServices.Load("WorkflowLoose.xaml"));
                    break;
                case XamlAssemblyResolutionOption.FullName:

                    // This will ensure the correct assemblies are loaded prior to loading loose XAML
                    WorkflowInvoker.Invoke(
                        StrictXamlHelper.ActivityLoad("WorkflowLoose.xaml", GetWorkflowLooseReferencedAssemblies()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("resolutionOption");
            }
        }
        catch (XamlObjectWriterException ex)
        {
            Console.WriteLine("Error loading loose xaml {0}", ex.Message);
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            Console.WriteLine("Could not find assembly {0}", fileNotFoundException.FileName);
        }
        catch (FileLoadException fileLoadException)
        {
            Console.WriteLine("Could not load assembly {0}", fileLoadException.FileName);
        }
    }

    #endregion
}