// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuAction.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingTPLWithWorkflowApplication
{
    /// <summary>
    /// The menu action.
    /// </summary>
    internal enum MenuAction
    {
        /// <summary>
        ///   The none value.
        /// </summary>
        None, 

        /// <summary>
        ///   The resume.
        /// </summary>
        Resume, 

        /// <summary>
        ///   The cancel.
        /// </summary>
        Cancel, 

        /// <summary>
        ///   The abort.
        /// </summary>
        Abort, 

        /// <summary>
        ///   Terminate the workflow
        /// </summary>
        Terminate, 
    }
}