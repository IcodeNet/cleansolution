// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingProfileExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.Tracking;
    using System.Diagnostics.Contracts;
    using System.Text;

    using Microsoft.Activities.Extensions.Diagnostics;

    using DiagTrace = System.Diagnostics.Trace;

    /// <summary>
    ///   The tracking profile extensions.
    /// </summary>
    public static class TrackingProfileExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="trackingProfile">
        /// The tracking profile.
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An argument was null
        /// </exception>
        public static string ToFormattedString(this TrackingProfile trackingProfile, int tabs = 0)
        {
            Contract.Requires(trackingProfile != null);
            if (trackingProfile == null)
            {
                throw new ArgumentNullException("trackingProfile");
            }

            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendTitle("Tracking profile: {0}", trackingProfile.Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("ActivityDefinitionId : {0}", trackingProfile.ActivityDefinitionId);
                tsb.AppendLine("ImplementationVisibility : {0}", trackingProfile.ImplementationVisibility);
                tsb.AppendCollection("Queries", trackingProfile.Queries, (trackingQuery, t) => trackingQuery.ToFormattedString(t));
            }

            var formattedString = tsb.ToString();
            return formattedString;
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="trackingProfile">
        /// The tracking profile. 
        /// </param>
        public static void Trace(this TrackingProfile trackingProfile)
        {
            Contract.Requires(trackingProfile != null);
            if (trackingProfile == null)
            {
                throw new ArgumentNullException("trackingProfile");
            }

            WorkflowTrace.Information(ToFormattedString(trackingProfile));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The close brace.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="tab">
        /// The tab.
        /// </param>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        private static int CloseBrace(StringBuilder sb, int tab)
        {
            sb.AppendLine("}");
            tab--;
            return tab;
        }

        /// <summary>
        /// The open brace.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="tab">
        /// The tab.
        /// </param>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        private static int OpenBrace(StringBuilder sb, int tab)
        {
            sb.AppendLine("{");
            tab++;
            return tab;
        }

        #endregion
    }
}