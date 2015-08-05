// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyName.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The assembly name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ActivityLibrary1
{
    using System.Activities;
    using System.Reflection;

    /// <summary>
    /// Activity that returns the assembly name.
    /// </summary>
    public sealed class AssemblyName : CodeActivity<AssemblyResolutionInfo>
    {
        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// </returns>
        protected override AssemblyResolutionInfo Execute(CodeActivityContext context)
        {
            var name = this.GetType().Assembly.GetName();

            return new AssemblyResolutionInfo
            {
                Name = name.Name,
                Version = name.Version.ToString(),
                Path = Assembly.GetExecutingAssembly().Location
            };
        }

        #endregion
    }
}