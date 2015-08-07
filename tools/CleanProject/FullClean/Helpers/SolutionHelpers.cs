using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace IcodeNet.Helpers
{
    public static class SolutionHelpers
    {
        private static Project GetActiveProject(DTE2 dte)
        {
            try
            {
                Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
                {
                    return activeSolutionProjects.GetValue(0) as Project;
                }
            }
            catch (Exception exception)
            {
                Debug.Write(exception.Message);
            }
            return null;
        }

        public static DTE2 GetDTE2()
        {
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }

        public static string GetRootFolder(DTE2 dte)
        {
            Project activeProject = SolutionHelpers.GetActiveProject(dte);
            if (activeProject == null)
            {
                return null;
            }
            string str = activeProject.Properties.Item("FullPath").Value.ToString();
            if (Directory.Exists(str))
            {
                return str;
            }
            return Path.GetDirectoryName(str);
        }

        public static string GetSourceFilePath()
        {
            string str;
            string str1;
            Array selectedItems = (Array)SolutionHelpers.GetDTE2().ToolWindows.SolutionExplorer.SelectedItems;
            if (selectedItems != null)
            {
                IEnumerator enumerator = selectedItems.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        ProjectItem obj = ((UIHierarchyItem)enumerator.Current).Object as ProjectItem;
                        str = (obj.Properties == null ? obj.Name : obj.Properties.Item("FullPath").Value.ToString());
                        str1 = str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                return str1;
            }
            return string.Empty;
        }
    }
}