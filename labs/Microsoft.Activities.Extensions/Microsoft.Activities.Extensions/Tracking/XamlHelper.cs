// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Xaml;

    /// <summary>
    ///   Helpers for dealing with XAML and XAMLX files
    /// </summary>
    public static class XamlHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Loads a XAML or XAMLX file
        /// </summary>
        /// <param name="xamlFile">
        /// The xaml file. 
        /// </param>
        /// <returns>
        /// The activity or root activity of a WorkflowService 
        /// </returns>
        public static Activity Load(string xamlFile)
        {
            switch (GetXamlType(xamlFile))
            {
                case WorkflowXamlFileType.Activity:
                    return ActivityXamlServices.Load(xamlFile);
                case WorkflowXamlFileType.WorkflowService:
                    return ((WorkflowService)XamlServices.Load(xamlFile)).GetWorkflowRoot();
                default:
                    throw new ArgumentException("xamlFile has an invalid file extension");
            }
        }

        /// <summary>
        /// Loads a XAML or XAMLX file
        /// </summary>
        /// <param name="xamlFile">
        /// The xaml file. 
        /// </param>
        /// <param name="localAssembly">
        /// The local assembly. 
        /// </param>
        /// <returns>
        /// The activity or root activity of a WorkflowService 
        /// </returns>
        public static Activity Load(string xamlFile, Assembly localAssembly)
        {
            Contract.Requires(localAssembly != null);
            if (localAssembly == null)
            {
                throw new ArgumentNullException("localAssembly");
            }

            var readerSettings = new XamlXmlReaderSettings
                {
                    LocalAssembly = localAssembly,
                    AllowProtectedMembersOnRoot = true
                };

            var xamlType = GetXamlType(xamlFile);
            switch (xamlType)
            {
                case WorkflowXamlFileType.Activity:
                    using (var reader = new XamlXmlReader(xamlFile, readerSettings))
                    {
                        return ActivityXamlServices.Load(reader);
                    }

                case WorkflowXamlFileType.WorkflowService:
                    using (var reader = new XamlXmlReader(xamlFile, readerSettings))
                    {
                        return ((WorkflowService)XamlServices.Load(reader)).GetWorkflowRoot();
                    }

                default:
                    throw new ArgumentException("Invalid file extension on xamlFile");
            }
        }

        /// <summary>
        /// Loads a WorkflowService
        /// </summary>
        /// <param name="xamlxFile">
        /// The xaml file. 
        /// </param>
        /// <returns>
        /// A WorkflowService 
        /// </returns>
        public static WorkflowService LoadWorkflowService(string xamlxFile)
        {
            switch (GetXamlType(xamlxFile))
            {
                case WorkflowXamlFileType.WorkflowService:
                    return (WorkflowService)XamlServices.Load(xamlxFile);
                default:
                    throw new ArgumentException(string.Format("Invalid XAMLX file name: {0}", xamlxFile));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the XamlType of the file
        /// </summary>
        /// <param name="xamlFile">
        /// The xaml file. 
        /// </param>
        /// <returns>
        /// A XamlType 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The argument was null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The argument was invalid
        /// </exception>
        private static WorkflowXamlFileType GetXamlType(string xamlFile)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(xamlFile));

            if (string.IsNullOrWhiteSpace(xamlFile))
            {
                throw new ArgumentNullException("xamlFile");
            }

            var extension = Path.GetExtension(xamlFile);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("No file extension found");
            }

            if (extension.Equals(Extensions.Constants.XamlxExt, StringComparison.InvariantCultureIgnoreCase))
            {
                return WorkflowXamlFileType.WorkflowService;
            }

            if (extension.Equals(Extensions.Constants.XamlExt, StringComparison.InvariantCultureIgnoreCase))
            {
                return WorkflowXamlFileType.Activity;
            }

            return WorkflowXamlFileType.Unknown;
        }

        #endregion
    }
}