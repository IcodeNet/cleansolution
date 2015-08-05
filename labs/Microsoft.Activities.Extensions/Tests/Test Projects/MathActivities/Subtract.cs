// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subtract.cs" company="">
//   
// </copyright>
// <summary>
//   The subtract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MathActivities
{
    using System.Activities;

    /// <summary>
    /// The subtract.
    /// </summary>
    public sealed class Subtract : CodeActivity<int>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Op1.
        /// </summary>
        public InArgument<int> Op1 { get; set; }

        /// <summary>
        /// Gets or sets Op2.
        /// </summary>
        public InArgument<int> Op2 { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The execute.
        /// </returns>
        protected override int Execute(CodeActivityContext context)
        {
            return this.Op1.Get(context) - this.Op2.Get(context);
        }

        #endregion
    }
}