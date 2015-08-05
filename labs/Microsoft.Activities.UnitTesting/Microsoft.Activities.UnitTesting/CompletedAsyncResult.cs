// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletedAsyncResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///   An AsyncResult that completes as soon as it is instantiated.
    /// </summary>
    internal class CompletedAsyncResult : AsyncResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletedAsyncResult"/> class.
        /// </summary>
        /// <param name="callback">
        /// The callback. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        public CompletedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.Complete(true);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The end.
        /// </summary>
        /// <param name="result">
        /// The result. 
        /// </param>
        public static void End(IAsyncResult result)
        {
            End<CompletedAsyncResult>(result);
        }

        #endregion
    }

    /// <summary>
    /// The completed async result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Reviewed. Suppression is OK here.")]
    internal class CompletedAsyncResult<T> : AsyncResult
    {
        #region Fields

        /// <summary>
        ///   The _data.
        /// </summary>
        private readonly T data;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletedAsyncResult{T}"/> class.
        /// </summary>
        /// <param name="data">
        /// The data. 
        /// </param>
        /// <param name="callback">
        /// The callback. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        public CompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.data = data;
            this.Complete(true);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The end.
        /// </summary>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <returns>
        /// The result
        /// </returns>
        public static T End(IAsyncResult result)
        {
            var completedResult = End<CompletedAsyncResult<T>>(result);
            return completedResult.data;
        }

        #endregion
    }
}