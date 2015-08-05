// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookmarkResumptionException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Runtime.Serialization;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    /// The bookmark resumption exception.
    /// </summary>
    [Serializable]
    public class BookmarkResumptionException : Exception, ITraceable
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkResumptionException"/> class. 
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmark Name. 
        /// </param>
        /// <param name="result">
        /// The result. 
        /// </param>
        /// <param name="value">The resumption value </param>
        /// <param name="innerException">
        /// The inner Exception. 
        /// </param>
        public BookmarkResumptionException(string bookmarkName, BookmarkResumptionResult result, object value = null, Exception innerException = null)
            : base(string.Format(SR.Unable_to_resume_bookmark_name, bookmarkName, result), innerException)
        {
            this.BookmarkName = bookmarkName;
            this.Result = result;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkResumptionException"/> class. 
        /// </summary>
        /// <param name="info">
        /// The serialzation info. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        protected BookmarkResumptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the Bookmark Name that failed to resume
        /// </summary>
        public string BookmarkName { get; private set; }

        /// <summary>
        ///   Gets the BookmarkResumptionResult
        /// </summary>
        public BookmarkResumptionResult Result { get; private set; }

        public object Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The
        ///   <paramref name="info"/>
        ///   parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BookmarkName", this.BookmarkName);
            info.AddValue("Result", this.Result);
            info.AddValue("Value", this.Value);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public string ToFormattedString(int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);
            tsb.AppendLine(this.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendProperty("Message", this.Message);
                tsb.AppendProperty("BookmarkName", this.BookmarkName);
                tsb.AppendProperty("Result", this.Result);
                tsb.AppendProperty("Value", this.Value);
            }

            return tsb.ToString();
        }

        #endregion
    }
}