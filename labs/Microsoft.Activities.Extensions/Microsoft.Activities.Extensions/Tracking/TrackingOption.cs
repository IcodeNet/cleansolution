// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingOption.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;

    /// <summary>
    ///   Determines which elements of the tracking record will be included in the trace
    /// </summary>
    /// <remarks>
    ///   The Default value will apply the TrackingOptions.Default value.  To set
    ///   the option for all traces set this value.
    /// </remarks>
    [Flags]
    public enum TrackingOption
    {
        /// <summary>
        ///   Traces the instance ID
        /// </summary>
        InstanceId = 0x1, 

        /// <summary>
        ///   Traces the variables
        /// </summary>
        Variables = 0x2, 

        /// <summary>
        ///   Traces the arguments
        /// </summary>
        Arguments = 0x4, 

        /// <summary>
        ///   Traces the annotations
        /// </summary>
        Annotations = 0x8, 

        /// <summary>
        ///   The default tracing options
        /// </summary>
        Default = Variables | Arguments | Annotations | RecordNumber, 

        /// <summary>
        ///   Traces the data of custom tracking records
        /// </summary>
        Data = 0x20, 

        /// <summary>
        ///   Traces the time
        /// </summary>
        Time = 0x40, 

        /// <summary>
        ///   Traces Record Number
        /// </summary>
        RecordNumber = 0x80, 

        /// <summary>
        ///   The type of the activity
        /// </summary>
        TypeName = 0x100,

        /// <summary>
        ///   No options are selected
        /// </summary>
        None = 0x200,

        /// <summary>
        ///   Combines all tracking options
        /// </summary>
        All = Time | InstanceId | Variables | Arguments | Annotations | Data | TypeName,

    }
}