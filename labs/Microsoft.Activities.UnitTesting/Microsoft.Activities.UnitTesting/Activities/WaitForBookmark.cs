// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForBookmark.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;

    /// <summary>
    /// An activity that creates a bookmark named for the trigger
    /// </summary>
    /// <typeparam name="T">
    /// The type of the bookmark name
    /// </typeparam>
    public sealed class WaitForBookmark<T> : NativeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets BookmarkName.
        /// </summary>
        public T BookmarkName { get; set; }

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
            context.CreateBookmark(this.BookmarkName.ToString());
        }

        #endregion
    }
}