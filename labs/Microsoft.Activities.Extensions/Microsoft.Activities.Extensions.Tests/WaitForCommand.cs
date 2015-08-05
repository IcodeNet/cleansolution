// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    ///   The wait for command.
    /// </summary>
    internal class WaitForCommand : NativeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ActivityOptions Command { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether can induce idle.
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
            context.CreateBookmark(this.Command.ToString(), (activityContext, bookmark, value) => { });
        }

        #endregion
    }
}