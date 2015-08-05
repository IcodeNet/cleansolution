// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTracker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.Tracking;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   A tracking participant that writes to a file using Async I/O
    /// </summary>
    public sealed class FileTracker : TrackingParticipant, IDisposable
    {
        #region Fields

        /// <summary>
        ///   The ASCII encoding
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        ///   The file
        /// </summary>
        private readonly FileStream file;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTracker"/> class.
        /// </summary>
        /// <param name="path">
        /// The file name. 
        /// </param>
        public FileTracker(string path)
            : this(path, Encoding.ASCII)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTracker"/> class.
        /// </summary>
        /// <param name="path">
        /// The path. 
        /// </param>
        /// <param name="encoding">
        /// The encoding. 
        /// </param>
        public FileTracker(string path, Encoding encoding)
        {
            this.encoding = encoding;
            this.file = new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.file.Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        /// <param name="callback">
        /// The callback. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <returns>
        /// The async result. 
        /// </returns>
        protected override IAsyncResult BeginTrack(
            TrackingRecord record, TimeSpan timeout, AsyncCallback callback, object state)
        {
            var bytes = this.encoding.GetBytes(record.ToFormattedString() + Environment.NewLine);
            return this.file.BeginWrite(bytes, 0, bytes.Length, callback, state);
        }

        /// <summary>
        /// When implemented in a derived class, represents the end of an asynchronous tracking operation.
        /// </summary>
        /// <param name="result">
        /// The status of the operation. 
        /// </param>
        protected override void EndTrack(IAsyncResult result)
        {
            this.file.EndWrite(result);
        }

        /// <summary>
        /// When implemented in a derived class, used to synchronously process the tracking record.
        /// </summary>
        /// <param name="record">
        /// The generated tracking record. 
        /// </param>
        /// <param name="timeout">
        /// The time period after which the provider aborts the attempt. 
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            var bytes = this.encoding.GetBytes(record.ToFormattedString() + Environment.NewLine);
            this.file.Write(bytes, 0, bytes.Length);
        }

        #endregion
    }
}