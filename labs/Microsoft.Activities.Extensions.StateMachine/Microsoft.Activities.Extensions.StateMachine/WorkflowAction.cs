// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowAction.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities;

    /// <summary>
    /// An activity that invokes an action
    /// </summary>
    public class WorkflowAction : AsyncCodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Action.
        /// </summary>
        [RequiredArgument]
        public Action Action { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow.
        /// </summary>
        /// <returns>
        /// The object that saves variable information for an instance of an asynchronous activity. 
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
            return this.Action.BeginInvoke(callback, state);
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <param name="context">
        /// Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/> . 
        /// </param>
        /// <param name="result">
        /// The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends. 
        /// </param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            this.Action.EndInvoke(result);
        }

        #endregion
    }

    /// <summary>
    /// An activity that invokes an action with one argument
    /// </summary>
    /// <typeparam name="T1">
    /// The type of the argument
    /// </typeparam>
    public class WorkflowAction<T1> : AsyncCodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Action.
        /// </summary>
        [RequiredArgument]
        public Action<T1> Action { get; set; }

        /// <summary>
        /// Gets or sets Arg1.
        /// </summary>
        public InArgument<T1> Arg1 { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow.
        /// </summary>
        /// <returns>
        /// The object that saves variable information for an instance of an asynchronous activity. 
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
            return this.Action.BeginInvoke(this.Arg1.Get(context), callback, state);
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <param name="context">
        /// Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/> . 
        /// </param>
        /// <param name="result">
        /// The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends. 
        /// </param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            this.Action.EndInvoke(result);
        }

        #endregion
    }
}