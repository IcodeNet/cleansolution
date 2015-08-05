// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LabsAssemblyInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(AssemblyInfo.Title + " " + Globals.Configuration + " " + Globals.NetVersionTitle)]
[assembly: AssemblyProduct(AssemblyInfo.Title)]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyConfiguration(Globals.Configuration + " " + Globals.NetVersionTitle)]
[assembly: ComVisible(false)]

/// <summary>
/// Provides constants for assembly attributes
/// </summary>
internal class Globals
{
#if DEBUG
    internal const string Configuration = "Debug";
#else
    internal const string Configuration = "Release";
#endif

#if NET40
    internal const string NetVersionTitle = ".NET V4.0";
    internal const string NetVersion = "NET40";
#elif NET401
    internal const string NetVersionTitle = ".NET V4.0.1";
    internal const string NetVersion = "NET401";
#elif NET45
    internal const string NetVersionTitle = ".NET V4.5";
    internal const string NetVersion = "NET45";
#endif
}


