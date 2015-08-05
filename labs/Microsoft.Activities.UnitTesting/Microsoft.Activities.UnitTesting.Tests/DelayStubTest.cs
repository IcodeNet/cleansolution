using Microsoft.Activities.UnitTesting.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Diagnostics;

    /// <summary>
    ///This is a test class for DelayStubTest and is intended
    ///to contain all DelayStubTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DelayStubTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Verifies the DelayStub does not delay longer than the duration set
        ///</summary>
        /// <remarks>
        /// Given
        /// * A workflow with a DelayStub activity
        /// When
        /// * The workflow is invoked
        /// Then
        /// * The workflow does not delay longer than the DelayStub.Duration time
        /// </remarks>
        [TestMethod()]
        public void DelayStubShouldNotDelayLongerThanDuration()
        {
            var host = WorkflowInvokerTest.Create(new DelayStub());
            try
            {
                const int ExpectedDuration = 01;
                DelayStub.StubDuration = TimeSpan.FromMilliseconds(ExpectedDuration);
                var sw = new Stopwatch();
                sw.Start();
                host.TestActivity();
                sw.Stop();
                Assert.IsTrue(sw.ElapsedMilliseconds >= ExpectedDuration, string.Format("Expected delay {0}, actual delay {1}", ExpectedDuration, sw.ElapsedMilliseconds));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }
    }
}
