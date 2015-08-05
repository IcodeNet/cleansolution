namespace CleanProject
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CmdLine;

    using Ionic.Zip;

    internal class ZipHelper
    {
        #region Methods

        internal static void CreateZipFile(SolutionInfo si)
        {
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(si.TempPath);
                var zipDirectory = Program.Options.ZipDirectory;

                // No ZipDirectory provided
                if (String.IsNullOrWhiteSpace(zipDirectory))
                {
                    // Default to the parent folder of the solution
                    zipDirectory = Path.GetFullPath(Path.Combine(si.Directory, ".."));
                    if (!Directory.Exists(zipDirectory))
                    {
                        // No parent folder then use the solution directory
                        zipDirectory = si.Directory;
                    }
                }

                var zipName = Path.Combine(zipDirectory, si.Name + ".zip");
                zip.Save(zipName);
                CommandLine.WriteLineColor(ConsoleColor.Yellow, "Created zip file {0}", zipName);
                DirectoryHelper.Remove(si.TempPath);
            }
        }

        internal static void ZipDirectories(IEnumerable<SolutionInfo> directories)
        {
            foreach (var solutionInfo in directories)
            {
                CreateZipFile(solutionInfo);
            }
        }

        #endregion
    }
}