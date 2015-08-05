// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBookmark.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;

    /// <summary>
    /// Provides a means to create bookmarks in test activities
    /// </summary>
    /// <typeparam name="T">
    /// The type of data associated with the bookmark 
    /// </typeparam>
    public sealed class TestBookmark<T> : NativeActivity<T>
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the name of the bookmark
        /// </summary>
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

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
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark(this.BookmarkName.Get(context), this.OnBookmarkResume);
        }

        /// <summary>
        /// The bookmark callback
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="bookmark">
        /// The bookmark. 
        /// </param>
        /// <param name="data">
        /// The data for the bookmark 
        /// </param>
        private void OnBookmarkResume(NativeActivityContext context, Bookmark bookmark, object data)
        {
            if (data != null)
            {
                this.Result.Set(context, (T)data);
            }
        }

        #endregion
    }
}