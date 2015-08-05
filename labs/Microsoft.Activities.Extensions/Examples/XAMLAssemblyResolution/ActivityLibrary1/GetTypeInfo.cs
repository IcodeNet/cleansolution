// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTypeInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ActivityLibrary1
{
    using System.Activities;

    /// <summary>
    /// The get type info.
    /// </summary>
    public sealed class GetTypeInfo : CodeActivity<string>
    {
        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// Version information
        /// </returns>
        protected override string Execute(CodeActivityContext context)
        {
            return string.Format("GetTypeInfo Activity version {0}\r\n{1}", this.GetType().Assembly.GetName().Version, this.GetType().Assembly.Location);
        }

        #endregion
    }
}