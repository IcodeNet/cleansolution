// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InArgumentExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System.Activities;

    /// <summary>
    /// The in argument extensions.
    /// </summary>
    public static class InArgumentExtensions
    {
        #region Public Methods

        /// <summary>
        /// Gets the value of an argument or the default value if there is no expression
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument
        /// </typeparam>
        /// <returns>
        /// the value of an argument or the default value if there is no expression
        /// </returns>
        /// <example>
        /// An activity that has an optional in argument
        ///   <code source="Examples\CSDocExamples\ActivityWithOptionalArgs.cs" lang="CSharp">
        /// </code>
        /// </example>
        public static T Get<T>(this InArgument<T> argument, ActivityContext context, T defaultValue)
        {
            var value = argument.Get(context);

            // If the value is the default value and the user did not supply an expression
            if (Equals(value, default(T)) && argument.Expression == null)
            {
                // Use the default value instead
                value = defaultValue;
            }

            return value;
        }

        #endregion
    }
}