// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityWithOptionalStringArg.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    /// An activity with an optional string arg
    /// </summary>
    public class ActivityWithOptionalStringArg : CodeActivity<string>
    {
        #region Constants

        /// <summary>
        /// The default option value
        /// </summary>
        public const string DefaultOptionalValue = "Test";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets OptionalArg.
        /// </summary>
        public InArgument<string> OptionalArg { get; set; }

        /// <summary>
        /// Gets or sets RequiredArg.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> RequiredArg { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override string Execute(CodeActivityContext context)
        {
            var text = this.OptionalArg.Get(context, DefaultOptionalValue);

            return string.Format("{0}: {1}", this.RequiredArg.Get(context), text);
        }

        #endregion
    }
}