// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncrementExtensionStore.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    /// The increment extension store.
    /// </summary>
    public sealed class IncrementExtensionStore : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets InitialValue.
        /// </summary>
        public InArgument<int> InitialValue { get; set; }

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
            metadata.RequireExtension<IncrementStore>();
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            var store = context.GetExtension<IncrementStore>();
            store.Value = this.InitialValue.Get(context) + 1;
        }

        #endregion
    }
}