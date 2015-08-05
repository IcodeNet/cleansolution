// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticXamlTestWorker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Activities.UnitTesting;

    using WorkflowLibrary;

    /// <summary>
    /// The static xaml helper test.
    /// </summary>
    [Serializable]
    public class StaticXamlTestWorker : MarshalByRefObject
    {
        #region Public Methods

        /// <summary>
        /// Given loose XAML and a reference to ActivityLibrary (V2) ActivityLoad should load V2
        /// </summary>
        public void WhenLooseXamlRefV2ShouldLoadV2WhenFound()
        {
            var host =
                new WorkflowInvokerTest(
                    StrictXamlHelper.ActivityLoad(GetWorkflowXaml(), GetListWithActivityLibraryVersion(2)));
            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("AssemblyVersion", new Version(2, 0, 0, 0));
        }

        /// <summary>
        /// Given 
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V1
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * Workflow should load and return version 1.0.0.0
        /// </summary>
        public void WorkflowV1RefActivityV1DeployedActivityV1ShouldLoad()
        {
            var activity = new Workflow(XamlAssemblyResolutionOption.FullName, GetListWithActivityLibraryVersion(1));
            var host = new WorkflowInvokerTest(activity);
            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("AssemblyVersion", new Version(1, 0, 0, 0));
        }

        /// <summary>
        /// Given 
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V1 (Unsigned)
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * FileLoadException should be thrown
        /// </summary>
        public void WorkflowV1RefActivityV1DeployedActivityV1UnsignedShouldThrow()
        {
            AssertHelper.Throws<FileLoadException>(
                () => new Workflow(XamlAssemblyResolutionOption.FullName, GetListWithActivityLibraryVersion(1)));
        }

        /// <summary>
        /// Given 
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V2
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * FileLoadException is thrown
        /// </summary>
        public void WorkflowV1RefActivityV1DeployedActivityV2ShouldThrow()
        {
            AssertHelper.Throws<FileLoadException>(
                () => new Workflow(XamlAssemblyResolutionOption.FullName, GetListWithActivityLibraryVersion(1)));
        }

        /// <summary>
        /// Given
        ///   * Workflow.xaml deployed
        ///   * Activity V1 (signed) deployed
        ///   When
        ///   * ActivityLoad with ref to ActivityLibrary V1 (signed)
        ///   Then
        ///   * the workflow should return V1
        /// </summary>
        public void WorkflowXamlRefActivityV1DeployedActivityV1ShouldLoad()
        {
            var host =
                new WorkflowInvokerTest(
                    StrictXamlHelper.ActivityLoad(GetWorkflowXaml(), GetListWithActivityLibraryVersion(1)));
            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("AssemblyVersion", new Version(1, 0, 0, 0));
        }

        /// <summary>
        /// Given 
        ///   * Workflow.xaml deployed
        ///   * ActivityLibrary (V2) deployed
        ///   When 
        ///   * ActivityLoad with reference to ActivityLibrary V1
        ///   Then 
        ///   * should throw FileLoadException because cannot load ActivityLibrary V1
        /// </summary>
        public void WorkflowXamlRefActivityV1DeployedActivityV2ShouldThrow()
        {
            AssertHelper.Throws<FileLoadException>(
                () => StrictXamlHelper.ActivityLoad(GetWorkflowXaml(), GetListWithActivityLibraryVersion(1)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a referenced assembly list with a specific version for ActivityLibrary
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="signed">
        /// The use Token.
        /// </param>
        /// <returns>
        /// The referenced assembly list
        /// </returns>
        private static List<string> GetListWithActivityLibraryVersion(int version, bool signed = true)
        {
            var list = new List<string> 
            {
                    string.Format(
                        "ActivityLibrary, Version={0}.0.0.0, Culture=neutral, PublicKeyToken={1}", 
                        version, 
                        signed ? "824701d2ab05d638" : "null")
                };
            list.AddRange(StrictXamlHelper.StandardCSharpReferencedAssemblies);
            return list;
        }

        /// <summary>
        /// The get workflow xaml.
        /// </summary>
        /// <returns>
        /// The path to the workflow XAML
        /// </returns>
        private static string GetWorkflowXaml()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Workflow.xaml");
        }

        #endregion
    }
}