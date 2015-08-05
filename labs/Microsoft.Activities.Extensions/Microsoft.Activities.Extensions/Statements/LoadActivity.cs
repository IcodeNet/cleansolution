// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadActivity.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Reflection;
    using System.Xaml;

    /// <summary>
    /// Loads a workflow definition from a XAML file
    /// </summary>
    /// <remarks>
    /// Use this activity to load XAML activities (not workflow services)
    /// </remarks>
    public sealed class LoadActivity : CodeActivity<Activity>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets LocalAssembly.
        /// </summary>
        public InArgument<Assembly> LocalAssembly { get; set; }

        /// <summary>
        /// Gets or sets Path.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> Path { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// An activity loaded from a file
        /// </returns>
        protected override Activity Execute(CodeActivityContext context)
        {
            Assembly localAssembly = this.LocalAssembly.Get(context);

            // Setup the local assembly in the schema context if required
            return localAssembly != null
                       ? ActivityXamlServices.Load(
                           ActivityXamlServices.CreateReader(
                               new XamlXmlReader(
                             this.Path.Get(context), new XamlXmlReaderSettings { LocalAssembly = localAssembly })))
                       : ActivityXamlServices.Load(this.Path.Get(context));
        }

        #endregion
    }
}