// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForTrigger.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Windows.Activities
{
    using System.Activities;

    using Microsoft.Activities.Extensions;

    /// <summary>
    ///   An activity that creates a bookmark named for the trigger
    /// </summary>
    public sealed class WaitForTrigger : NativeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets BookmarkName.
        /// </summary>
        public StateTrigger Trigger { get; set; }

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
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">
        /// The execution context in which the activity executes. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark(this.Trigger);
        }

        #endregion
    }
}