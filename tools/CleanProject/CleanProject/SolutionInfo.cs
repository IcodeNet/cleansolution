namespace CleanProject
{
    using System;
    using System.IO;
    using System.Linq;

    internal class SolutionInfo
    {
        #region Constants and Fields

        private string name;

        private string tempPath;

        #endregion

        #region Public Properties

        public string TempPath
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.tempPath) ? this.GetTempPath() : this.tempPath;
            }
        }

        public string WorkingPath
        {
            get
            {
                return Program.Options.ZipProject ? this.TempPath : this.Directory;
            }
        }

        #endregion

        #region Properties

        internal string Directory { get; set; }

        internal string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion

        #region Methods

        private string GetTempPath()
        {
            if (string.IsNullOrWhiteSpace(this.name))
            {
                this.name = this.Directory.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
            }

            this.tempPath = Path.Combine(Path.GetTempPath(), this.name);

            return this.tempPath;
        }

        #endregion
    }
}