// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Labs.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities
{
    /// <summary>
    ///   Public shared members
    /// </summary>
    public static class Labs
    {
        /// <summary>
        /// Path to the UnitTestingExamples folder
        /// </summary>
        public const string UnitTestingExamples = Dir + @"\Microsoft.Activities.UnitTesting\Examples\";

        public const string Dir = "%LABDIR%";

//#if NCRUNCH
//    /// <summary>
//    /// The Lab directory 
//    /// </summary>
//    /// <remarks>
//    /// NCrunch does not expand environment variables
//    /// </remarks>
//        public const string Dir = @"D:\WF\Labs";
//#else

//        /// <summary>
//        ///   The test projects folder.
//        /// </summary>
//        public const string Dir = "%LABDIR%";
//#endif
    }
}