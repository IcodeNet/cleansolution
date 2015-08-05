namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Expressions;
    using System.Activities.Statements;
    using System.Threading;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    ///<summary>
    ///  This is a test class for WorkflowArgumentsTest and is intended
    ///  to contain all WorkflowArgumentsTest Unit Tests
    ///</summary>
    [TestClass]
    public class WorkflowArgumentsTest
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
        ///  Verify that WorkflowArguments can be passed to a WorkflowApplication
        ///</summary>
        ///<remarks>
        ///  Given
        ///  * An activity with two input arguments and an out argument
        ///  When
        ///  * input arguments are created with a dynamic keyword
        ///  Then
        ///  * The WorkflowArguments type can be passed as the input to a workflow
        ///  * WorkflowArguments can receive the output of a workflow
        ///</remarks>
        [TestMethod]
        public void WorkflowArgumentsCanPassToWorkflowApplication()
        {
            // Arrange
            var activity = new ArgTest();
            dynamic input = new WorkflowArguments();
            dynamic output = null;
            var completed = new AutoResetEvent(false);
            // Act
            input.Num1 = 2;
            input.Num2 = 3;

            var host = new WorkflowApplication(activity, input);
            host.Completed += args =>
                {
                    output = WorkflowArguments.FromDictionary(args.Outputs);
                    completed.Set();
                };

            host.Run();

            // Wait for complete
            Assert.IsTrue(completed.WaitOne(1000));

            // Assert            
            Assert.AreEqual(5, output.Result);
        }

        ///<summary>
        ///  Verify that WorkflowArguments can be passed to a WorkflowInvoker
        ///</summary>
        ///<remarks>
        ///  Given
        ///  * An activity with two input arguments and an out argument
        ///  When
        ///  * input arguments are created with a dynamic keyword
        ///  Then
        ///  * The WorkflowArguments type can be passed as the input to a workflow
        ///  * WorkflowArguments can receive the output of a workflow
        ///</remarks>
        [TestMethod]
        public void WorkflowArgumentsCanPassToWorkflowInvoker()
        {
            // Arrange
            var activity = new ArgTest();
            dynamic input = new WorkflowArguments();

            // Act
            input.Num1 = 2;
            input.Num2 = 3;

            var output = WorkflowArguments.FromDictionary(WorkflowInvoker.Invoke(activity, input));

            // Assert            
            Assert.AreEqual(5, output.Result);
        }

        ///<summary>
        ///  Verify that WorkflowArguments cause an exception if invalid
        ///</summary>
        ///<remarks>
        ///  Given
        ///  * An activity with two input arguments and an out argument
        ///  When
        ///  * input arguments are created with a dynamic
        ///</remarks>
        [TestMethod]
        public void WorkflowArgumentsThrowIfInvalid()
        {
            // Arrange
            var activity = new ArgTest();
            dynamic input = new WorkflowArguments();

            // Act
            input.Num1 = 2;
            input.Num2 = 3;
            input.DoesNotExist = "This will cause an exception";

            // Assert            
            AssertHelper.Throws<ArgumentException>(() => WorkflowInvoker.Invoke(activity, input));
        }

        #endregion

        internal sealed class ArgTest : Activity
        {
            #region Constructors and Destructors

            internal ArgTest()
            {
                this.Result = new OutArgument<int>();

                this.Implementation =
                    () =>
                    new Sequence
                        {
                            Activities =
                                {
                                    new Assign<int>
                                        {
                                            To = new ArgumentReference<int> { ArgumentName = "Result" },
                                            Value = new InArgument<int>(ctx => this.Num1.Get(ctx) + this.Num2.Get(ctx))
                                        }
                                }
                        };
            }

            #endregion

            #region Properties

            public InArgument<int> Num1 { get; set; }

            public InArgument<int> Num2 { get; set; }

            public OutArgument<int> Result { get; set; }

            #endregion
        }
    }
}