// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockWriteLine.cs" company="">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests.MockActivities
{
    using System.Activities;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// Mock WriteLine activity
    /// </summary>
    public class MockWriteLine : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        [DefaultValue("")]
        public InArgument<string> Text { get; set; }

        /// <summary>
        /// Gets or sets TextWriter.
        /// </summary>
        [DefaultValue("")]
        public InArgument<TextWriter> TextWriter { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            // Do nothing
        }
    }
}