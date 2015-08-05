namespace Microsoft.Activities.UnitTesting.Activities
{
    using System;
    using System.Activities;
    using System.Threading;

    /// <summary>
    /// An async class that sleeps for a duration
    /// </summary>
    /// <remarks>
    /// While similar to the Delay activity,
    /// this class does not create a bookmark
    /// allowing you to simulate an async operation 
    /// when testing
    /// </remarks>
    public class TestAsync : AsyncCodeActivity
    {
        #region Public Properties

        /// <summary>
        /// The number of milliseconds to sleep
        /// </summary>
        public InArgument<int> Sleep { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow. 
        /// </summary>
        /// <returns>
        /// The object that saves variable information for an instance of an asynchronous activity.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="callback">The method to be called after the asynchronous activity and completion notification have occurred.</param><param name="state">An object that saves variable information for an instance of an asynchronous activity.</param>
        protected override IAsyncResult BeginExecute(
            AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var sleep = this.Sleep.Get(context);

            Action action = () => Thread.Sleep(sleep);

            context.UserState = action;

            return action.BeginInvoke(callback, state);
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="result">The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends.</param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            var action = (Action)context.UserState;
            action.EndInvoke(result);
        }

        #endregion
    }
}