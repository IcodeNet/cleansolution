// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskWaiter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrackingStateMachine
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///   Waits on a list of tasks
    /// </summary>
    public sealed class TaskWaiter : IDisposable
    {
        #region Fields

        /// <summary>
        ///   The countdown event
        /// </summary>
        private readonly CountdownEvent countdownEvent = new CountdownEvent(1);

        /// <summary>
        ///   Indicates done adding
        /// </summary>
        private bool doneAdding;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a task to the wait list
        /// </summary>
        /// <param name="task">
        /// The task. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The task is null
        /// </exception>
        public void Add(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            this.countdownEvent.AddCount();
            task.ContinueWith(ct => this.countdownEvent.Signal());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.countdownEvent.Dispose();
        }

        /// <summary>
        ///   Waits for the tasks
        /// </summary>
        public void Wait()
        {
            if (!this.doneAdding)
            {
                this.doneAdding = true;
                this.countdownEvent.Signal();
            }

            this.countdownEvent.Wait();
        }

        #endregion
    }
}