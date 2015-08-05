// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeIncrement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DynamicLoad
{
    using System.Activities;
    using System.ComponentModel;

    /// <summary>
    /// The code increment.
    /// </summary>
    public sealed class CodeIncrement : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Num.
        /// </summary>
        [DefaultValue(null)]
        public InOutArgument<int> Num { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            this.Num.Set(context, this.Num.Get(context) + 1);
        }

        #endregion
    }
}