// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowCondition.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities;

    /// <summary>
    ///   A condition for a workflow
    /// </summary>
    public class WorkflowCondition : AsyncCodeActivity<bool>
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Condition.
        /// </summary>
        public Func<bool> Condition { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow.
        /// </summary>
        /// <returns>
        /// An object. 
        /// </returns>
        /// <param name="context">
        /// Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/> . 
        /// </param>
        /// <param name="callback">
        /// The method to be called after the asynchronous activity and completion notification have occurred. 
        /// </param>
        /// <param name="state">
        /// An object that saves variable information for an instance of an asynchronous activity. 
        /// </param>
        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            return this.Condition.BeginInvoke(callback, state);
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <returns>
        /// A generic type. 
        /// </returns>
        /// <param name="context">
        /// Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/> . 
        /// </param>
        /// <param name="result">
        /// The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends. 
        /// </param>
        protected override bool EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            return this.Condition.EndInvoke(result);
        }

        #endregion
    }
}