// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XAMLAssemblyResolutionOption.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    /// <summary>
    /// The xaml assembly resolution option.
    /// </summary>
    public enum XamlAssemblyResolutionOption
    {
        /// <summary>
        /// Will load an assemblies with matching Name, Version and PublicKeyToken (Default CLR behavior).
        /// </summary>
        FullName,

        /// <summary>
        /// Will load an assemblies with matching Name and PublicKeyToken (WF4 behavior).
        /// </summary>
        VersionIndependent, 
    }
}