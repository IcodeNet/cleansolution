// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestInnerActivity.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Expressions;
    using System.Activities.Statements;

    /// <summary>
    /// The test inner activity.
    /// </summary>
    public sealed class TestInnerActivity : Activity
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestInnerActivity" /> class.
        /// </summary>
        public TestInnerActivity()
        {
            this.Result = new OutArgument<int>();

            this.Implementation =
                () =>
                new Sequence
                    {
                        Activities =
                            {
                                new If(new InArgument<bool>(ctx => this.Num.Get(ctx) < 0))
                                    {
                                        Then =
                                            new Throw { Exception = new InArgument<Exception>(ctx => new ArgumentException()) }, 
                                        Else =
                                            new Assign<int> 
                                            {
                                                    To = new ArgumentReference<int> { ArgumentName = "Result" }, 
                                                    Value = new InArgument<int>(ctx => this.Num.Get(ctx) + 1)
                                            }
                                    }
                            }
                    };
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Num.
        /// </summary>
        public InArgument<int> Num { get; set; }

        /// <summary>
        ///   Gets or sets Result.
        /// </summary>
        public OutArgument<int> Result { get; set; }

        #endregion
    }
}