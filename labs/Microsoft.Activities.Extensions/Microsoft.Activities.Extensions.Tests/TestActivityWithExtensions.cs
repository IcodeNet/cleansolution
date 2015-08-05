// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestActivityWithExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Activities.Statements;

    /// <summary>
    /// The test activity with extensions.
    /// </summary>
    public sealed class TestActivityWithExtensions : Activity
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TestActivityWithExtensions" /> class.
        /// </summary>
        public TestActivityWithExtensions()
        {
            this.Implementation =
                () =>
                new Sequence
                    {
                        Activities =
                            {
                                new IncrementExtensionStore
                                    {
                                       InitialValue = new InArgument<int>(ctx => this.Num.Get(ctx)) 
                                    }, 
                                new DecrementExtensionStore
                                    {
                                       InitialValue = new InArgument<int>(ctx => this.Num.Get(ctx)) 
                                    }, 
                            }
                    };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Num.
        /// </summary>
        public InArgument<int> Num { get; set; }

        #endregion
    }
}