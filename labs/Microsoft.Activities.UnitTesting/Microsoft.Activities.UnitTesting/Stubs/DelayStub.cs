// -----------------------------------------------------------------------
// <copyright file="DelayStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.ComponentModel;

    /// <summary>
    ///   A stubbed version of the Delay activity
    /// </summary>
    public class DelayStub : NativeActivity
    {
        #region Constants and Fields

        /// <summary>
        ///   The default duration for the DelayStub
        /// </summary>
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(1);

        /// <summary>
        ///   The Duration value that will be used for all instances
        /// </summary>
        public static TimeSpan StubDuration = DefaultDuration;


        /// <summary>
        /// The duration to delay
        /// </summary>
        /// <remarks>
        /// Ignored by the DelayStub activity
        /// </remarks>
        [DefaultValue(null)]
        public InArgument<TimeSpan> Duration
        {
            get;
            set;
        } 

        private Delay delay;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that indicates whether the activity can cause the workflow to become idle.
        /// </summary>
        /// <returns>
        ///   true if the activity can cause the workflow to become idle. This value is false by default.
        /// </returns>
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
        ///   Creates and validates a description of the activity’s arguments, variables, child activities, and activity delegates.
        /// </summary>
        /// <param name = "metadata">The activity’s metadata that encapsulates the activity’s arguments, variables, child activities, and activity delegates.</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            this.delay = new Delay { Duration = StubDuration };
            metadata.AddImplementationChild(this.delay);
            base.CacheMetadata(metadata);
        }

        /// <summary>
        ///   When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name = "context">The execution context in which the activity executes.</param>
        protected override void Execute(NativeActivityContext context)
        {
            context.ScheduleActivity(this.delay);
        }

        #endregion
    }
}