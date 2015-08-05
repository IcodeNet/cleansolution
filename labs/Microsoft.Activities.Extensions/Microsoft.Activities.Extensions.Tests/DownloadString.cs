// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadString.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Diagnostics;
    using System.Net;

    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The download string.
    /// </summary>
    public class DownloadString : EventAsyncActivity<string>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        public InArgument<Uri> Uri { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(AsyncCodeActivityContext context)
        {
            Debug.WriteLine("Async Operation Starting");

            var webClient = new WebClient();
            webClient.DownloadStringCompleted += this.OnCompleted;
            webClient.DownloadStringAsync(this.Uri.Get(context));
        }

        #endregion
    }
}