// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///   Global readonly static values
    /// </summary>
    internal static class Global
    {
        #region Static Fields

        /// <summary>
        ///   The default timeout used when a debugger is attached
        /// </summary>
        private static TimeSpan defaultDebugTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        ///   The default timeout used
        /// </summary>
        private static TimeSpan defaultTimeout = TimeSpan.FromSeconds(1);

        #endregion

        #region Properties

        /// <summary>
        ///  Gets or sets the default timeout used when a debugger is attached
        /// </summary>
        /// <remarks>
        /// Allows users of this library to set the default
        /// </remarks>
        internal static TimeSpan DefaultDebugTimeout
        {
            get
            {
                return defaultDebugTimeout;
            }

            set
            {
                defaultDebugTimeout = value;
            }
        }

        /// <summary>
        ///  Gets or sets the default timeout
        /// </summary>
        /// <remarks>
        /// Allows users of this library to set the default
        /// </remarks>
        internal static TimeSpan DefaultTimeout
        {
            get
            {
                return defaultTimeout;
            }

            set
            {
                defaultTimeout = value;
            }
        }

        /// <summary>
        ///   Gets Timeout used for wait operations
        /// </summary>
        /// <remarks>
        ///   Notice how the Timeout adjust when the debugger is attached
        /// </remarks>
        internal static TimeSpan Timeout
        {
            get
            {
                return Debugger.IsAttached ? defaultDebugTimeout : defaultTimeout;
            }
        }

        #endregion
    }
}