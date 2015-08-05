namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Xaml;
    using System.Xml;

    /// <summary>
    /// Provides a way to load a XAML file and replace types with test doubles
    /// </summary>
    public class XamlInjector : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The test doubles.
        /// </summary>
        private readonly XamlTestDoubles testDoubles = new XamlTestDoubles();

        /// <summary>
        ///   The xaml file.
        /// </summary>
        private readonly string xamlFile = "Injected.xaml";

        /// <summary>
        ///   The xaml xml reader.
        /// </summary>
        private readonly XamlXmlReader xamlXmlReader;

        /// <summary>
        ///   The injected activity.
        /// </summary>
        private Activity injectedActivity;

        /// <summary>
        ///   The injected service.
        /// </summary>
        private WorkflowService injectedService;

        /// <summary>
        ///   The test xaml file.
        /// </summary>
        private string testXamlFile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="xamlFile">
        /// The xaml file.
        /// </param>
        public XamlInjector(string xamlFile)
        {
            this.xamlFile = xamlFile;
            this.xamlXmlReader = new XamlXmlReader(xamlFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public XamlInjector(Stream stream)
        {
            this.xamlXmlReader = new XamlXmlReader(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="textReader">
        /// The text reader.
        /// </param>
        public XamlInjector(TextReader textReader)
        {
            this.xamlXmlReader = new XamlXmlReader(textReader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="xmlReader">
        /// The xml reader.
        /// </param>
        public XamlInjector(XmlReader xmlReader)
        {
            this.xamlXmlReader = new XamlXmlReader(xmlReader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="xamlXmlReader">
        /// The xaml xml reader.
        /// </param>
        public XamlInjector(XamlXmlReader xamlXmlReader)
        {
            this.xamlXmlReader = xamlXmlReader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlInjector"/> class.
        /// </summary>
        /// <param name="xamlFile">
        /// The xaml File.
        /// </param>
        /// <param name="overrideDefaultAssembly">
        /// The assembly in which your custom workflow types are defined.
        /// </param>
        public XamlInjector(string xamlFile, Assembly overrideDefaultAssembly)
            : this(new XamlXmlReader(xamlFile, new XamlXmlReaderSettings { LocalAssembly = overrideDefaultAssembly }))
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the activity after injection
        /// </summary>
        /// <returns>
        /// The activity with test doubles
        /// </returns>
        public Activity GetActivity()
        {
            if (this.injectedActivity != null)
            {
                return this.injectedActivity;
            }

            this.GenerateUniqueTestFilename("xaml");
            this.InjectXaml();
            return this.injectedActivity = ActivityXamlServices.Load(this.testXamlFile);
        }

        /// <summary>
        /// Gets a workflow service after injection
        /// </summary>
        /// <returns>
        /// The workflow service with test doubles
        /// </returns>
        public WorkflowService GetWorkflowService()
        {
            if (this.injectedService != null)
            {
                return this.injectedService;
            }

            this.GenerateUniqueTestFilename("xamlx");
            this.InjectXaml();
            return this.injectedService = (WorkflowService)XamlServices.Load(this.testXamlFile);
        }

        /// <summary>
        /// Replaces all instances of searchType with replaceType
        /// </summary>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <param name="replaceType">
        /// The replace type.
        /// </param>
        public void ReplaceAll(Type searchType, Type replaceType)
        {
            this.testDoubles.AddDouble(searchType, replaceType);
        }

        /// <summary>
        /// Replaces only the instance at index if it is of type searchType
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <param name="replaceType">
        /// The replace type.
        /// </param>
        public void ReplaceAt(int index, Type searchType, Type replaceType)
        {
            this.testDoubles.AddDouble(searchType, replaceType, false, index);
        }

        #endregion

        #region Methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.xamlXmlReader.Close();
            }
        }

        /// <summary>
        /// The generate unique test filename.
        /// </summary>
        /// <param name="extension">
        /// The extension.
        /// </param>
        private void GenerateUniqueTestFilename(string extension)
        {
            var fileNum = 0;

            // Generate a unique file name for the injected xaml file
            do
            {
                this.testXamlFile = string.Format(
                    "{0}.{1}.Test.{2}", Path.GetFileNameWithoutExtension(this.xamlFile), fileNum++, extension);
            }
            while (File.Exists(this.testXamlFile));
        }

        /// <summary>
        /// Injects the XAML with replacement types
        /// </summary>
        private void InjectXaml()
        {
            var writerSettings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            // using (XmlReader.Create(this.xamlFile))
            using (var xmlWriter = XmlWriter.Create(this.testXamlFile, writerSettings))
            using (this.xamlXmlReader)
            using (var xamlWriter = new XamlXmlWriter(xmlWriter, this.xamlXmlReader.SchemaContext))
            {
                var skippedLastNode = false;
                while (skippedLastNode || this.xamlXmlReader.Read())
                {
                    skippedLastNode = false;

                    Type replacementType;
                    if (this.xamlXmlReader.NodeType == XamlNodeType.StartObject
                        && this.xamlXmlReader.Type.UnderlyingType != null
                        && this.testDoubles.TryGetDouble(this.xamlXmlReader.Type.UnderlyingType, out replacementType))
                    {
                        xamlWriter.WriteStartObject(xamlWriter.SchemaContext.GetXamlType(replacementType));
                    }
                    else if (this.xamlXmlReader.NodeType == XamlNodeType.StartMember
                             &&
                             (this.xamlXmlReader.Member == XamlLanguage.Base
                              || this.xamlXmlReader.Member == XamlLanguage.Space))
                    {
                        // Skip XML members that are automatically added by the XAML stack
                        this.xamlXmlReader.Skip();
                        skippedLastNode = !this.xamlXmlReader.IsEof;
                    }
                    else
                    {
                        xamlWriter.WriteNode(this.xamlXmlReader);
                    }
                }
            }
        }

        #endregion
    }
}