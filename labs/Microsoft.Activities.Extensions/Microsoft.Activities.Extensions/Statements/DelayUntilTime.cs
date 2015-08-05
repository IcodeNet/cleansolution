// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilTime.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;

    /// <summary>
    ///   The DelayUntilTime activity
    /// </summary>
    /// <remarks>
    ///   An activity that will delay until the next occurance of a time on an allowed day.
    ///   If no days of the week are specified, any day is allowed
    /// </remarks>
    public sealed class DelayUntilTime : NativeActivity
    {
        #region Fields

        /// <summary>
        ///   The delay.
        /// </summary>
        private readonly Delay delay;

        /// <summary>
        ///   The delay interval.
        /// </summary>
        private readonly Variable<TimeSpan> delayInterval = new Variable<TimeSpan>("delayInterval");

        /// <summary>
        ///   The occurence days.
        /// </summary>
        private List<DayOfWeek> occurenceDays;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="DelayUntilTime" /> class.
        /// </summary>
        public DelayUntilTime()
        {
            this.delay = new Delay { Duration = new InArgument<TimeSpan>(this.delayInterval) };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets OccurenceDays
        /// </summary>
        /// <remarks>
        ///   The list of days where you want the event to occur
        /// </remarks>
        public List<DayOfWeek> OccurenceDays
        {
            get
            {
                if (this.occurenceDays == null)
                {
                    this.occurenceDays = new List<DayOfWeek>();
                    foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        this.occurenceDays.Add((DayOfWeek)day);
                    }
                }

                return this.occurenceDays;
            }

            set
            {
                this.occurenceDays = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Time to wait until
        /// </summary>
        [RequiredArgument]
        public InArgument<TimeSpan> Time { get; set; }

        #endregion

        #region Properties

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
            metadata.AddArgument(new RuntimeArgument("Time", typeof(TimeSpan), ArgumentDirection.In, true));
            metadata.AddImplementationChild(this.delay);
            metadata.AddImplementationVariable(this.delayInterval);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            // Get the next occurance of the time on an allowed day
            var allowedDays = this.OccurenceDays;

            var interval = Occurance.Interval(this.Time.Get(context), allowedDays);

            // If the delay is in the future
            if (interval > TimeSpan.Zero)
            {
                this.delayInterval.Set(context, interval);
                context.ScheduleActivity(this.delay);
            }
        }

        #endregion
    }
}