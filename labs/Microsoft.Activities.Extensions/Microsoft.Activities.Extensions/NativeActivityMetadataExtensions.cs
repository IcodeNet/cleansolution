// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeActivityMetadataExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System.Activities;

    /// <summary>
    /// Extensions to the NatvieActivityMetadata class
    /// </summary>
    public static class NativeActivityMetadataExtensions
    {
        #region Public Methods

        /// <summary>
        /// Adds and binds and argument in one step
        /// </summary>
        /// <param name="metadata">
        /// The activity metadata
        /// </param>
        /// <param name="binding">
        /// The argument to bind
        /// </param>
        /// <param name="argument">
        /// The runtime argument
        /// </param>
        public static void AddAndBindArgument(
            this NativeActivityMetadata metadata, Argument binding, RuntimeArgument argument)
        {
            metadata.Bind(binding, argument);
            metadata.AddArgument(argument);
        }

        #endregion
    }
}