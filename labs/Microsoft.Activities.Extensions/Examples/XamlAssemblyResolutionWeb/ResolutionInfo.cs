// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The resolution info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XamlAssemblyResolutionWeb
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using ActivityLibrary1;

    /// <summary>
    /// The resolution info.
    /// </summary>
    [KnownType(typeof(Version))]
    public class ResolutionInfo
    {
        #region Properties

        /// <summary>
        ///   Gets or sets ActivityAssembly.
        /// </summary>
        public AssemblyResolutionInfo ActivityAssembly { get; set; }

        /// <summary>
        ///   Gets or sets WebAssembly.
        /// </summary>
        public AssemblyResolutionInfo WebAssembly { get; set; }

        #endregion
    }
}