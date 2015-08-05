// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   The list extensions.
    /// </summary>
    public static class ListExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts a list of ActivityStateRecords to a comma delimited string with activity names
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <returns>
        /// The delimited list. 
        /// </returns>
        public static string ToDelimitedList(this IEnumerable<ActivityStateRecord> records)
        {
            var result = new StringBuilder();

            foreach (var record in records)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }

                result.Append(record.Activity.Name);
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts a list of CustomTrackingRecord to a comma delimited string with activity names
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <returns>
        /// The delimited list. 
        /// </returns>
        public static string ToDelimitedStateList(this IEnumerable<CustomTrackingRecord> records)
        {
            var result = new TraceStringBuilder();

            foreach (var record in records)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }

                result.AppendFormat("{0}", record.Data["currentstate"]);
            }

            return result.ToString();
        }

        #endregion
    }
}