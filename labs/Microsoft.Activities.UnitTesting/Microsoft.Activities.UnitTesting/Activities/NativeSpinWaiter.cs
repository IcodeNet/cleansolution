// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeSpinWaiter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;
    using System.Threading;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The spin wait.
    /// </summary>
    public sealed class NativeSpinWaiter : NativeActivity
    {
        #region Fields

        /// <summary>
        /// The is cancellation requested.
        /// </summary>
        private bool isCancellationRequested;

        /// <summary>
        ///   The notify.
        /// </summary>
        private SpinNotify notify;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="NativeSpinWaiter" /> class.
        /// </summary>
        public NativeSpinWaiter()
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
        /// When implemented in a derived class, runs logic to cause graceful early completion of the activity.
        /// </summary>
        /// <param name="context">
        /// The execution context in which the activity executes. 
        /// </param>
        protected override void Cancel(NativeActivityContext context)
        {
            WorkflowTrace.Verbose("NativeSpinWaiter Cancel Requested");
            this.isCancellationRequested = true;
            base.Cancel(context);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            this.notify = context.GetExtension<SpinNotify>();
            
            for (var i = 0; i < this.Loops; i++)
            {
                WorkflowTrace.Verbose(
                    "NativeSpinWaiter loop {0} of {1}, spinning {2} iterations", i, this.Loops, this.Iterations);

                var token = context.GetExtension<ActivityCancellationToken>();
                token.ThrowIfCancellationRequested();


                if (this.isCancellationRequested)
                {
                    return;
                }

                Thread.SpinWait(this.Iterations);

                this.notify.LoopComplete(this.Loops, this.Iterations);
            }

            WorkflowTrace.Verbose("NativeSpinWaiter done with {0} iterations", this.Iterations * this.Loops);
        }

        #endregion
    }
}