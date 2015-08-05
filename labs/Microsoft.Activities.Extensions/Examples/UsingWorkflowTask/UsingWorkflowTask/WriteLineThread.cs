// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WriteLineThread.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingWorkflowTask
{
    using System;
    using System.Activities;
    using System.Threading;

    /// <summary>
    /// The write line thread.
    /// </summary>
    public sealed class WriteLineThread : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        ///   Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

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
            var args = this.Args ?? new object[0];

            Console.WriteLine(
                "[{0,2:00}] {1}", Thread.CurrentThread.ManagedThreadId, string.Format(this.Message, args));
        }

        #endregion
    }
}