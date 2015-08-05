// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Diagnostics.Contracts;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The exception ex.
    /// </summary>
    public static class ExceptionEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns a formatted string
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="tabs">
        /// The tabs.
        /// </param>
        /// <returns>
        /// A string with the formatted message
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The exception is null
        /// </exception>
        public static string ToFormattedString(this Exception exception, int tabs = 0)
        {
            Contract.Requires(exception != null);
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var traceableException = exception as ITraceable;

            if (traceableException != null)
            {
                return traceableException.ToFormattedString(tabs);
            }

            return new TraceStringBuilder(tabs, exception.ToString()).ToString();
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="exception">
        /// The exception. 
        /// </param>
        public static void Trace(this Exception exception)
        {
            WorkflowTrace.Information(exception.ToFormattedString());
        }

        #endregion
    }
}