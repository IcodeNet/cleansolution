// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWorkflowInstanceStoreScript.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.DurableInstancing
{
    /// <summary>
    ///   The type of script
    /// </summary>
    public enum SqlWorkflowInstanceStoreScript
    {
        /// <summary>
        ///   The logic script
        /// </summary>
        Logic, 

        /// <summary>
        ///   The schema script
        /// </summary>
        Schema
    }
}