// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskSpinWaiter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The spin wait.
    /// </summary>
    public sealed class TaskNativeSpinWaiter : TaskNativeActivity
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskNativeSpinWaiter" /> class.
        /// </summary>
        public TaskNativeSpinWaiter()
        {
            this.Loops = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the iterations.
        /// </summary>
        public InArgument<int> Iterations { get; set; }

        /// <summary>
        ///   Gets or sets the number of loops
        /// </summary>
        public InArgument<int> Loops { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The do spin.
        /// </summary>
        /// <param name="loops">
        /// The loops.
        /// </param>
        /// <param name="iterations">
        /// The iterations.
        /// </param>
        /// <param name="notify">
        /// The notify.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        public void DoSpin(int loops, int iterations, SpinNotify notify, CancellationToken token)
        {
            for (var i = 0; i < loops; i++)
            {
                WorkflowTrace.Verbose("TaskSpinWaiter loop {0} of {1}", i, loops);

                try
                {
                    if (token.IsCancellationRequested)
                    {
                        WorkflowTrace.Verbose("TaskSpinWaiter Cancel Requested");
                        return;
                    }

                    Task.Run(
                        () =>
                        {
                            WorkflowTrace.Verbose("SpinWait({0})", iterations);
                            Thread.SpinWait(iterations);
                            if (token.IsCancellationRequested)
                            {
                                WorkflowTrace.Verbose("SpinWait - Cancellation is requested");
                            }
                        },
                        token).Wait();
                }
                finally
                {
                    notify.LoopComplete(loops, iterations);
                }
            }

            WorkflowTrace.Verbose("TaskSpinWaiter done with {0} iterations", iterations * loops);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cache metadata.
        /// </summary>
        /// <param name="metadata">
        /// The metadata. 
        /// </param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<SpinNotify>();
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="token">
        /// The token. 
        /// </param>
        protected override void Execute(NativeActivityContext context, CancellationToken token)
        {
            var loops = this.Loops.Get(context);
            var iterations = this.Iterations.Get(context);
            var notify = context.GetExtension<SpinNotify>();

            using (new NoPersistZone(context))
            {
                Task.Run(() => this.DoSpin(loops, iterations, notify, token), token).Wait();
            }
        }

        #endregion
    }
}