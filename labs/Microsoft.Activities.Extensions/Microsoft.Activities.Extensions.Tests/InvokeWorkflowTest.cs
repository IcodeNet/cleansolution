// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeWorkflowTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Activities.Extensions.Statements;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The invoke workflow test.
    /// </summary>
    [TestClass]
    public class InvokeWorkflowTest
    {
        #region Constants

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition which uses activities that require two extensions 
        ///   * An in argument named "Num" will be passed with the value of 3 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * the first child activity can increment Num and then store the value in extension 1 
        ///   * the second child activity can decrement Num and store the value in extension 2 
        ///   Then 
        ///   * The workflow definition is run with WorkflowInvoker.Invoke 
        ///   * Extension1.Value will be 4 
        ///   * Extension2.Value will be 2
        /// </summary>
        [TestMethod]
        public void InvokeWithExtensions()
        {
            var childActivity = new TestActivityWithExtensions();
            var target = new InvokeWorkflow();
            var incStore = new IncrementStore();
            var decStore = new DecrementStore();

            var extensions = new object[] { incStore, decStore };

            dynamic innerInput = new WorkflowArguments();
            innerInput.Num = 3;

            dynamic input = new WorkflowArguments();
            input.Activity = childActivity;
            input.Input = innerInput;
            input.Extensions = extensions;

            // inner dictionary gets passed to child
            var host = WorkflowInvokerTest.Create(target);

            try
            {
                host.TestActivity(input);
                Assert.AreEqual(4, incStore.Value);
                Assert.AreEqual(2, decStore.Value);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition 
        ///   * with an InArgument(Of int) named "Num" 
        ///   * and an Assign activity that will assign Result = Num + 1 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * and inner arguments which contain an input "Num" = 1 
        ///   Then 
        ///   * The workflow definition is run with WorkflowInvoker.Invoke 
        ///   * and Result will be 2
        /// </summary>
        [TestMethod]
        public void InvokeWorkflowShouldInvokeInner()
        {
            WorkflowTestTrace.Arrange();
            var childActivity = new TestInnerActivity();
            var target = new InvokeWorkflow();

            // inner dictionary gets passed to child
            var innerInput = InputDictionary.Create("Num", 1);
            var input = InputDictionary.Create("Activity", childActivity, "Input", innerInput);

            var host = WorkflowApplicationTest.Create(target, input);

            try
            {
                WorkflowTestTrace.Act();
                host.TestWorkflowApplication.RunEpisode();
                host.AssertOutArgument.IsNotNull("Result");
                var innerArgs = OutputDictionary.Get<IDictionary<string, object>>(host.Results.Output, "Result");

                WorkflowTestTrace.Assert();
                AssertOutArgument.AreEqual(innerArgs, "Result", 2);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition 
        ///   * with an InArgument(Of int) named "Num" 
        ///   * and an If activity that will throw an exception if Num is negative 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * and inner arguments which contain an input "Num" = -1 
        ///   Then 
        ///   * The InvokeWorkflow activity will run the workflow with WorkflowInvoker.Invoke 
        ///   * and will throw an exception to the outer workflow
        /// </summary>
        [TestMethod]
        public void InvokeWorkflowShouldThrow()
        {
            var childActivity = new TestInnerActivity();
            var target = new InvokeWorkflow();

            // inner dictionary gets passed to child
            var innerInput = InputDictionary.Create("Num", -1);
            var input = InputDictionary.Create("Activity", childActivity, "Input", innerInput);

            var host = WorkflowApplicationTest.Create(target, input);

            try
            {
                AssertHelper.Throws<ArgumentException>(() => host.TestWorkflowApplication.RunEpisode());
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition 
        ///   * with an InArgument(Of TimeSpan) named DelayTime 
        ///   * and a Delay activity that will Delay for DelayTime 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * and inner arguments which contain an input "DelayTime" = TimeSpan.FromSeconds=30 
        ///   * and the Timeout for the InvokeWorkflow activity set to 10 ms 
        ///   Then 
        ///   * The InvokeWorkflow activity will run the workflow with WorkflowInvoker.Invoke 
        ///   * which will throw a TimeoutException
        /// </summary>
        [TestMethod]
        public void InvokeWorkflowShouldThrowTimeoutException()
        {
            var childActivity = new TestTimeoutActivity();
            var target = new InvokeWorkflow();

            // inner dictionary gets passed to child
            var innerInput = InputDictionary.Create("DelayTime", TimeSpan.FromSeconds(30));
            var input = InputDictionary.Create("Activity", childActivity, "Input", innerInput);

            var host = WorkflowApplicationTest.Create(target, input);

            try
            {
                AssertHelper.Throws<TimeoutException>(
                    () => host.TestWorkflowApplication.RunEpisode(TimeSpan.FromMilliseconds(100)));
            }
            finally
            {
                host.Tracking.Trace(TrackingOption.All);
            }
        }

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition in a deployed XAML file 
        ///   * with an InArgument(Of int) named "Num" 
        ///   * and an Assign activity that will assign Result = Num + 1 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * and inner arguments which contain an input "Num" = 1 
        ///   Then 
        ///   * The workflow definition is run with WorkflowInvoker.Invoke 
        ///   * and Result will be 2
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestProjectDir + @"AddToNumOrThrow.xaml")]
        public void LoadAndInvokeShouldRun()
        {
            var target = new LoadAndRun();

            // inner dictionary gets passed to child
            var innerInput = InputDictionary.Create("Num", 1);
            var input = InputDictionary.Create("innerInputArguments", innerInput);

            var host = WorkflowApplicationTest.Create(target, input);

            try
            {
                host.TestWorkflowApplication.RunEpisode();
                host.AssertOutArgument.IsNotNull("innerOutput");
                var innerArgs = OutputDictionary.Get<IDictionary<string, object>>(host.Results.Output, "innerOutput");

                AssertOutArgument.AreEqual(innerArgs, "Result", 2);
            }
            finally
            {
                host.Tracking.Trace(TrackingOption.All);
            }
        }

        /// <summary>
        ///   Given 
        ///   * A inner workflow definition in a deployed XAML file 
        ///   * with an InArgument(Of int) named "Num" 
        ///   * and an Assign activity that will assign Result = Num + 1 
        ///   When 
        ///   * the InvokeWorkflow activity is executed 
        ///   * with the inner workflow definition 
        ///   * and inner arguments which contain an input "Num" = 1 
        ///   Then 
        ///   * The workflow definition is run with WorkflowInvoker.Invoke 
        ///   * and Result will be 2
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.Extensions\Microsoft.Activities.Extensions.Tests\" + @"AddToNumOrThrow.xaml")]
        public void LoadAndInvokeWorkflowShouldRun()
        {
            var target = new LoadAndInvokeWorkflow();

            // inner dictionary gets passed to child
            var innerInput = InputDictionary.Create("Num", 1);
            var input = InputDictionary.Create("Input", innerInput, "Path", "AddToNumOrThrow.xaml");

            var host = WorkflowApplicationTest.Create(target, input);

            try
            {
                host.TestWorkflowApplication.RunEpisode();
                host.AssertOutArgument.IsNotNull("Result");
                var innerArgs = OutputDictionary.Get<IDictionary<string, object>>(host.Results.Output, "Result");

                AssertOutArgument.AreEqual(innerArgs, "Result", 2);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A XAML workflow which uses activities with extensions
        ///   When
        ///   * The LoadAndInvokeWorkflow is called with extensions
        ///   Then
        ///   * The extensions are passed through to the child workflow
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.Extensions\Microsoft.Activities.Extensions.Tests\" + @"ChildWithExtensions.xaml")]
        public void LoadAndInvokeWorkflowWithExtensions()
        {
            var target = new LoadAndInvokeWorkflow();
            var incStore = new IncrementStore();
            var decStore = new DecrementStore();

            var extensions = new object[] { incStore, decStore };

            dynamic innerInput = new WorkflowArguments();
            innerInput.Num = 3;

            dynamic input = new WorkflowArguments();
            input.Input = innerInput;
            input.Path = "ChildWithExtensions.xaml";
            input.Extensions = extensions;
            input.LocalAssembly = Assembly.GetExecutingAssembly();

            var host = WorkflowInvokerTest.Create(target);

            try
            {
                host.TestActivity(input);
                Assert.AreEqual(4, incStore.Value);
                Assert.AreEqual(2, decStore.Value);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A custom activity in an activity library 
        ///   * A XAML activity from the same activity library referenced with the local: prefix 
        ///   When 
        ///   * LoadAssembly is invoked to load the assembly 
        ///   * LoadActivity is invoked with the LocalAssembly argument 
        ///   Then 
        ///   * The XAML activity should invoke the custom activity with the local: prefix
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestProjects + @"DynamicLoad\TestNum.xaml")]
        [DeploymentItem(Constants.TestProjects + @"DynamicLoad\" + Constants.BinFolder + @"\DynamicLoad.dll")]
        public void LocalAssemblyShouldOverride()
        {
            // Arrange
            var activity = new LoadAndInvokeWorkflow();
            var host = WorkflowInvokerTest.Create(activity);

            try
            {
                // Act
                // Setup the arguments for the TestNum.xaml workflow
                dynamic innerArgs = new WorkflowArguments();
                innerArgs.Num = 2;

                // Load the assembly to override the local
                var assembly = Assembly.LoadFrom("DynamicLoad.dll");

                // Setup the arguments to the LoadAndInvokeWorkflow
                dynamic arguments = new WorkflowArguments();
                arguments.LocalAssembly = assembly;
                arguments.Input = innerArgs;
                arguments.Path = "TestNum.xaml";

                host.TestActivity(arguments);

                // Assert
                host.AssertOutArgument.IsNotNull("Result");
                var innerOutArgs = WorkflowArguments.FromDictionary(host.OutArguments.Result);
                Assert.AreEqual(3, innerOutArgs.Num);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A custom activity in an activity library 
        ///   * A XAML activity from a different activity library 
        ///   When 
        ///   * The dependent assembly is loaded 
        ///   * LoadAndInvokeWorkflow is run 
        ///   Then 
        ///   * The XAML activity should invoke the custom activity
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestProjects + @"DynamicLoad\SubtractTest.xaml")]
        [DeploymentItem(Constants.TestProjects + @"MathActivities\" + Constants.BinFolder + @"\MathActivities.dll")]
        public void XamlActivityCanLoadAssemblyAndInvokeWorkflow()
        {
            // Arrange
            var activity = new TestLoadAndInvokeDependentAssembly();
            var host = WorkflowInvokerTest.Create(activity);

            try
            {
                // Act
                dynamic arguments = new WorkflowArguments();
                arguments.Num = 2;

                host.TestActivity(arguments);

                // Assert
                host.AssertOutArgument.AreEqual("Num", 1);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}