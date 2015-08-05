// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadAssembly.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System.Activities;
    using System.Reflection;

    /// <summary>
    /// Loads an assembly into the current AppDomain
    /// </summary>
    public sealed class LoadAssembly : NativeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Assembly.
        /// </summary>
        [RequiredArgument]
        public OutArgument<Assembly> Assembly { get; set; }

        /// <summary>
        /// Gets or sets Path to the assembly you want to load.
        /// </summary>
        public InArgument<string> Path { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Caches metadata
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddAndBindArgument(this.Path, new RuntimeArgument("Path", typeof(string), ArgumentDirection.In, true));
            metadata.AddAndBindArgument(
                this.Assembly, new RuntimeArgument("Assembly", typeof(Assembly), ArgumentDirection.Out));
        }

        /// <summary>
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            this.Assembly.Set(context, System.Reflection.Assembly.LoadFrom(this.Path.Get(context)));
        }

        #endregion
    }
}