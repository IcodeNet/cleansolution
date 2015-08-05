// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendStubBase.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.ServiceModel.Activities;
    using System.Threading.Tasks;

    /// <summary>
    /// The send stub base.
    /// </summary>
    public abstract class SendStubBase : AsyncCodeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Content.
        /// </summary>
        [DefaultValue(null)]
        public SendContent Content { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Begin async execute
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// An IAsync result
        /// </returns>
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var stub = context.GetExtension<IMessagingStub>();

            if (stub == null)
            {
                throw new InvalidOperationException("Cannot locate extension of type IMessagingStub");
            }

            if (this.Content is SendParametersContent)
            {
                {
                    var parametersContent = this.Content as SendParametersContent;
                    this.Send(stub, parametersContent.Parameters.ToDictionary(pair => pair.Key, pair => parametersContent.Parameters[pair.Key].Get(context)));
                }
            }
            else if (this.Content is SendMessageContent)
            {
                var messageContent = this.Content as SendMessageContent;
                this.Send(stub, messageContent.Message.Get(context));
            }
            else
            {
                this.Send(stub, null);
            }

            var task = Task.Factory.StartNew(
                _ =>
                {
                    // Allow for subclasses to provide different implementations based on the activity
                    // They can override GetImplementation to provide it
                    var implementation = stub.GetImplementation(this);
                    if (implementation != null)
                    {
                        implementation();
                    }
                },
                state);

            if (callback != null)
            {
                task.ContinueWith(_ => callback(task));
            }

            return task;
        }

        /// <summary>
        /// The cache metadata.
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.RequireExtension<IMessagingStub>();

            var parametersContent = this.Content as SendParametersContent;

            if (parametersContent != null)
            {
                foreach (var pair in parametersContent.Parameters)
                {
                    var argument = new RuntimeArgument(pair.Key, pair.Value.ArgumentType, ArgumentDirection.In);
                    metadata.Bind(pair.Value, argument);
                    metadata.AddArgument(argument);
                }
            }

            var messageContent = this.Content as SendMessageContent;
            if (messageContent != null)
            {
                var runtimeArgumentType = (messageContent.Message == null) ? typeof(object) : messageContent.Message.ArgumentType;
                var argument = new RuntimeArgument("Message", runtimeArgumentType, ArgumentDirection.In);
                metadata.Bind(messageContent.Message, argument);
                metadata.AddArgument(argument);
            }

            // Note: Not adding other properties here as arguments
        }

        /// <summary>
        /// The end execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            // Wait will throw aggregate exceptions
            try
            {
                ((Task)result).Wait();
            }
            catch (AggregateException aggregateException)
            {
                // Test code will not expect an aggregate exception
                throw aggregateException.InnerException;
            }
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stub">
        /// The stub.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        protected abstract void Send(IMessagingStub stub, object messageContent);

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stub">
        /// The stub.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        protected abstract void Send(IMessagingStub stub, Dictionary<string, object> parametersContent);

        #endregion
    }
}