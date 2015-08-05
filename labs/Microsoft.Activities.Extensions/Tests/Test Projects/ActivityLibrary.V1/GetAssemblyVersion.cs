// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAssemblyVersion.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ActivityLibrary
{
    using System;
    using System.Activities;
    using System.Reflection;

    /// <summary>
    /// The get assembly version.
    /// </summary>
    public sealed class GetAssemblyVersion : CodeActivity<Version>
    {
        #region Methods

        /// <summary>
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The assembly version
        /// </returns>
        protected override Version Execute(CodeActivityContext context)
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        #endregion
    }
}