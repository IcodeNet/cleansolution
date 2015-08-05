// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowXamlFileType.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    /// <summary>
    ///   Types of XAML artifacts for Workflow
    /// </summary>
    public enum WorkflowXamlFileType
    {
        /// <summary>
        ///   The XAML file is unknown
        /// </summary>
        Unknown, 

        /// <summary>
        ///   The XAML file is an activity
        /// </summary>
        Activity, 

        /// <summary>
        ///   The XAML file is a WorkflowService
        /// </summary>
        WorkflowService
    }
}