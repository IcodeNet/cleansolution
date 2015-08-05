namespace UsingTypedTracking
{
    using System;
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    /// A sample TypedTrackingParticipant
    /// </summary>
    internal class WriteLineTracker : TypedTrackingParticipant
    {
        #region Methods

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">The generated tracking record.</param><param name="timeout">The time period after which the provider aborts the attempt.</param>
        protected override void Track(ActivityScheduledRecord record, TimeSpan timeout)
        {
            Console.WriteLine("Activity Scheduled {0}", record.Activity != null ? record.Activity.Name : "<Null>");           
        }

        #endregion
    }
}