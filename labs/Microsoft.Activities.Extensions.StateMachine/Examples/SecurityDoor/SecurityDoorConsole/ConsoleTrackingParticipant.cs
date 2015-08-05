// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTrackingParticipant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoorConsole
{
    using System;
    using System.Activities.Tracking;

    using CmdLine;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    /// A Tracking Participant that outputs tracking information to the console
    /// </summary>
    internal class ConsoleTrackingParticipant : TrackingParticipant
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            // CommandLine.WriteLineColor(ConsoleColor.Green, record.ToFormattedString());
            Console.WriteLine(record.ToFormattedString());
        }

        #endregion
    }
}