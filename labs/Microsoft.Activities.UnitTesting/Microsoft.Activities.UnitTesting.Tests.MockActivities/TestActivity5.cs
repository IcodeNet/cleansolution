// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestActivity5.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests.MockActivities
{
    using System.Activities;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
    public sealed class TestActivity5 : CodeActivity<int>
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The execute.
        /// </returns>
        protected override int Execute(CodeActivityContext context)
        {
            Trace.WriteLine(this.GetType());

            return 5;
        }

        #endregion
    }
}