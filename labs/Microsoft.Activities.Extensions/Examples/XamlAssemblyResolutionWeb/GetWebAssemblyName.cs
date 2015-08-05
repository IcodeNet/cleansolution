// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetWebAssemblyName.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The get web assembly name.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XamlAssemblyResolutionWeb
{
    using System.Activities;
    using System.Reflection;

    using ActivityLibrary1;

    using AssemblyName = System.Reflection.AssemblyName;

    /// <summary>
    /// The get web assembly name.
    /// </summary>
    public sealed class GetWebAssemblyName : CodeActivity<AssemblyResolutionInfo>
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
                   Name = name.Name, Version = name.Version.ToString(), Path = Assembly.GetExecutingAssembly().Location 
                };
        }

        #endregion
    }
}