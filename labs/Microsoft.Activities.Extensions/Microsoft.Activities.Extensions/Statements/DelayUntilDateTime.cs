// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilDateTime.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;
    using System.Activities.Statements;

    /// <summary>
    /// The DelayUntilDateTime activity will delay until the specified date and time if it is in the future.
    /// </summary>
    public sealed class DelayUntilDateTime : NativeActivity
    {
        #region Constants and Fields

        /// <summary>
        ///   The delay activity
        /// </summary>
        private readonly Delay delay;

        /// <summary>
        ///   The delay interval.
        /// </summary>
        private readonly Variable<TimeSpan> delayInterval = new Variable<TimeSpan>("delayInterval");

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelayUntilDateTime" /> class.
        /// </summary>
        public DelayUntilDateTime()
        {
            this.delay = new Delay { Duration = new InArgument<TimeSpan>(this.delayInterval) };
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the specific date and time to wait until
        /// </summary>
        /// <remarks>
        /// If the date and time is in the past, the activity will not delay
        /// </remarks>
        [RequiredArgument]
        public InArgument<DateTime> UntilDate { get; set; }

        /// <summary>
        ///   Gets a value indicating whether CanInduceIdle.
        /// </summary>
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
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
            metadata.AddArgument(new RuntimeArgument("UntilDate", typeof(DateTime), ArgumentDirection.In, true));
            metadata.AddImplementationChild(this.delay);
            metadata.AddImplementationVariable(this.delayInterval);
        }

        /// <summary>
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            this.delayInterval.Set(context, this.UntilDate.Get(context).Subtract(DateTime.Now));

            if (this.delayInterval.Get(context) > TimeSpan.Zero)
            {
                context.ScheduleActivity(this.delay);
            }
        }

        #endregion
    }
}