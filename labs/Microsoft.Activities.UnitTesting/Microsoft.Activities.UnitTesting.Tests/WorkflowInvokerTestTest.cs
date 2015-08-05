#region copyright

//  ----------------------------------------------------------------------------------
//  Microsoft
//  
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//  ----------------------------------------------------------------------------------
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
//  ----------------------------------------------------------------------------------

#endregion

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.Activities.UnitTesting.Tests.MockActivities;
    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    ///<summary>
    ///  This is a test class for WorkflowInvokerTestTest and is intended
    ///  to contain all WorkflowInvokerTestTest Unit Tests
    ///</summary>
    [TestClass]
    public class WorkflowInvokerTestTest
    {
        #region Properties

        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        ///<summary>
        ///  A test for WorkflowInvokerTest Constructor
        ///</summary>
        [TestMethod]
        public void ActivityShouldReturnActivityUsedToConstruct()
        {
            Activity activity = new Sequence();
            var target = new WorkflowInvokerTest(activity);
            Assert.IsNotNull(target.Activity);
        }

        ///<summary>
        ///  A test for AssertOutArgument
        ///</summary>
        [TestMethod]
        public void AssertOutArgumentShouldNotBeNull()
        {
            Activity activity = new Sequence();
            var target = new WorkflowInvokerTest(activity);
            target.TestActivity();
            Assert.IsNotNull(target.AssertOutArgument);
        }

        ///<summary>
        ///  Verifies that the InArguments property can be used to pass values
        ///</summary>
        [TestMethod]
        public void InArgumentsShouldPassValues()
        {
            const string UserName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", UserName);
            var target = new WorkflowInvokerTest(new SayHello());
            target.InArguments.UserName = UserName;
            target.TestActivity();
            Assert.AreEqual(expectedGreeting, target.OutArguments.Greeting);
        }

        ///<summary>
        ///  Verifies that the InArguments property can be used to pass values
        ///</summary>
        [TestMethod]
        public void ArgumentsOverloadShouldPassViaStatic()
        {
            const string UserName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", UserName);
            dynamic arguments = new WorkflowArguments();
            arguments.UserName = UserName;
            var target = WorkflowInvokerTest.Create(new SayHello(), arguments);
            target.TestActivity();
            Assert.AreEqual(expectedGreeting, target.OutArguments.Greeting);
        }

        ///<summary>
        ///  A test for Invoker
        ///</summary>
        [TestMethod]
        public void InvokerShouldBeInstanceOfWorkflowInvoker()
        {
            Activity activity = new Sequence();
            var target = new WorkflowInvokerTest(activity);
            Assert.IsInstanceOfType(target.Invoker, typeof(WorkflowInvoker));
        }

        ///<summary>
        ///  Verifies that the OutArguments property is not null when there are no arguments
        ///</summary>
        [TestMethod]
        public void OutArgumentsShouldNotBeNull()
        {
            Activity activity = new Sequence();
            var target = new WorkflowInvokerTest(activity);
            target.TestActivity();
            Assert.IsNotNull(target.OutArguments);
        }

        ///<summary>
        ///  Verifies that accessing a dynamic OutArguments that does not exist will throw an exception
        ///</summary>
        [TestMethod]
        public void OutArgumentsShouldThrowWhenDoesNotExist()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            target.TestActivity();
            AssertHelper.Throws<RuntimeBinderException>(() => { var value = target.OutArguments.DoesNotExist; });
        }

        ///<summary>
        ///  Verifies that OutArguments return values when they exist
        ///</summary>
        [TestMethod]
        public void OutArgumentsShouldReturnValues()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            target.TestActivity();
            Assert.AreEqual(expectedGreeting, target.OutArguments.Greeting);
        }

        ///<summary>
        ///  Verifies that accessing the OutArguments property before the workflow has run will throw
        ///</summary>
        [TestMethod]
        public void OutArgumentsShouldThrowWhenNotSetYet()
        {
            const string userName = "Test User";
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            AssertHelper.Throws<InvalidOperationException>(() => { var value = target.OutArguments.Greeting; });
        }

        ///<summary>
        ///  A test for Output
        ///</summary>
        [TestMethod]
        public void OutputShouldContainIDictionaryWithKeyValues()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            target.TestActivity();

            Assert.IsInstanceOfType(target.Output, typeof(IDictionary<string, object>));
            AssertOutArgument.AreEqual(target.Output, "Greeting", expectedGreeting);
        }

        ///<summary>
        ///  Test to ensure that Output throws when not ready
        ///</summary>
        [TestMethod]
        public void OutputShouldThrowWhenNotSetYet()
        {
            const string userName = "Test User";
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            AssertHelper.Throws<InvalidOperationException>(() => { var value = target.Output["Greeting"]; });
        }

        ///<summary>
        ///  A test for Tracking
        ///</summary>
        [TestMethod]
        public void ShouldEnableTrackingWhenInvoked()
        {
            const string userName = "Test User";
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            target.TestActivity();

            Assert.IsInstanceOfType(target.Tracking, typeof(MemoryTrackingParticipant));
            Assert.IsTrue(target.Tracking.Records.Count() > 0);
        }

        ///<summary>
        ///  A test for TestActivity
        ///</summary>
        [TestMethod]
        public void TestActivityTest()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var target = new WorkflowInvokerTest(new SayHello { UserName = userName });
            var ouput = target.TestActivity();
            target.AssertOutArgument.AreEqual("Greeting", expectedGreeting);
        }

        ///<summary>
        ///  A test for TestActivity with passing extra arguments
        ///</summary>
        [TestMethod]
        public void TestActivityWithExtraArgsTest()
        {
            var activity = new TestActivityTakesArgs();
            var target = new WorkflowInvokerTest(activity);
            var args = new Dictionary<string, object> { { "Arg1", "ArgVal" }, { "Arg2", 5 } };
            target.TestActivity(args);

            Assert.AreEqual("ArgVal", activity.Arg1Processed);
            Assert.AreEqual(5, activity.Arg2Processed);
        }


        ///<summary>
        ///  Verifies that if you use both an input dictionary an InArguments you get an argument exception
        ///</summary>
        [TestMethod]
        public void InArgumentsWithDictionaryWillThrow()
        {
            var activity = new TestActivityTakesArgs();
            var target = new WorkflowInvokerTest(activity);
            var args = new Dictionary<string, object> { { "Arg1", "ArgVal" }, { "Arg2", 5 } };
            target.InArguments.Arg1 = "ArgValDynamic";
            AssertHelper.Throws<ArgumentException>(() => target.TestActivity(args));
        }

        ///<summary>
        ///  A test for TestActivity with passing no arguments when the activity expects arguments. Basically, the
        ///  default invoke should happen.
        ///</summary>
        [TestMethod]
        public void TestActivityWithoutExtraArgsTest()
        {
            var activity = new TestActivityTakesArgs();
            var target = new WorkflowInvokerTest(activity);
            target.TestActivity();

            Assert.IsNull(activity.Arg1Processed);
            Assert.AreEqual(0, activity.Arg2Processed);
        }

        #endregion
    }
}