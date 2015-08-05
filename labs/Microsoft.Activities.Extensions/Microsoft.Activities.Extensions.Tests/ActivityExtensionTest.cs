// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityExtensionTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    ///   The activity extension test.
    /// </summary>
    public class ActivityExtensionTest : CodeActivity<bool>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to add an extension provider.
        /// </summary>
        public bool AddExtensionProvider { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The cache metadata.
        /// </summary>
        /// <param name="metadata">
        /// The metadata. 
        /// </param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (this.AddExtensionProvider)
            {
                metadata.AddDefaultExtensionProvider(() => new TestExtension());
            }

            metadata.RequireExtension<TestExtension>();
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override bool Execute(CodeActivityContext context)
        {
            var extension = context.GetExtension<TestExtension>();
            extension.InvokeCount++;
            return true;
        }

        #endregion
    }
}