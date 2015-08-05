// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///   Constants for the Tests
    /// </summary>
    public static class Constants
    {
        #region Constants

        /// <summary>
        ///   Activity with Send and Receive Reply
        /// </summary>
        public const string ActivityWithSendAndReceiveReplyXaml = "ActivityWithSendAndReceiveReply.Xaml";

        /// <summary>
        ///   Activity with Send and Receive Reply
        /// </summary>
        public const string ActivityWithSendAndReceiveReplyXamlPath =
            TestActivitiesDir + ActivityWithSendAndReceiveReplyXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithReceiveAndSendReplyAndCorrXaml = "ActivityWithReceiveAndSendReplyAndCorr.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithReceiveAndSendReplyAndCorrXamlPath =
            TestActivitiesDir + ActivityWithReceiveAndSendReplyAndCorrXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithReceiveAndSendReplyXaml = "ActivityWithReceiveAndSendReply.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithReceiveAndSendReplyXamlPath =
            TestActivitiesDir + ActivityWithReceiveAndSendReplyXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithReceiveMessageXaml = "ActivityWithReceiveMessage.xaml";

        /// <summary>
        ///   Path to the xamlx
        /// </summary>
        internal const string ActivityWithReceiveMessageXamlPath =
            TestActivitiesDir + ActivityWithReceiveMessageXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithReceiveXaml = "ActivityWithReceive.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithReceiveXamlPath =
            TestActivitiesDir + ActivityWithReceiveXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithSendCatchCommExceptionXaml = "ActivityWithSendCatchCommException.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithSendCatchCommExceptionXamlPath =
            TestActivitiesDir + ActivityWithSendCatchCommExceptionXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithSendMessageContentXaml = "ActivityWithSendMessageContent.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithSendMessageContentXamlPath =
            TestActivitiesDir + ActivityWithSendMessageContentXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithSendParametersContentXaml = "ActivityWithSendParametersContent.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithSendParametersContentXamlPath =
            TestActivitiesDir + ActivityWithSendParametersContentXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithSendXaml = "ActivityWithSend.Xaml";

        /// <summary>
        ///   Path to the Xaml
        /// </summary>
        internal const string ActivityWithSendXamlPath =
            TestActivitiesDir + ActivityWithSendXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ActivityWithTwoReceivesXaml = "ActivityWithTwoReceives.xaml";

        /// <summary>
        ///   Path to the xamlx
        /// </summary>
        internal const string ActivityWithTwoReceivesXamlPath =
            TestActivitiesDir + ActivityWithTwoReceivesXaml;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string DefaultServiceXamlx = "DefaultService.xamlx";

        /// <summary>
        ///   Path to the xamlx
        /// </summary>
        internal const string DefaultServiceXamlxPath =
            TestActivitiesDir + DefaultServiceXamlx;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ServiceWithParallelThreeReceivesXamlx = "ServiceWithParallelThreeReceives.xamlx";

        /// <summary>
        ///   Path to the xamlx
        /// </summary>
        internal const string ServiceWithParallelThreeReceivesXamlxPath =
            TestActivitiesDir + ServiceWithParallelThreeReceivesXamlx;

        /// <summary>
        ///   Service with two receives and increment
        /// </summary>
        internal const string ServicewithtworeceivesandincrementXamlx = "ServiceWithTwoReceivesAndIncrement.xamlx";

        /// <summary>
        ///   Path to the xamlx
        /// </summary>
        internal const string ServicewithtworeceivesandincrementXamlxPath =
            TestActivitiesDir + ServicewithtworeceivesandincrementXamlx;

        /// <summary>
        ///   Directory of the Test Activiteis
        /// </summary>
        internal const string TestActivitiesDir =
            Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests.Activities\";

        /// <summary>
        ///   The unit testing examples folder
        /// </summary>
        internal const string UnitTestingExamples = @"\Microsoft.Activities.UnitTesting\Examples\";

        #endregion

        #region Static Fields

        /// <summary>
        ///   Test timeout
        /// </summary>
        internal static readonly TimeSpan Timeout = Debugger.IsAttached
                                                        ? TimeSpan.FromSeconds(60)
                                                        : TimeSpan.FromSeconds(1);

        #endregion

        /// <summary>
        ///   The default test database name
        /// </summary>
        internal const string DefaultTestDatabaseName = "TestDatabase";

        /// <summary>
        ///   The local SQL Express instance
        /// </summary>
        internal const string LocalSqlExpress = @"(local)\SQLExpress";
    }
}