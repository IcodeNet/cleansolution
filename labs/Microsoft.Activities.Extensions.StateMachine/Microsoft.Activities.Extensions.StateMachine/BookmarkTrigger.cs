// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookmarkTrigger.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities;

    /// <summary>
    /// An activity which creates a bookmark with the given name
    /// </summary>
    public class BookmarkTrigger : NativeActivity<object>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets BookmarkName.
        /// </summary>
        public string BookmarkName { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether CanInduceIdle.
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
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">
        /// The execution context in which the activity executes. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            if (string.IsNullOrWhiteSpace(this.BookmarkName))
            {
                throw new InvalidOperationException("Bookmark name is null or empty");
            }

            context.CreateBookmark(this.BookmarkName, this.BookmarkCallback);
        }

        /// <summary>
        /// Callback from the bookmark
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="bookmark">
        /// The bookmark.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void BookmarkCallback(NativeActivityContext context, Bookmark bookmark, object value)
        {
            // TODO: How can we pass the bookmark value safely to a trigger condition?
            this.Result.Set(context, value);
        }

        #endregion
    }
}