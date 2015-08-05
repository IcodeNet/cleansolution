// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HRServices.Tests.Activities
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
        #region Static Fields

        /// <summary>
        ///   The default duration for the DelayStub
        /// </summary>
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(1);

        /// <summary>
        ///   The Duration value that will be used for all instances
        /// </summary>
        private static TimeSpan stubDuration = DefaultDuration;

        #endregion

        #region Fields

        /// <summary>
        /// The delay activity
        /// </summary>
        private Delay delay;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the Duration value that will be used for all instances
        /// </summary>
        public static TimeSpan StubDuration
        {
            get
            {
                return stubDuration;
            }

            set
            {
                stubDuration = value;
            }
        }

        /// <summary>
        ///   The duration to delay
        /// </summary>
        /// <remarks>
        ///   Ignored by the DelayStub activity
        /// </remarks>
        [DefaultValue(null)]
        public InArgument<TimeSpan> Duration { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that indicates whether the activity can cause the workflow to become idle.
        /// </summary>
        /// <returns> true if the activity can cause the workflow to become idle. This value is false by default. </returns>
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
        /// Creates and validates a description of the activity’s arguments, variables, child activities, and activity delegates.
        /// </summary>
        /// <param name="metadata">
        /// The activity’s metadata that encapsulates the activity’s arguments, variables, child activities, and activity delegates. 
        /// </param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            this.delay = new Delay { Duration = stubDuration };
            metadata.AddImplementationChild(this.delay);
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">
        /// The execution context in which the activity executes. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            context.ScheduleActivity(this.delay);
        }

        #endregion
    }
}