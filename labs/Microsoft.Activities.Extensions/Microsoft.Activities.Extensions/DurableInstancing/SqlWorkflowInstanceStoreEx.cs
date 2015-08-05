// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWorkflowInstanceStoreEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.DurableInstancing
{
    using System.Activities.DurableInstancing;

    /// <summary>
    /// The sql workflow instance store ex.
    /// </summary>
    public static class SqlWorkflowInstanceStoreEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns the readonly status of the instance store.
        /// </summary>
        /// <param name="instanceStore">
        /// The instance store.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool IsReadOnly(this SqlWorkflowInstanceStore instanceStore)
        {
            dynamic store = new ReflectionObject(instanceStore);
            return store.isReadOnly;
        }

        #endregion
    }
}