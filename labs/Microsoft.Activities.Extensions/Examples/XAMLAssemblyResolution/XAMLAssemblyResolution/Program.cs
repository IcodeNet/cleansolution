// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XAMLAssemblyResolution
{
    using System;
    using System.Activities;
    using System.Activities.XamlIntegration;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xaml;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Public Methods

        /// <summary>
        /// Gets a list of referenced assemblies for WorkflowLoose.xaml
        /// </summary>
        /// <returns>
        /// a list of referenced assemblies for WorkflowLoose.xaml
        /// </returns>
        public static IList<string> GetWorkflowLooseReferencedAssemblies()
        {
            // Create a list of activities you want to reference here
            var list = new List<string> { "ActivityLibrary1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c18b97d2d48a43ab", };

            // Add the standard list of references
            list.AddRange(StrictXamlHelper.StandardCSharpReferencedAssemblies);
            return list;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The invoke compiled xaml.
        /// </summary>
        /// <param name="resolutionOption">
        /// The XAML resolution option
        /// </param>
        private static void InvokeCompiledXaml(XamlAssemblyResolutionOption resolutionOption)
        {
            Console.WriteLine();
            Console.WriteLine("Compiled XAML");

            try
            {
                // The partial class definition in WorkflowCompiled.cs provides an 
                // alternat ctor to support the resolution option
                WorkflowInvoker.Invoke(new WorkflowCompiled(resolutionOption));
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine("Could not find assembly {0}", fileNotFoundException.FileName);
            }
            catch (FileLoadException fileLoadException)
            {
                Console.WriteLine("Could not load assembly {0}", fileLoadException.FileName);
            }
        }

        /// <summary>
        /// The invoke loose xaml.
        /// </summary>
        /// <param name="resolutionOption">
        /// The XAML resolution option
        /// </param>
        private static void InvokeLooseXaml(XamlAssemblyResolutionOption resolutionOption)
        {
            Console.WriteLine();
            Console.WriteLine("Loose XAML");
            try
            {
                switch (resolutionOption)
                {
                    case XamlAssemblyResolutionOption.VersionIndependent:
                        WorkflowInvoker.Invoke(ActivityXamlServices.Load("WorkflowLoose.xaml"));
                        break;
                    case XamlAssemblyResolutionOption.FullName:

                        // This will ensure the correct assemblies are loaded prior to loading loose XAML
                        WorkflowInvoker.Invoke(
                            StrictXamlHelper.ActivityLoad(
                            "WorkflowLoose.xaml", 
                            GetWorkflowLooseReferencedAssemblies()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("resolutionOption");
                }
            }
            catch (XamlObjectWriterException ex)
            {
                Console.WriteLine("Error loading loose xaml {0}", ex.Message);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine("Could not find assembly {0}", fileNotFoundException.FileName);
            }
            catch (FileLoadException fileLoadException)
            {
                Console.WriteLine("Could not load assembly {0}", fileLoadException.FileName);
            }
        }

        /// <summary>
        /// The main method.
        /// </summary>
        private static void Main()
        {
            Console.Clear();
            Console.WriteLine("WF4 XAML Assembly Resolution example");
            MenuChoice menuChoice;

            do
            {
                Console.WriteLine("-------------------------------------------------------------------");
                switch (menuChoice = PromptEnum<MenuChoice>("Which workflow do you want to run first?"))
                {
                    case MenuChoice.Compiled:
                        RunCompiledThenLoose();
                        break;
                    case MenuChoice.Loose:
                        RunLooseThenCompiled();
                        break;
                }

                Console.WriteLine();
                if (menuChoice != MenuChoice.Quit)
                {
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey(true);
                }
            }
            while (menuChoice != MenuChoice.Quit);
        }

        /// <summary>
        /// The prompt enum.
        /// </summary>
        /// <param name="prompt">
        /// The prompt.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enum to prompt with
        /// </typeparam>
        /// <returns>
        /// The selected enum value
        /// </returns>
        /// <exception cref="InvalidEnumArgumentException">
        /// The argument is invalid
        /// </exception>
        private static T PromptEnum<T>(string prompt) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidEnumArgumentException();
            }

            var names = Enum.GetNames(typeof(T));
            var choice = -1;
            while (choice - 1 < 0 || choice > names.Length)
            {
                Console.WriteLine(prompt);
                for (var i = 0; i < names.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i + 1, names[i]);
                }

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(CultureInfo.InvariantCulture), out choice))
                {
                    choice = -1;
                }
            }

            return (T)Enum.GetValues(typeof(T)).GetValue(choice - 1);
        }

        /// <summary>
        /// The run compiled then loose.
        /// </summary>
        private static void RunCompiledThenLoose()
        {
            var resolutionOption =
                PromptEnum<XamlAssemblyResolutionOption>("What kind of assembly resolution do you want to use?");
            Console.Clear();
            ShowHost();
            InvokeCompiledXaml(resolutionOption);
            InvokeLooseXaml(resolutionOption);
        }

        /// <summary>
        /// The run loose then compiled.
        /// </summary>
        private static void RunLooseThenCompiled()
        {
            var resolutionOption =
                PromptEnum<XamlAssemblyResolutionOption>("What kind of assembly resolution do you want to use?");
            Console.Clear();
            ShowHost();
            InvokeLooseXaml(resolutionOption);
            InvokeCompiledXaml(resolutionOption);
        }

        /// <summary>
        /// The show host.
        /// </summary>
        private static void ShowHost()
        {
            Console.WriteLine("Host application version {0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }

        #endregion
    }
}