// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityXamlServicesEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xaml;
    using System.Xml;

    /// <summary>
    /// Diagnostic class with methods to output activities serialized as XAML
    /// </summary>
    public static class ActivityXamlServicesEx
    {
        #region Public Methods

        /// <summary>
        /// Writes an activity to a XAML file
        /// </summary>
        /// <param name="activity">
        /// The activity to write to file
        /// </param>
        /// <param name="fileName">
        /// The name of the file
        /// </param>
        public static void WriteToFile(Activity activity, string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Create);
            WriteToStream(activity, stream);
        }

        /// <summary>
        /// Writes an activity to a stream
        /// </summary>
        /// <param name="activity">
        /// The activity to write
        /// </param>
        /// <param name="stream">
        /// The stream to write to
        /// </param>
        public static void WriteToStream(Activity activity, Stream stream)
        {
            var xmlw = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
            var xw =
                ActivityXamlServices.CreateBuilderWriter(
                    new XamlXmlWriter(xmlw, new XamlSchemaContext(), new XamlXmlWriterSettings { CloseOutput = true }));
            XamlServices.Save(xw, activity);
        }

        /// <summary>
        /// Writes an activity to a string
        /// </summary>
        /// <param name="activity">
        /// The activity to write
        /// </param>
        /// <param name="referenceAssemblies">
        /// The reference Assemblies.
        /// </param>
        /// <returns>
        /// The activity serialized as XAML
        /// </returns>
        public static string WriteToString(Activity activity, IEnumerable<Assembly> referenceAssemblies = null)
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var xmlw = XmlWriter.Create(tw, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
            var xamlSchemaContext = referenceAssemblies != null ? new XamlSchemaContext(referenceAssemblies) : new XamlSchemaContext();
            var xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(xmlw, xamlSchemaContext));
            XamlServices.Save(xw, activity);
            return sb.ToString();
        }

        #endregion
    }
}