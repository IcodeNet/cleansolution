// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInvokerTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Properties;
    using Microsoft.Activities.UnitTesting.Tracking;

    /// <summary>
    /// The workflow invoker test.
    /// </summary>
    public class WorkflowInvokerTest
    {
        #region Constants and Fields

        /// <summary>
        ///   The activity.
        /// </summary>
        private readonly Activity activity;

        /// <summary>
        /// The default timeout.
        /// </summary>
        private TimeSpan defaultTimeout = Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(1);

        /// <summary>
        ///   The invoker.
        /// </summary>
        private WorkflowInvoker invoker;

        /// <summary>
        ///   The out argument.
        /// </summary>
        private AssertOutput outArgument;

        /// <summary>
        ///   The output.
        /// </summary>
        private IDictionary<string, object> output;

        /// <summary>
        ///   Captures lines of text written during the test run.
        /// </summary>
        private string[] textLines;

        /// <summary>
        ///   The tracking.
        /// </summary>
        private MemoryTrackingParticipant tracking;

        /// <summary>
        ///   The length or the writer.
        /// </summary>
        private int writerLength;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "WorkflowInvokerTest" /> class.
        /// </summary>
        public WorkflowInvokerTest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInvokerTest"/> class.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        public WorkflowInvokerTest(Activity activity)
        {
            this.activity = activity;
            this.Writer = new StringWriter();
            this.Invoker.Extensions.Add(this.Writer);
            this.InArguments = new WorkflowArguments();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInvokerTest"/> class.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <param name="arguments">A dictionary of arguments </param>
        public WorkflowInvokerTest(Activity activity, IDictionary<string, object> arguments)
        {
            this.activity = activity;
            this.Writer = new StringWriter();
            this.Invoker.Extensions.Add(this.Writer);
            this.InArguments = arguments;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Activity.
        /// </summary>
        public Activity Activity
        {
            get
            {
                return this.activity;
            }
        }

        /// <summary>
        ///   Gets AssertOutArgument.
        /// </summary>
        public AssertOutput AssertOutArgument
        {
            get
            {
                return this.outArgument ?? (this.outArgument = new AssertOutput(this.Output));
            }
        }

        /// <summary>
        ///   Gets or sets the default timeout.
        /// </summary>
        public TimeSpan DefaultTimeout
        {
            get
            {
                return this.defaultTimeout;
            }

            set
            {
                this.defaultTimeout = value;
            }
        }

        /// <summary>
        ///   Gets the Extensions manager
        /// </summary>
        public WorkflowInstanceExtensionManager Extensions
        {
            get
            {
                return this.Invoker.Extensions;
            }
        }

        /// <summary>
        ///   Gets or sets A dynamic object for setting the In arguments of the activity
        /// </summary>
        public dynamic InArguments { get; set; }

        /// <summary>
        ///   Gets Invoker.
        /// </summary>
        public WorkflowInvoker Invoker
        {
            get
            {
                return this.invoker ?? this.CreateInvoker();
            }
        }

        /// <summary>
        ///   Gets OutArguments from the hosted workflow.
        /// </summary>
        public dynamic OutArguments
        {
            get
            {
                this.CheckOutput();
                return WorkflowArguments.FromDictionary(this.Output);
            }
        }

        /// <summary>
        ///   Gets Output.
        /// </summary>
        /// <exception cref = "InvalidOperationException">
        ///   The output dictionary is not set yet
        /// </exception>
        public IDictionary<string, object> Output
        {
            get
            {
                this.CheckOutput();

                return this.output;
            }
        }

        /// <summary>
        ///   Gets TextLines.
        /// </summary>
        public string[] TextLines
        {
            get
            {
                var strWriter = this.Writer.ToString();

                // If the buffer has changed, split it
                if (this.writerLength != strWriter.Length)
                {
                    this.writerLength = strWriter.Length;
                    this.textLines = Regex.Split(strWriter, Environment.NewLine);
                }

                return this.textLines;
            }
        }

        /// <summary>
        ///   Gets Tracking.
        /// </summary>
        public MemoryTrackingParticipant Tracking
        {
            get
            {
                return this.tracking ?? (this.tracking = new MemoryTrackingParticipant());
            }
        }

        /// <summary>
        ///   Gets or sets Writer.
        /// </summary>
        public StringWriter Writer { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowInvokerTest"/> class.
        /// </summary>
        /// <param name="activity">
        /// The activity
        /// </param>
        /// <returns>
        /// A <see cref="WorkflowInvokerTest"/> instance
        /// </returns>
        public static WorkflowInvokerTest Create(Activity activity)
        {
            return new WorkflowInvokerTest(activity);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowInvokerTest"/> class.
        /// </summary>
        /// <param name="activity">
        /// The activity
        /// </param>
        /// <param name="arguments">A dictionary of arguments</param>
        /// <returns>
        /// A <see cref="WorkflowInvokerTest"/> instance
        /// </returns>
        public static WorkflowInvokerTest Create(Activity activity, IDictionary<string, object> arguments )
        {
            return new WorkflowInvokerTest(activity, arguments);
        }

        /// <summary>
        /// The test activity.
        /// </summary>
        /// <returns>
        /// The output dictionary
        /// </returns>
        public IDictionary<string, object> TestActivity()
        {
            return this.TestActivity(this.DefaultTimeout);
        }

        /// <summary>
        /// The test activity.
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The output dictionary
        /// </returns>
        public IDictionary<string, object> TestActivity(TimeSpan timeout)
        {
            this.output = this.Invoker.Invoke(this.InArguments, timeout);

            return this.output;
        }

        /// <summary>
        /// Execute the activity, passing in custom arguments.
        /// </summary>
        /// <param name="arguments">
        /// The input arguments
        /// </param>
        /// <returns>
        /// The output dictionary
        /// </returns>
        public IDictionary<string, object> TestActivity(IDictionary<string, object> arguments)
        {
            return this.TestActivity(arguments, this.DefaultTimeout);
        }

        /// <summary>
        /// Execute the activity, passing in custom arguments.
        /// </summary>
        /// <param name="arguments">
        /// The input arguments
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The output dictionary
        /// </returns>
        public IDictionary<string, object> TestActivity(IDictionary<string, object> arguments, TimeSpan timeout)
        {
            var wa = (WorkflowArguments)this.InArguments;
            if (wa.Count > 0)
            {
                throw new ArgumentException(Resources.parameter_arguments, Resources.If_using_InArguments_you_should_not_pass_arguments_in_a_dictionary);
            }

            this.output = arguments == null ? this.Invoker.Invoke(timeout) : this.Invoker.Invoke(arguments, timeout);

            return this.output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The check output.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Output has not been set yet
        /// </exception>
        private void CheckOutput()
        {
            if (this.output == null)
            {
                throw new InvalidOperationException("Output not set yet");
            }
        }

        /// <summary>
        /// Creates the WorkflowInvoker
        /// </summary>
        /// <returns>
        /// The workflow invoker
        /// </returns>
        private WorkflowInvoker CreateInvoker()
        {
            this.invoker = new WorkflowInvoker(this.Activity);
            this.invoker.Extensions.Add(this.Tracking);
            return this.invoker;
        }

        #endregion
    }
}