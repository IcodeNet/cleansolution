// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceExtensionManagerEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Extensions to the WorkflowInstanceExtensionManager class
    /// </summary>
    public static class WorkflowInstanceExtensionManagerEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds a collectio of extensions
        /// </summary>
        /// <param name="extensionManager">
        /// The extension manager.
        /// </param>
        /// <param name="extensions">
        /// The extensions.
        /// </param>
        public static void AddRange(
            this WorkflowInstanceExtensionManager extensionManager, IEnumerable<object> extensions)
        {
            foreach (var extension in extensions)
            {
                extensionManager.Add(extension);
            }
        }

        /// <summary>
        /// Gets a collection of singleton extensions
        /// </summary>
        /// <param name="extensionManager">
        /// The extension manager. 
        /// </param>
        /// <returns>
        /// The collection of singleton extensions 
        /// </returns>
        public static ICollection<object> GetSingletonExtensions(this WorkflowInstanceExtensionManager extensionManager)
        {
            dynamic extensions = new ReflectionObject(extensionManager);

            return extensions != null && extensions.SingletonExtensions != null
                       ? (ICollection<object>)new ReadOnlyCollection<object>(extensions.SingletonExtensions)
                       : new Collection<object>();
        }

        #endregion
    }
}