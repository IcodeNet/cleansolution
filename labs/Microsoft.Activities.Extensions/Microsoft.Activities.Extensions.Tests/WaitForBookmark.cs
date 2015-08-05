// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForBookmark.cs" company="">
//   
// </copyright>
// <summary>
//   The wait for bookmark.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    /// The wait for bookmark.
    /// </summary>
    public sealed class WaitForBookmark : NativeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets BookmarkName.
        /// </summary>
        public InArgument<string> BookmarkName { get; set; }

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
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark(
                this.BookmarkName.Get(context), 
                (activityContext, bookmark, value) =>
                activityContext.ResumeBookmark(new Bookmark(this.BookmarkName.Get(activityContext)), null));
        }

        #endregion
    }
}