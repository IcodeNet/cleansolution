// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedTrackingParticipant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.Tracking;
    using System.ServiceModel.Activities.Tracking;

#if NET401_OR_GREATER
    using System.Activities.Statements.Tracking;
#endif

    /// <summary>
    ///   A tracking particpant that includes typed overloads of the Track method
    /// </summary>
    public class TypedTrackingParticipant : TrackingParticipant
    {
        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(ActivityStateRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(ActivityScheduledRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(BookmarkResumptionRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(CancelRequestedRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

#if NET401_OR_GREATER

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(StateMachineStateRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

#endif

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(ReceiveMessageRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(SendMessageRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(CustomTrackingRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(FaultPropagationRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(WorkflowInstanceAbortedRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(WorkflowInstanceSuspendedRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(WorkflowInstanceTerminatedRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(WorkflowInstanceUnhandledExceptionRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected virtual void Track(WorkflowInstanceRecord record, TimeSpan timeout)
        {
            // Do nothing
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if (record is ActivityStateRecord)
            {
                this.Track((ActivityStateRecord)record, timeout);
            }
            else if (record is ActivityScheduledRecord)
            {
                this.Track((ActivityScheduledRecord)record, timeout);
            }
            else if (record is BookmarkResumptionRecord)
            {
                this.Track((BookmarkResumptionRecord)record, timeout);
            }
            else if (record is CancelRequestedRecord)
            {
                this.Track((CancelRequestedRecord)record, timeout);
            }

#if NET401_OR_GREATER
            else if (record is StateMachineStateRecord)
            {
                this.Track((StateMachineStateRecord)record, timeout);
            }

#endif
            else if (record is ReceiveMessageRecord)
            {
                this.Track((ReceiveMessageRecord)record, timeout);
            }
            else if (record is SendMessageRecord)
            {
                this.Track((SendMessageRecord)record, timeout);
            }
            else if (record is CustomTrackingRecord)
            {
                this.Track((CustomTrackingRecord)record, timeout);
            }
            else if (record is FaultPropagationRecord)
            {
                this.Track((FaultPropagationRecord)record, timeout);
            }
            else if (record is WorkflowInstanceAbortedRecord)
            {
                this.Track((WorkflowInstanceAbortedRecord)record, timeout);
            }
            else if (record is WorkflowInstanceSuspendedRecord)
            {
                this.Track((WorkflowInstanceSuspendedRecord)record, timeout);
            }
            else if (record is WorkflowInstanceTerminatedRecord)
            {
                this.Track((WorkflowInstanceTerminatedRecord)record, timeout);
            }
            else if (record is WorkflowInstanceUnhandledExceptionRecord)
            {
                this.Track((WorkflowInstanceUnhandledExceptionRecord)record, timeout);
            }
            else if (record is WorkflowInstanceRecord)
            {
                this.Track((WorkflowInstanceRecord)record, timeout);
            }
            else
            {
                throw new NotImplementedException(string.Format("There is no track handler for type {0}", record.GetType().Name));
            }
        }
    }
}