// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlHelperTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Reflection;
    using System.ServiceModel.Activities;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using UsingTraceTrackingParticipant;

    /// <summary>
    ///   This is a test class for XamlHelperTest and is intended
    ///   to contain all XamlHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class XamlHelperTest
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadEmptyShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load(string.Empty));
        }

        /// <summary>
        ///   Given 
        ///   * A file name with no extension
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadNoExtShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load("NoExtension"));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadNullShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load(null));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWithLocalEmptyShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load(string.Empty, Assembly.GetExecutingAssembly()));
        }

        /// <summary>
        ///   Given 
        ///   * A file name with no extension
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWithLocalNoExtShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(
                () => XamlHelper.Load("NoExtension", Assembly.GetExecutingAssembly()));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWithLocalNullAssemblyShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load("test.xaml", null));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWithLocalNullShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load(null, Assembly.GetExecutingAssembly()));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWithLocalWrongExtShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load("file.ext", Assembly.GetExecutingAssembly()));
        }

        /// <summary>
        ///   Given 
        ///   * a XAML file
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * The activity is loaded
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.Workflow1Path)]
        public void LoadWithLocalXamlShouldLoad()
        {
            var actual = XamlHelper.Load(Constants.Workflow1Xaml, typeof(Workflow1).Assembly);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///   Given 
        ///   * a XAMLX file
        ///   When
        ///   * LoadWorkflowService is invoked
        ///   Then
        ///   * The WorkflowService is returned
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.DefaultServiceXamlxPath)]
        public void LoadWorkflowServiceShouldLoad()
        {
            var actual = XamlHelper.LoadWorkflowService(Constants.DefaultServiceXamlx);
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(WorkflowService));
        }

        /// <summary>
        ///   Given 
        ///   * a file name with an extension other than .xaml or .xamlx
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        public void LoadWrongExtShouldthrow()
        {
            AssertHelper.Throws<ArgumentException>(() => XamlHelper.Load("file.ext"));
        }

        /// <summary>
        ///   Given 
        ///   * a XAML file
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * The activity is loaded
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.Workflow1Path)]
        public void LoadXamlShouldLoad()
        {
            var actual = XamlHelper.Load(Constants.Workflow1Xaml);
            Assert.IsNotNull(actual);
        }
        
        /// <summary>
        ///   Given 
        ///   * a XAMLX file
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * The root activity is returned
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.DefaultServiceXamlxPath)]
        public void LoadXamlXShouldLoad()
        {
            var actual = XamlHelper.Load(Constants.DefaultServiceXamlx);
            Assert.IsNotNull(actual);
        }

        #endregion
    }
}