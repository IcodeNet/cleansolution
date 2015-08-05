// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveStubBase.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel.Activities;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// The receive stub base.
    /// </summary>
    public abstract class ReceiveStubBase : NativeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Action.
        /// </summary>
        [DefaultValue(null)]
        public string Action { get; set; }

        /// <summary>
        ///   Gets or sets Content.
        /// </summary>
        [DefaultValue(null)]
        public ReceiveContent Content { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether CanInduceIdle.
        /// </summary>
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cache metadata.
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<IMessagingStub>();

            var parametersContent = this.Content as ReceiveParametersContent;

            if (parametersContent != null)
            {
                foreach (var pair in parametersContent.Parameters)
                {
                    var argument = new RuntimeArgument(pair.Key, pair.Value.ArgumentType, ArgumentDirection.Out);
                    metadata.Bind(pair.Value, argument);
                    metadata.AddArgument(argument);
                }
            }

            var messageContent = this.Content as ReceiveMessageContent;
            if (messageContent != null)
            {
                var runtimeArgumentType = (messageContent.Message == null) ? typeof(object) : messageContent.Message.ArgumentType;
                var argument = new RuntimeArgument("Message", runtimeArgumentType, ArgumentDirection.Out);
                metadata.Bind(messageContent.Message, argument);
                metadata.AddArgument(argument);
            }

            // Note: Not adding other properties here as arguments
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            WorkflowTrace.Information("ReceiveStubBase creating bookmark {0}", this.GetBookmarkName());
            context.CreateBookmark(this.GetBookmarkName(), this.OnBookmarkCallback);
        }

        /// <summary>
        /// Gets the bookmark name
        /// </summary>
        /// <returns>
        /// The bookmark name.
        /// </returns>
        protected abstract string GetBookmarkName();

        /// <summary>
        /// The on bookmark callback.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="bookmark">
        /// The bookmark.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void OnBookmarkCallback(NativeActivityContext context, Bookmark bookmark, object value)
        {
            var stub = context.GetExtension<IMessagingStub>();

            if (this.Content is ReceiveParametersContent)
            {
                var parametersContent = this.Content as ReceiveParametersContent;
                var parameters = (IDictionary<string, object>)value;

                foreach (var pair in parameters)
                {
                    if (parametersContent.Parameters.ContainsKey(pair.Key))
                    {
                        parametersContent.Parameters[pair.Key].Set(context, pair.Value);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Failed to set message parameter \"{0}\" because Activity \"{1}\" does not contain a message parameter named \"{0}\"", 
                                pair.Key, 
                                this.DisplayName));
                    }
                }
            }
            else if (this.Content is ReceiveMessageContent)
            {
                var messageContent = this.Content as ReceiveMessageContent;
                messageContent.Message.Set(context, value);
            }

            if (stub != null)
            {
                // Allow for subclasses to provide different implementations based on the activity
                // They can override GetImplementation to provide it
                var implementation = stub.GetImplementation(this);
                if (implementation != null)
                {
                    implementation();
                }
            }
        }

        #endregion
    }
}