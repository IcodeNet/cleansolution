// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Globals.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrackingStateMachine.Tests
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The globals.
    /// </summary>
    internal static class Globals
    {
        #region Static Fields

        /// <summary>
        /// The timeout.
        /// </summary>
        public static readonly TimeSpan Timeout = Debugger.IsAttached
                                                      ? TimeSpan.FromMinutes(2)
                                                      : TimeSpan.FromSeconds(2);

        #endregion
    }
}