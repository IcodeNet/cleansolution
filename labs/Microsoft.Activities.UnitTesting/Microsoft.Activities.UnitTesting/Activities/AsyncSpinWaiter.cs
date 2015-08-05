// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncSpinWaiter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System;
    using System.Activities;
    using System.Threading;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The spin wait.
    /// </summary>
    public sealed class AsyncSpinWaiter : AsyncCodeActivity
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncSpinWaiter" /> class.
        /// </summary>
        public AsyncSpinWaiter()
        {
            this.Loops = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the iterations.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        ///   Gets or sets the number of loops
        /// </summary>
        public int Loops { get; set; }

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
        /// The System.IAsyncResult. 
        /// </returns>
        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var token = context.GetExtension<ActivityCancellationToken>();
         
            var notify = context.GetExtension<SpinNotify>();

            Action<ActivityCancellationToken, SpinNotify> action = this.DoSpinWait;
            context.UserState = action;
            return action.BeginInvoke(token, notify, callback, state);
        }

        /// <summary>
        /// The end execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            var token = context.GetExtension<ActivityCancellationToken>();

            if (token.IsCancellationRequested(context))
            {
                context.MarkCanceled();
            }

            var action = (Action<ActivityCancellationToken, SpinNotify>)context.UserState;

            WorkflowTrace.Verbose(
                "AsyncSpinWaiter.EndExecute IsCancellationRequested {0}", context.IsCancellationRequested);
            action.EndInvoke(result);
        }

        /// <summary>
        /// Executes the spin wait
        /// </summary>
        /// <param name="token">
        /// The token. 
        /// </param>
        /// <param name="notify">
        /// The notify. 
        /// </param>
        private void DoSpinWait(ActivityCancellationToken token, SpinNotify notify)
        {
            for (var i = 0; i < this.Loops; i++)
            {
                WorkflowTrace.Verbose(
                    "AsyncSpinWaiter loop {0} of {1}, spinning {2} iterations", i, this.Loops, this.Iterations);

                try
                {
                    // For the token cancel
                    if (token.IsCancellationRequested())
                    {
                        WorkflowTrace.Verbose("Token requests cancel");
                        return;
                    }

                    Thread.SpinWait(this.Iterations);
                }
                finally
                {
                    if (notify != null)
                    {
                        notify.LoopComplete(this.Loops, this.Iterations);
                    }   
                }
            }

            WorkflowTrace.Verbose("AsyncSpinWaiter done with {0} iterations", this.Iterations * this.Loops);
        }

        #endregion
    }
}