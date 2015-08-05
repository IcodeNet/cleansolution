// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CleanProject
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///   Helpers for dealing with directories
    /// </summary>
    internal class DirectoryHelper
    {
        #region Static Fields

        /// <summary>
        ///   The list of excluded directories
        /// </summary>
        private static List<string> excludedDirs;

        /// <summary>
        ///   List of directories to be removed
        /// </summary>
        private static List<string> removeDirs;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Finds the list of excluded directories
        /// </summary>
        public static void FindExcludedDirectories()
        {
            excludedDirs = new List<string>();

            if (Program.Options.ExcludeDirectories == null)
            {
                return;
            }

            foreach (var pattern in Program.Options.ExcludeDirectories)
            {
                foreach (var directory in Program.Options.Directories)
                {
                    excludedDirs.AddRange(Directory.GetDirectories(directory, pattern, SearchOption.AllDirectories));
                }
            }
        }

        /// <summary>
        ///   Finds the list of Remove directories
        /// </summary>
        public static void FindRemoveDirectories()
        {
            removeDirs = new List<string>();

            if (Program.Options.RemoveDirectories == null)
            {
                return;
            }

            foreach (var pattern in Program.Options.RemoveDirectories)
            {
                foreach (var directory in Program.Options.Directories)
                {
                    removeDirs.AddRange(Directory.GetDirectories(directory, pattern, SearchOption.AllDirectories));
                }
            }
        }

        /// <summary>
        /// Removes SubDirectories not excluded
        /// </summary>
        /// <param name="directory">
        /// The directory. 
        /// </param>
        /// <param name="searchPattern">
        /// The search pattern. 
        /// </param>
        public static void RemoveSubDirectories(string directory, string searchPattern)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            foreach (var d in
                Directory.GetDirectories(directory, searchPattern, SearchOption.AllDirectories).Except(excludedDirs))
            {
                Remove(d);
            }
        }

        /// <summary>
        /// Removes SubDirectories
        /// </summary>
        /// <param name="directory">
        /// The directory. 
        /// </param>
        public static void RemoveSubDirectories(string directory)
        {
            foreach (var pattern in Program.Options.RemoveDirectories.Except(excludedDirs))
            {
                RemoveSubDirectories(directory, pattern);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies a directory
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="dest">
        /// The dest. 
        /// </param>
        /// <param name="subdirs">
        /// The subdirs. 
        /// </param>
        /// <param name="removeIfExists">
        /// The remove if exists. 
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// The source does not exist
        /// </exception>
        internal static void CopyDirectory(string source, string dest, bool subdirs, bool removeIfExists)
        {
            var dir = new DirectoryInfo(source);
            var dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);
            }

            // Don't copy excluded or directories that will be removed
            if (DirectoryExcludedOrInRemoveList(dir))
            {
                return;
            }

            // Removes the directory if it already exists
            if (removeIfExists)
            {
                Remove(dest);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();
            var excludedFiles = GetExcludedFiles(dir);

            foreach (var file in files.Except(excludedFiles))
            {
                // Create the path to the new copy of the file.
                var temppath = Path.Combine(dest, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If subdirs is true, copy the subdirectories.
            if (subdirs)
            {
                foreach (var subdir in dirs.Where(s => !DirectoryExcludedOrInRemoveList(s)))
                {
                    // Create the subdirectory.
                    var temppath = Path.Combine(dest, subdir.Name);

                    // Copy the subdirectories.
                    CopyDirectory(subdir.FullName, temppath, true, removeIfExists);
                }
            }
        }

        /// <summary>
        /// Removes a directory
        /// </summary>
        /// <param name="directory">
        /// The directory. 
        /// </param>
        /// <exception cref="ApplicationException">
        /// Error removing directory
        /// </exception>
        internal static void Remove(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    return;
                }

                Program.WriteVerboseMessage("Checking Directory {0}", directory);

                if (excludedDirs != null && excludedDirs.Contains(directory))
                {
                    Program.WriteVerboseMessage("Not Removing Excluded Directory {0}", directory);
                }
                else
                {
                    Program.WriteVerboseMessage("Removing {0}", directory);
                    FileHelper.DeleteFiles(directory);

                    // If the directory contains any excluded files, don't remove it.
                    if (Program.Options.ExcludeFiles != null
                        &&
                        Program.Options.ExcludeFiles.Any(
                            pattern => Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories).Any()))
                    {
                        Program.WriteVerboseMessage("Directory contains excluded files, not removing {0}", directory);
                        return;
                    }

                    Directory.Delete(directory, true);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                throw new ApplicationException(
                    string.Format("Error removing directory {0}: {1}", directory, exception.Message));
            }
            catch (IOException exception)
            {
                throw new ApplicationException(
                    string.Format("Error removing directory {0}: {1}", directory, exception.Message));
            }
        }

        /// <summary>
        /// Determines if a directory is excluded
        /// </summary>
        /// <param name="dir">
        /// The dir. 
        /// </param>
        /// <returns>
        /// true if the directory is excluded 
        /// </returns>
        private static bool DirectoryExcludedOrInRemoveList(DirectoryInfo dir)
        {
            // Don't copy directories that are excluded or to be removed
            return IsInList(excludedDirs, dir.FullName) || IsInList(removeDirs, dir.FullName);
        }

        /// <summary>
        /// Determines if a directory is excluded
        /// </summary>
        /// <param name="dir">
        /// The dir. 
        /// </param>
        /// <returns>
        /// true if the directory is excluded 
        /// </returns>
        private static bool DirectoryExcludedOrInRemoveList(string dir)
        {
            // Don't copy directories that are excluded or to be removed
            return excludedDirs.Contains(dir) || removeDirs.Contains(dir);
        }

        /// <summary>
        /// Gets a list of excluded files
        /// </summary>
        /// <param name="dir">
        /// The dir. 
        /// </param>
        /// <returns>
        /// The list of files 
        /// </returns>
        private static IEnumerable<FileInfo> GetExcludedFiles(DirectoryInfo dir)
        {
            var excludedFiles = new List<FileInfo>();
            if (Program.Options.ExcludeFiles != null)
            {
                foreach (var excludeFile in Program.Options.ExcludeFiles)
                {
                    excludedFiles.AddRange(dir.GetFiles(excludeFile));
                }
            }

            if (Program.Options.RemoveFiles != null)
            {
                foreach (var removeFile in Program.Options.RemoveFiles)
                {
                    excludedFiles.AddRange(dir.GetFiles(removeFile));
                }
            }

            return excludedFiles;
        }

        /// <summary>
        /// Determines if an item is in a list
        /// </summary>
        /// <param name="list">
        /// The list. 
        /// </param>
        /// <param name="s">
        /// The s. 
        /// </param>
        /// <returns>
        /// true if in the list 
        /// </returns>
        private static bool IsInList(ICollection<string> list, string s)
        {
            return list != null && list.Contains(s);
        }

        #endregion
    }
}