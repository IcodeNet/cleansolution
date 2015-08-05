namespace CleanProject
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class RecursiveSearchHelper
    {
        #region Constants and Fields

        private readonly List<string> excludeList;

        private readonly List<string> fileList;

        #endregion

        #region Constructors and Destructors

        internal RecursiveSearchHelper()
        {
            this.fileList = new List<string>();
            this.excludeList = new List<string>();
        }

        #endregion

        #region Methods

        internal string[] GetFiles(string initialDirectory, string filePattern)
        {
            this.fileList.Clear();

            this.Search(initialDirectory, filePattern);

            return this.fileList.ToArray();
        }

        internal string[] GetFiles(string initialDirectory, string[] filePatterns, string[] excludePatterns)
        {
            this.fileList.Clear();
            this.excludeList.Clear();

            foreach (string filePattern in filePatterns)
            {
                this.Search(initialDirectory, filePattern);
            }

            if (excludePatterns != null)
            {
                foreach (string excludePattern in excludePatterns)
                {
                    this.SearchExclude(initialDirectory, excludePattern);
                }
            }

            foreach (var file in this.excludeList)
            {
                this.fileList.RemoveAll(s => s == file);
            }

            return this.fileList.ToArray();
        }

        private void Search(string initialDirectory, string filePattern)
        {
            foreach (var file in Directory.GetFiles(initialDirectory, filePattern).Where(file => !this.fileList.Contains(file)))
            {
                this.fileList.Add(file);
            }

            foreach (string item in Directory.GetDirectories(initialDirectory))
            {
                this.Search(item, filePattern);
            }
        }

        private void SearchExclude(string initialDirectory, string excludePattern)
        {
            foreach (string file in Directory.GetFiles(initialDirectory, excludePattern).Where(file => !this.excludeList.Contains(file)))
            {
                this.excludeList.Add(file);
            }

            foreach (string item in Directory.GetDirectories(initialDirectory))
            {
                this.SearchExclude(item, excludePattern);
            }
        }

        #endregion
    }
}