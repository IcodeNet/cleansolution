// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTimeoutActivity.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The test timeout activity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;

    /// <summary>
    /// The test timeout activity.
    /// </summary>
    public sealed class TestTimeoutActivity : Activity
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestTimeoutActivity" /> class.
        /// </summary>
        public TestTimeoutActivity()
        {
            this.Implementation =
                () =>
                new Sequence
                    {
                       Activities = { new Delay { Duration = new InArgument<TimeSpan>(ctx => this.DelayTime.Get(ctx)) } } 
                    };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets DelayTime.
        /// </summary>
        public InArgument<TimeSpan> DelayTime { get; set; }

        #endregion
    }
}