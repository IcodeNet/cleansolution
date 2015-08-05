// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///   The constants.
    /// </summary>
    internal static class Constants
    {
#if NET40

    // <summary>
    // The .NET version.
    // <summary>
    // The net ver.
    // </summary>
        internal const string NetVer = "NET40";
#elif NET401

        // <summary>
        /// <summary>
        /// </summary>
        internal const string NetVer = "NET401";

#elif NET45
    
    
    
    
    // <summary>
    // The net ver.
    // </summary>
        internal const string NetVer = "NET45";
#endif

#if DEBUG

        /// <summary>
        ///   The build configuration.
        /// </summary>
        internal const string Configuration = "Debug";
#else // release folder
        internal const string Configuration = "Release";
#endif

        /// <summary>
        ///   The bin folder.
        /// </summary>
        internal const string BinFolder = @"bin\" + Configuration + @"\" + NetVer;

        /// <summary>
        ///   The test projects folder.
        /// </summary>
        internal const string TestProjects = Labs.Dir + @"\Microsoft.Activities.Extensions\Tests\Test Projects\";

        /// <summary>
        ///   The activity v 1.
        /// </summary>
        internal const string ActivityV1 = TestProjects + @"ActivityLibrary.V1\" + BinFolder + @"\ActivityLibrary.dll";

        /// <summary>
        ///   The activity v 1 unsigned.
        /// </summary>
        internal const string ActivityV1Unsigned =
            TestProjects + @"ActivityLibrary.V1.Unsigned\" + BinFolder + @"\ActivityLibrary.dll";

        /// <summary>
        ///   The activity v 2.
        /// </summary>
        internal const string ActivityV2 = TestProjects + @"ActivityLibrary.V2\" + BinFolder + @"\ActivityLibrary.dll";

        /// <summary>
        ///   The test assembly.
        /// </summary>
        internal const string TestProjectDir =
            Labs.Dir + @"\Microsoft.Activities.Extensions\Microsoft.Activities.Extensions.Tests\";

        /// <summary>
        ///   The default service.
        /// </summary>
        internal const string DefaultServiceXamlx = "DefaultService.xamlx";

        /// <summary>
        ///   The default service path.
        /// </summary>
        internal const string DefaultServiceXamlxPath = TestProjectDir + "DefaultService.xamlx";

        /// <summary>
        ///   The test assembly.
        /// </summary>
        internal const string TestAssembly =
            Labs.Dir + @"\Microsoft.Activities.Extensions\Microsoft.Activities.Extensions.Tests\" + BinFolder
            + @"\Microsoft.Activities.Extensions.Tests.dll";

        /// <summary>
        ///   The workflow v 1.
        /// </summary>
        internal const string WorkflowV1 = TestProjects + @"WorkflowLibrary.V1\" + BinFolder + @"\WorkflowLibrary.dll";

        /// <summary>
        ///   The workflow v 1 activity v 1.
        /// </summary>
        internal const string WorkflowV1ActivityV1 = "WorkflowV1ActivityV1";

        /// <summary>
        ///   Deploy Directory with Workflow (V1) Activity (V1 unsigned)
        /// </summary>
        internal const string WorkflowV1ActivityV1Unsigned = "WorkflowV1ActivityV1Unsigned";

        /// <summary>
        ///   Deploy Directory with Workflow (V1) Activity (V2)
        /// </summary>
        internal const string WorkflowV1ActivityV2 = "WorkflowV1ActivityV2";

        /// <summary>
        ///   The Activity Library xaml.
        /// </summary>
        internal const string WorkflowXaml = TestProjects + @"WorkflowLibrary.V1\Workflow.xaml";

        /// <summary>
        ///   Deploy Directory with Workflow xaml and Activity (V1)
        /// </summary>
        internal const string WorkflowXamlActivityV1 = "WorkflowXamlActivityV1";

        /// <summary>
        ///   Deploy Directory with Workflow xaml and Activity (V2)
        /// </summary>
        internal const string WorkflowXamlActivityV2 = "WorkflowXamlActivityV2";

        /// <summary>
        ///   The sample Workflow1Xaml
        /// </summary>
        internal const string Workflow1Xaml = "Workflow1.xaml";

        /// <summary>
        ///   Folder for the UsingTraceTrackingParticipant1 example
        /// </summary>
        internal const string UsingTraceTrackingParticipant1Dir =
            Labs.Dir + @"\Microsoft.Activities.Extensions\Examples\UsingTraceTrackingParticipant\";

        /// <summary>
        ///   Folder for the UsingTraceTrackingParticipant1 example
        /// </summary>
        internal const string Workflow1Path = UsingTraceTrackingParticipant1Dir + Workflow1Xaml;

#if NET401_OR_GREATER
        /// <summary>
        ///   The sample WorkflowService
        /// </summary>
        internal const string StateMachineServiceExampleXamlx = "StateMachineServiceExample.xamlx";

        /// <summary>
        ///   The sample StateMachineXaml
        /// </summary>
        internal const string StateMachineExampleXaml = "StateMachineExample.xaml";

        /// <summary>
        ///   The sample StateMachineXaml
        /// </summary>
        internal const string StateMachineExample = "StateMachineExample";

        /// <summary>
        ///   Folder for the TrackingStateMachine example
        /// </summary>
        internal const string TrackingStateMachineDir =
            Labs.Dir + @"\Microsoft.Activities.Extensions\Examples\TrackingStateMachine\TrackingStateMachine.Activities\";

        /// <summary>
        ///   Folder for the TrackingStateMachine example
        /// </summary>
        internal const string StateMachineExamplePath = TrackingStateMachineDir + StateMachineExampleXaml;

        /// <summary>
        ///   Path to the StateMachineService
        /// </summary>
        internal const string StateMachineServiceExamplePath =
            TrackingStateMachineServiceDir + StateMachineServiceExampleXamlx;

        /// <summary>
        ///   Folder for the TrackingStateMachineService example
        /// </summary>
        internal const string TrackingStateMachineServiceDir =
            Labs.Dir + @"\Microsoft.Activities.Extensions\Examples\TrackingStateMachineService\";
#endif

        /// <summary>
        /// StateMachine display name
        /// </summary>
        internal const string StateMachine = "StateMachine";

        /// <summary>
        /// Name TestSource
        /// </summary>
        internal const string TestSource = "TestSource";

        /// <summary>
        ///   Test timeout
        /// </summary>
        internal static readonly TimeSpan Timeout = Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(3);

        /// <summary>
        /// A short timeout
        /// </summary>
        internal static readonly TimeSpan ShortTimeout = Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromMilliseconds(10);
    }
}