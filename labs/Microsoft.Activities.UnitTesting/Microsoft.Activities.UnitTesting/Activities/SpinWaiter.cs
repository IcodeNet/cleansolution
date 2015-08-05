// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpinWaiter.cs" company="Microsoft">
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
    /// The spin wait.
    /// </summary>
    public sealed class SpinWaiter : CodeActivity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpinWaiter"/> class.
        /// </summary>
        public SpinWaiter()
        {
            this.Loops = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the iterations.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Gets or sets the number of loops
        /// </summary>
        public int Loops { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            var token = context.GetExtension<ActivityCancellationToken>();
            var notify = context.GetExtension<SpinNotify>();
            for (int i = 0; i < this.Loops; i++)
            {
                WorkflowTrace.Verbose(
                    "SpinWaiter loop {0} of {1}, spinning {2} iterations", i, this.Loops, this.Iterations);

                // Don't do this from a Code or Native Activity
                // The activity will be faulted
                // token.ThrowIfCancellationRequested();
                if (token != null && token.IsCancellationRequested(context))
                {
                    return;
                }

                Thread.SpinWait(this.Iterations);
                if (notify != null)
                {
                    notify.LoopComplete(this.Loops, this.Iterations);
                }
            }

            WorkflowTrace.Verbose("SpinWaiter done with {0} iterations", this.Iterations * this.Loops);
        }

        #endregion
    }
}