// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityWithOptionalArgs.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    /// <summary>
    /// The sample activity.
    /// </summary>
    public sealed class ActivityWithOptionalArgs : CodeActivity<string>
    {
        #region Constants and Fields

        /// <summary>
        ///   The default optional value.
        /// </summary>
        public const int DefaultOptionalValue = 1;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets an optional argument
        /// </summary>
        public InArgument<int> OptionalArg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Context.GetValue
        /// </summary>
        public bool UseContext { get; set; }

        /// <summary>
        ///   Gets or sets a required argument
        /// </summary>
        [RequiredArgument]
        public InArgument<string> RequiredArg { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute method
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// A string with the required and optional arguments
        /// </returns>
        protected override string Execute(CodeActivityContext context)
        {
            var num = this.UseContext ? context.GetValue(this.OptionalArg, DefaultOptionalValue) : this.OptionalArg.Get(context, DefaultOptionalValue);

            return string.Format("{0}: {1}", this.RequiredArg.Get(context), num);
        }

        #endregion
    }
}