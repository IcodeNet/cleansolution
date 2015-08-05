// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInstanceID.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;

    /// <summary>
    /// Activity that returns the workflow instance ID
    /// </summary>
    public sealed class GetInstanceId : CodeActivity<Guid>
    {
        #region Methods

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override Guid Execute(CodeActivityContext context)
        {
            return context.WorkflowInstanceId;
        }

        #endregion
    }
}