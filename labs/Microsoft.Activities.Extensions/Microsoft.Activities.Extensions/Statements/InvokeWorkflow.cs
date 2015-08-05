// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeWorkflow.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Activity that invokes another workflow using WorkflowInvoker
    /// </summary>
    /// <remarks>
    ///   The other workflow is subject to the rules of WorkflowInvoker (no bookmarks). The activity will invoke the other workflow an wait for it to complete before returning. Persistence is not allowed while the other workflow is invoked
    /// </remarks>
    public sealed class InvokeWorkflow : AsyncCodeActivity<IDictionary<string, object>>
    {
        #region Fields

        /// <summary>
        ///   The list tracking.
        /// </summary>
        private readonly ListTrackingParticipant listTracking = new ListTrackingParticipant();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Activity.
        /// </summary>
        /// <remarks>
        ///   The activity that will be invoked. Can be loaded from XAML.
        /// </remarks>
        [RequiredArgument]
        public InArgument<Activity> Activity { get; set; }

        /// <summary>
        ///   Gets or sets Extensions
        /// </summary>
        /// <remarks>
        ///   Use this property to pass extensions to the child workflow
        /// </remarks>
        public InArgument<IEnumerable<object>> Extensions { get; set; }

        /// <summary>
        ///   Gets or sets Inputs.
        /// </summary>
        /// <remarks>
        ///   The input arguments you want to pass to the other workflow
        /// </remarks>
        public InArgument<IDictionary<string, object>> Input { get; set; }

        /// <summary>
        ///   Gets or sets Timeout.
        /// </summary>
        public InArgument<TimeSpan> Timeout { get; set; }

        #endregion

        #region Public Methods and Operators

        #endregion

        #region Methods

        /// <summary>
        /// The begin execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="callback">
        /// The callback. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <returns>
        /// An async task 
        /// </returns>
        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var invoker = new WorkflowInvoker(this.Activity.Get(context));
            invoker.Extensions.Add(this.listTracking);

            var extensions = this.Extensions.Get(context);

            if (extensions != null)
            {
                foreach (var item in extensions)
                {
                    invoker.Extensions.Add(item);
                }
            }

            context.UserState = invoker;
            return this.Timeout.Expression == null
                       ? invoker.BeginInvoke(this.Input.Get(context), callback, state)
                       : invoker.BeginInvoke(this.Input.Get(context), this.Timeout.Get(context), callback, state);
        }

        /// <summary>
        /// The end execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="asyncResult">
        /// The result. 
        /// </param>
        /// <returns>
        /// The output arguments 
        /// </returns>
        protected override IDictionary<string, object> EndExecute(
            AsyncCodeActivityContext context, IAsyncResult asyncResult)
        {
            var invoker = (WorkflowInvoker)context.UserState;

            // Track records stored from the innner workflow
            foreach (var record in this.listTracking.Records)
            {
                context.Track(record);
            }

            return invoker.EndInvoke(asyncResult);
        }

        #endregion
    }
}