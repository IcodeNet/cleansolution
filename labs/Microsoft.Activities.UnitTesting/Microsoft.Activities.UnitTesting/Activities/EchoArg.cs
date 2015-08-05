// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EchoArg.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An activity with an InArgument "Value" that is assigned to the out argument "Result"
    /// </summary>
    /// <typeparam name="T">
    /// The type of the arguments 
    /// </typeparam>
    public class EchoArg<T> : CodeActivity<T>
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Value.
        /// </summary>
        public InArgument<T> Value { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute method
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <returns>
        /// The input value 
        /// </returns>
        protected override T Execute(CodeActivityContext context)
        {
            var execute = this.Value.Get(context);
            return execute;
        }

        #endregion
    }

    /// <summary>
    /// An activity that echos argument values
    /// </summary>
    /// <typeparam name="T1">
    /// The type of value 1 
    /// </typeparam>
    /// <typeparam name="T2">
    /// The type of value 2 
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Reviewed. Suppression is OK here.")]
    public class EchoArg<T1, T2> : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the result 1.
        /// </summary>
        public OutArgument<T1> Result1 { get; set; }

        /// <summary>
        /// Gets or sets the result 2.
        /// </summary>
        public OutArgument<T2> Result2 { get; set; }

        /// <summary>
        ///   Gets or sets Value1.
        /// </summary>
        public InArgument<T1> Value1 { get; set; }

        /// <summary>
        ///   Gets or sets the value 2.
        /// </summary>
        public InArgument<T2> Value2 { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">
        /// The execution context under which the activity executes. 
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            this.Result1.Set(context, this.Value1.Get(context));
            this.Result2.Set(context, this.Value2.Get(context));
        }

        #endregion
    }
}