namespace CleanProject
{
    using System.Collections.Generic;

    /// <summary>
    ///   Cleans solutions
    /// </summary>
    internal class SolutionCleaner
    {
        #region Methods

        internal static void CleanDirectories(IEnumerable<SolutionInfo> directories)
        {
            foreach (var solutionInfo in directories)
            {
                CleanDirectory(solutionInfo.WorkingPath);
            }
        }

        internal static void CleanDirectory(string directory)
        {
            DirectoryHelper.RemoveSubDirectories(directory);
            FileHelper.DeleteFiles(directory, Program.Options.RemoveFiles);

            if (Program.Options.RemoveSourceControl)
            {
                RemoveSourceControlBindings.Clean(directory);
            }
        }

        #endregion
    }
}