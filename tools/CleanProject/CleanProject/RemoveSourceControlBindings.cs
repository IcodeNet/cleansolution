namespace CleanProject
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml;

    internal class RemoveSourceControlBindings
    {
        #region Constants and Fields

        private const string EndGlobalSection = "EndGlobalSection";

        private const string GlobalSection = "GlobalSection(TeamFoundationVersionControl)";

        private const string ProjectPattern = "*.*proj";

        private const string SolutionPattern = "*.sln";

        private readonly string[] bindingPattern = new[] { "*.vssscc", "*.vspscc", "*.scc" };

        private readonly RecursiveSearchHelper recursiveSearch;

        #endregion

        #region Constructors and Destructors

        internal RemoveSourceControlBindings()
        {
            this.recursiveSearch = new RecursiveSearchHelper();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Get or Set the temporal directory for process proposes
        /// </summary>
        internal string ProcessDirectory { get; set; }

        #endregion

        #region Methods

        internal static void Clean(string directory)
        {
            var scCleaner = new RemoveSourceControlBindings { ProcessDirectory = directory };
            scCleaner.Execute();
        }

        /// <summary>
        ///   Remove Source Control Bindings according to a specified binding pattern.
        /// </summary>
        internal virtual void Execute()
        {
            this.DeleteSourceControlFiles();
            this.CleanSolutions();
            this.CleanProjects();
        }

        protected virtual void CleanProjects()
        {
            var projects = this.recursiveSearch.GetFiles(this.ProcessDirectory, ProjectPattern);

            foreach (var projectFile in projects)
            {
                Program.WriteVerboseMessage("Cleaning project file  {0}", projectFile);

                try
                {
                    var projectDocument = XDocument.Load(projectFile);

                    foreach (var item in projectDocument.Elements())
                    {
                        this.RemoveSccElements(item);
                    }
                    var readOnly = FileHelper.TurnOffReadOnlyFlag(projectFile);

                    projectDocument.Save(projectFile);

                    if (readOnly)
                    {
                        FileHelper.TurnOnReadOnlyFlag(projectFile);
                    }

                }
                catch (XmlException)
                {
                    Program.WriteVerboseMessage("Invalid project file  {0}", projectFile);                    
                }
            }
        }

        protected virtual void CleanSolutions()
        {
            var solutions = this.recursiveSearch.GetFiles(this.ProcessDirectory, SolutionPattern);

            foreach (var solutionFile in solutions)
            {
                Program.WriteVerboseMessage("Cleaning solution {0}", solutionFile);

                var solutionText = File.ReadAllText(solutionFile);

                if (solutionText.Contains(GlobalSection))
                {
                    var tfs = GetTfsGlobalSection(solutionText);

                    var readOnly = FileHelper.TurnOffReadOnlyFlag(solutionFile);

                    File.WriteAllText(solutionFile, solutionText.Replace(tfs, null), Encoding.UTF8);

                    if (readOnly)
                    {
                        FileHelper.TurnOnReadOnlyFlag(solutionFile);
                    }
                }
            }
        }

        protected virtual void DeleteSourceControlFiles()
        {
            var files = new List<string>();

            foreach (var pattern in this.bindingPattern)
            {
                files.AddRange(this.recursiveSearch.GetFiles(this.ProcessDirectory, pattern));
            }

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                Program.WriteVerboseMessage("Deleting file {0}", file);
                File.Delete(file);
            }
        }

        private static string GetTfsGlobalSection(string solutionText)
        {
            var globalTfsSection = solutionText.Substring(solutionText.IndexOf(GlobalSection, StringComparison.OrdinalIgnoreCase));
            globalTfsSection = globalTfsSection.Substring(
                0, (globalTfsSection.IndexOf(EndGlobalSection, StringComparison.OrdinalIgnoreCase) + EndGlobalSection.Length));

            return globalTfsSection;
        }

        private void RemoveSccElements(XElement node)
        {
            if (node.Name.LocalName.Contains("Scc"))
            {
                node.RemoveAll();
            }
            else if (node.HasElements)
            {
                foreach (var element in node.Elements())
                {
                    this.RemoveSccElements(element);
                }
            }
        }

        #endregion
    }
}