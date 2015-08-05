// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// An async result
    /// </summary>
    internal abstract class AsyncResult : IAsyncResult, IDisposable
    {
        #region Static Fields

        /// <summary>
        /// The completion callback
        /// </summary>
        private static AsyncCallback asyncCompletionWrapperCallback;

        #endregion

        #region Fields

        /// <summary>
        /// The callback
        /// </summary>
        private readonly AsyncCallback callback;

        /// <summary>
        /// The state
        /// </summary>
        private readonly object state;

        /// <summary>
        /// The lock
        /// </summary>
        private readonly object thisLock;

        /// <summary>
        /// Completed sync flag
        /// </summary>
        private bool completedSynchronously;

        /// <summary>
        /// endCalled flag
        /// </summary>
        private bool endCalled;

        /// <summary>
        /// The Exception
        /// </summary>
        private Exception exception;

        /// <summary>
        /// The is completed flag
        /// </summary>
        private bool isCompleted;

        /// <summary>
        /// The manual reset event
        /// </summary>
        private ManualResetEvent manualResetEvent;

        /// <summary>
        /// The next async completion
        /// </summary>
        private AsyncCompletion nextAsyncCompletion;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.thisLock = new object();
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Returns the async completion delegate
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        protected delegate bool AsyncCompletion(IAsyncResult result);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets AsyncState.
        /// </summary>
        public object AsyncState
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// Gets AsyncWaitHandle.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this.manualResetEvent != null)
                {
                    return this.manualResetEvent;
                }

                lock (this.ThisLock)
                {
                    if (this.manualResetEvent == null)
                    {
                        this.manualResetEvent = new ManualResetEvent(this.isCompleted);
                    }
                }

                return this.manualResetEvent;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CompletedSynchronously.
        /// </summary>
        public bool CompletedSynchronously
        {
            get
            {
                return this.completedSynchronously;
            }
        }

        /// <summary>
        /// Gets a value indicating whether HasCallback.
        /// </summary>
        public bool HasCallback
        {
            get
            {
                return this.callback != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsCompleted.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets ThisLock.
        /// </summary>
        private object ThisLock
        {
            get
            {
                return this.thisLock;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if an exception is fatal
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <returns>
        /// The is fatal.
        /// </returns>
        public static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException))
                    || exception is ThreadAbortException || exception is AccessViolationException
                    || exception is SEHException)
                {
                    return true;
                }

                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException || exception is TargetInvocationException)
                {
                    exception = exception.InnerException;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// End the async operation
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <typeparam name="TAsyncResult">
        /// The type of the result
        /// </typeparam>
        /// <returns>
        /// The result
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The argument was null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The argument was invalid
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Cannot do this operation at this time
        /// </exception>
        /// <exception cref="Exception">
        /// All other exceptions
        /// </exception>
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            var asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw new ArgumentException("result");
            }

            if (asyncResult.endCalled)
            {
                throw new InvalidOperationException("AsyncResult already ended");
            }

            asyncResult.endCalled = true;

            if (!asyncResult.isCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult.manualResetEvent != null)
            {
                asyncResult.manualResetEvent.Close();
            }

            if (asyncResult.exception != null)
            {
                throw asyncResult.exception;
            }

            return asyncResult;
        }

        /// <summary>
        /// Complete the async operation
        /// </summary>
        /// <param name="completed">
        /// The completed.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The operation is invalid at this time
        /// </exception>
        /// <exception cref="InvalidProgramException">
        /// The callback threw an exception
        /// </exception>
        protected void Complete(bool completed)
        {
            if (this.isCompleted)
            {
                // Don't call Complete twice.
                throw new InvalidOperationException();
            }

            this.completedSynchronously = completed;

            if (completed)
            {
                // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                // we don't need to worry about a race
                this.isCompleted = true;
            }
            else
            {
                lock (this.ThisLock)
                {
                    this.isCompleted = true;
                    if (this.manualResetEvent != null)
                    {
                        this.manualResetEvent.Set();
                    }
                }
            }

            if (this.callback != null)
            {
                try
                {
                    this.callback(this);
                }
                catch (Exception e)
                {
                    throw new InvalidProgramException("Async callback threw an Exception", e);
                }
            }
        }

        /// <summary>
        /// Complete the operation
        /// </summary>
        /// <param name="completed">
        /// The completed.
        /// </param>
        /// <param name="ex">
        /// The exception.
        /// </param>
        protected void Complete(bool completed, Exception ex)
        {
            this.exception = ex;
            this.Complete(completed);
        }

        /// <summary>
        /// Prepare the async completion
        /// </summary>
        /// <param name="cb">
        /// The callback.
        /// </param>
        /// <returns>
        /// The callback
        /// </returns>
        protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion cb)
        {
            this.nextAsyncCompletion = cb;
            return asyncCompletionWrapperCallback ?? (asyncCompletionWrapperCallback = AsyncCompletionWrapperCallback);
        }

        /// <summary>
        /// Wraps the callback
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private static void AsyncCompletionWrapperCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            var thisPtr = (AsyncResult)result.AsyncState;
            var callback = thisPtr.GetNextCompletion();

            bool completeSelf;
            Exception completionException = null;
            try
            {
                completeSelf = callback(result);
            }
            catch (Exception e)
            {
                if (IsFatal(e))
                {
                    throw;
                }

                completeSelf = true;
                completionException = e;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completionException);
            }
        }

        /// <summary>
        /// Disposes of resources
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.manualResetEvent.Close();
            }
        }

        /// <summary>
        /// Gets the next completion
        /// </summary>
        /// <returns>
        /// The async completion
        /// </returns>
        private AsyncCompletion GetNextCompletion()
        {
            var result = this.nextAsyncCompletion;
            this.nextAsyncCompletion = null;
            return result;
        }

        #endregion
    }
}