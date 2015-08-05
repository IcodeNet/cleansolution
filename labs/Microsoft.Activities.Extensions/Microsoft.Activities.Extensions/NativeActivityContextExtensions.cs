// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeActivityContextExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///   The activity context extensions.
    /// </summary>
    public static class NativeActivityContextExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates the point, with the specified name, at which a System.Activities.NativeActivity
        ///   can passively wait to be resumed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark 
        /// </typeparam>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="name">
        /// The name of the bookmark. 
        /// </param>
        /// <returns>
        /// A bookmark that includes the name of the bookmark. 
        /// </returns>
        public static Bookmark CreateBookmark<T>(this NativeActivityContext context, T name)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CreateBookmark(name.ToString());
        }

        /// <summary>
        /// Creates the point, with the specified name, at which a System.Activities.NativeActivity
        ///   can passively wait to be resumed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the name 
        /// </typeparam>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="name">
        /// The name of the bookmark. 
        /// </param>
        /// <param name="callback">
        /// The method to be called when a notification signals resumption of the System.Activities.NativeActivity. 
        /// </param>
        /// <returns>
        /// A bookmark that includes the name of the bookmark and the callback method. 
        /// </returns>
        public static Bookmark CreateBookmark<T>(this NativeActivityContext context, T name, BookmarkCallback callback)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CreateBookmark(name.ToString(), callback);
        }

        /// <summary>
        /// Creates the point, with the specified name, at which a System.Activities.NativeActivity
        ///   can passively wait to be resumed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the name 
        /// </typeparam>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="name">
        /// The name of the bookmark. 
        /// </param>
        /// <param name="callback">
        /// The method to be called when a notification signals resumption of the System.Activities.NativeActivity. 
        /// </param>
        /// <param name="options">
        /// The bookmark options that govern how the bookmark is used during the execution of the current System.Activities.NativeActivity. 
        /// </param>
        /// <returns>
        /// A bookmark that includes the name of the bookmark and the callback method. 
        /// </returns>
        public static Bookmark CreateBookmark<T>(
            this NativeActivityContext context, T name, BookmarkCallback callback, BookmarkOptions options)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CreateBookmark(name.ToString(), callback, options);
        }

        /// <summary>
        /// Creates the point, with the specified name, at which a System.Activities.NativeActivity
        ///   can passively wait to be resumed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the name 
        /// </typeparam>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="name">
        /// The name of the bookmark. 
        /// </param>
        /// <param name="callback">
        /// The method to be called when a notification signals resumption of the System.Activities.NativeActivity. 
        /// </param>
        /// <param name="scope">
        /// An identifier applied to a group of bookmarks that operate under the same protocol during a workflow runtime. 
        /// </param>
        /// <returns>
        /// A bookmark that includes the name of the bookmark and the callback method. 
        /// </returns>
        public static Bookmark CreateBookmark<T>(
            this NativeActivityContext context, T name, BookmarkCallback callback, BookmarkScope scope)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CreateBookmark(name.ToString(), callback, scope);
        }

        /// <summary>
        /// Creates the point, with the specified name, at which a System.Activities.NativeActivity
        ///   can passively wait to be resumed.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the name 
        /// </typeparam>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="name">
        /// The name of the bookmark. 
        /// </param>
        /// <param name="callback">
        /// The method to be called when a notification signals resumption of the System.Activities.NativeActivity. 
        /// </param>
        /// <param name="scope">
        /// An identifier applied to a group of bookmarks that operate under the same protocol during a workflow runtime. 
        /// </param>
        /// <param name="options">
        /// The bookmark options that govern how the bookmark is used during the execution of the current System.Activities.NativeActivity. 
        /// </param>
        /// <returns>
        /// A bookmark that includes the name of the bookmark and the callback method. 
        /// </returns>
        public static Bookmark CreateBookmark<T>(
            this NativeActivityContext context, 
            T name, 
            BookmarkCallback callback, 
            BookmarkScope scope, 
            BookmarkOptions options)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.CreateBookmark(name.ToString(), callback, scope, options);
        }

        /// <summary>
        /// Resumes the specified bookmark.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <param name="bookmarkName">
        /// A point at which a NativeActivity can passively wait to be resumed. 
        /// </param>
        /// <param name="value">
        /// The information related to the resumption of a bookmark. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the bookmarkName 
        /// </typeparam>
        /// <returns>
        /// A BookmarkResumptionResult. 
        /// </returns>
        public static BookmarkResumptionResult ResumeBookmark<T>(
            this NativeActivityContext context, T bookmarkName, object value = null)
        {
            Contract.Requires(context != null);
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.ResumeBookmark(new Bookmark(bookmarkName.ToString()), value);
        }

        #endregion
    }
}