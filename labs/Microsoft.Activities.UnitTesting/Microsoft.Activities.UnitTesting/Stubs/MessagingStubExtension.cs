// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingStubExtension.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Linq;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// The receive stub implementation.
    /// </summary>
    public class MessagingStubExtension : TrackingParticipant, IMessagingStub
    {
        #region Constants and Fields

        /// <summary>
        ///   The implementations.
        /// </summary>
        private readonly Dictionary<string, Action> implementations = new Dictionary<string, Action>();

        /// <summary>
        ///   The received messages.
        /// </summary>
        private readonly IList<StubMessage> messages = new List<StubMessage>();

        /// <summary>
        ///   The queue.
        /// </summary>
        private readonly Queue<StubMessage> queue = new Queue<StubMessage>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the received messages.
        /// </summary>
        /// <returns>
        ///   Returns messages received
        /// </returns>
        public IList<StubMessage> Messages
        {
            get
            {
                lock (this.messages)
                {
                    return new ReadOnlyCollection<StubMessage>(this.messages);
                }
            }
        }

        /// <summary>
        ///   Gets or sets IdleHandler.
        /// </summary>
        public Action<MessagingStubExtension> OnIdle { get; set; }

        /// <summary>
        ///   Gets QueueCount.
        /// </summary>
        public int QueueCount
        {
            get
            {
                lock (this.queue)
                {
                    return this.queue.Count;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets The instance proxy.
        /// </summary>
        protected WorkflowInstanceProxy InstanceProxy { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueue a message to be delivered
        /// </summary>
        /// <param name="contract">
        /// The contract.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        public void EnqueueReceive(XName contract, string operation, object messageContent)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(new StubMessage(StubMessageType.Receive) { Contract = contract.ToString(), Content = messageContent, Operation = operation });
            }
        }

        /// <summary>
        /// Enqueue a message to be delivered
        /// </summary>
        /// <param name="contract">
        /// The contract.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        public void EnqueueReceive(XName contract, string operation, IDictionary<string, object> parametersContent)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(new StubMessage(StubMessageType.Receive) { Contract = contract.ToString(), Content = parametersContent, Operation = operation });
            }
        }

        /// <summary>
        /// Enqueue a reply to a send
        /// </summary>
        /// <param name="fromContract">
        /// The from contract.
        /// </param>
        /// <param name="fromOperation">
        /// The from operation.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        public void EnqueueReceiveReply(XName fromContract, string fromOperation, object messageContent)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(
                    new StubMessage(StubMessageType.ReceiveReply) { Contract = fromContract.ToString(), Content = messageContent, Operation = fromOperation });
            }
        }

        /// <summary>
        /// Enqueue a reply to a send
        /// </summary>
        /// <param name="fromContract">
        /// The from contract.
        /// </param>
        /// <param name="fromOperation">
        /// The from operation.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        public void EnqueueReceiveReply(XName fromContract, string fromOperation, IDictionary<string, object> parametersContent)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(
                    new StubMessage(StubMessageType.ReceiveReply) { Contract = fromContract.ToString(), Content = parametersContent, Operation = fromOperation });
            }
        }

        /// <summary>
        /// When implemented, returns any additional extensions the implementing class requires.
        /// </summary>
        /// <returns>
        /// A collection of additional workflow extensions.
        /// </returns>
        public IEnumerable<object> GetAdditionalExtensions()
        {
            return null;
        }

        /// <summary>
        /// The get implementation.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// The implementation action
        /// </returns>
        public virtual Action GetImplementation(Activity activity)
        {
            Action action;
            this.implementations.TryGetValue(activity.DisplayName, out action);
            return action;
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stubMessage">
        /// The stub message.
        /// </param>
        public void Send(StubMessage stubMessage)
        {
            lock (this.messages)
            {
                this.messages.Add(ReflectionClone.DeepCopy(stubMessage));
            }
        }

        /// <summary>
        /// The set implementation.
        /// </summary>
        /// <param name="activityName">
        /// The activity name.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void SetImplementation(string activityName, Action action)
        {
            this.implementations[activityName] = action;
        }

        /// <summary>
        /// Sets the specified target <see cref="T:System.Activities.Hosting.WorkflowInstanceProxy"/>.
        /// </summary>
        /// <param name="instance">
        /// The target workflow instance to set.
        /// </param>
        public void SetInstance(WorkflowInstanceProxy instance)
        {
            this.InstanceProxy = instance;
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="preserveQueue">
        /// The preserve Queue.
        /// </param>
        public void Trace(bool preserveQueue = false)
        {
            lock (this.queue)
            {
                WorkflowTrace.Information("Messages in Queue: {0}", this.queue.Count);
                if (this.queue.Count > 0)
                {
                    System.Diagnostics.Trace.Indent();
                    if (preserveQueue)
                    {
                        WorkflowTrace.Information("Queue will be preserved, only first message is shown");
                        WorkflowTrace.Information("[0] {0}", this.queue.Peek());
                    }
                    else
                    {
                        int index = 0;
                        while (this.queue.Count > 0)
                        {
                            WorkflowTrace.Information(
                                "[{0}] {1}", index++, this.queue.Dequeue());
                        }
                    }

                    System.Diagnostics.Trace.Unindent();
                }
            }

            WorkflowTrace.Information("Messages Sent/Received: {0}", this.Messages.Count);
            System.Diagnostics.Trace.Indent();
            for (int index = 0; index < this.Messages.Count; index++)
            {
                WorkflowTrace.Information("[{0}] {1}", index, this.Messages[index]);
            }

            System.Diagnostics.Trace.Unindent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The track.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if (record is WorkflowInstanceRecord)
            {
                var wir = record as WorkflowInstanceRecord;
                if (wir.State == "Idle")
                {
                    this.OnWorkflowIdle();
                }
            }
        }

        /// <summary>
        /// The on workflow idle.
        /// </summary>
        /// <exception cref="BookmarkResumptionException">
        /// Unable to resume the bookmark
        /// </exception>
        private void OnWorkflowIdle()
        {
            if (this.OnIdle != null)
            {
                this.OnIdle(this);
            }

            if (this.queue.Count == 0)
            {
                return;
            }

            var message = this.queue.Peek();

            this.InstanceProxy.BeginResumeBookmark(
                new Bookmark(message.BookmarkName), 
                message.Content, 
                a =>
                    {
                        var result = this.InstanceProxy.EndResumeBookmark(a);
                        switch (result)
                        {
                            case BookmarkResumptionResult.Success:
                                lock (this.messages)
                                {
                                    this.queue.Dequeue();
                                    this.messages.Add(ReflectionClone.DeepCopy(message));
                                }

                                break;
                            case BookmarkResumptionResult.NotFound:
                                WorkflowTrace.Information(
                                    string.Format("Warning: bookmark {0} was not found - it may not be set yet", message.BookmarkName));

                                // Leave the message on the queue for the next idle event
                                break;
                            default:
                                WorkflowTrace.Information("Unable to resume bookmark {0} result is {1}", message.BookmarkName, result);
                                throw new BookmarkResumptionException(message.BookmarkName, result);
                        }
                    }, 
                null);
        }

        #endregion
    }
}