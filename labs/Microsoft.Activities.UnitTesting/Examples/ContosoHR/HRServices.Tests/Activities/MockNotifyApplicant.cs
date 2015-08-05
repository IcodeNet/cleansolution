// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockNotifyApplicant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HRServices.Tests.Activities
{
    using System.Activities;

    using HRApplicationServices.Contracts;

    /// <summary>
    /// Mock notify applicant
    /// </summary>
    public sealed class MockNotifyApplicant : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        public InArgument<bool> Cancel { get; set; }

        /// <summary>
        /// Gets or sets the hire.
        /// </summary>
        public InArgument<bool> Hire { get; set; }

        /// <summary>
        /// Gets or sets the resume.
        /// </summary>
        public InArgument<ApplicantResume> Resume { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override void Execute(CodeActivityContext context)
        {
            // Do nothing
        }

        #endregion
    }
}