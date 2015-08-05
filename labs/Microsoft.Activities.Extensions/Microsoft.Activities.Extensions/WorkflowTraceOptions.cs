// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceOptions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;

    /// <summary>
    ///   Options for tracing
    /// </summary>
    [Flags]
    public enum WorkflowTraceOptions
    {
        /// <summary>
        ///   Default options
        /// </summary>
        Default = 0x0, 

        /// <summary>
        ///   Empty collections will be traced
        /// </summary>
        ShowEmptyCollections = 0x01, 

        /// <summary>
        ///   Shows the count of the collection
        /// </summary>
        ShowCollectionCount = 0x02, 

        /// <summary>
        /// Provides additional tracing which may induce a performance penalty
        /// </summary>
        /// <remarks>
        /// Use this option when troubleshooting
        /// </remarks>
        Debug = 0x03,
    }
}