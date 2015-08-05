// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryCollectionTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Activities.Extensions.Statements;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for DictionaryCollectionTest and is intended
    ///   to contain all DictionaryCollectionTest Unit Tests
    /// </summary>
    [TestClass]
    public class DictionaryCollectionTest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with nothing in it
        ///   When
        ///   * The AddToDictionary activity is invoked with a string
        ///   Then
        ///   * A string should be added to the dictionary
        /// </summary>
        [TestMethod]
        public void AddToDictionaryShouldAddRefType()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string>();
            var activity = new AddToDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = ExpectedKey;
            input.Value = ExpectedValue;

            try
            {
                host.TestActivity(input);

                Assert.AreEqual(1, dictionary.Count);
                Assert.AreEqual(ExpectedValue, dictionary[ExpectedKey]);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, int) with nothing in it
        ///   When
        ///   * The AddToDictionary activity is invoked with an int
        ///   Then
        ///   * An int should be added to the dictionary
        /// </summary>
        [TestMethod]
        public void AddToDictionaryShouldAddValueType()
        {
            const string ExpectedKey = "key";
            const int ExpectedValue = 2;
            var dictionary = new Dictionary<string, int>();
            var activity = new AddToDictionary<string, int>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = ExpectedKey;
            input.Value = ExpectedValue;
            try
            {
                host.TestActivity(input);

                Assert.AreEqual(1, dictionary.Count);
                Assert.AreEqual(ExpectedValue, dictionary[ExpectedKey]);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The AddToDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void AddToNullDictionaryShouldThrow()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var activity = new AddToDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            input.Key = ExpectedKey;
            input.Value = ExpectedValue;
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, int) with 1 key value pair
        ///   When
        ///   * The ClearDictionary activity is invoked 
        ///   Then
        ///   * The dictionary should be cleared
        /// </summary>
        [TestMethod]
        public void ClearDictionaryShouldClear()
        {
            var dictionary = new Dictionary<string, int> { { "Key", 1 } };
            var activity = new ClearDictionary<string, int>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            try
            {
                host.TestActivity(input);

                Assert.AreEqual(0, dictionary.Count);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The ClearDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void ClearNullDictionaryShouldThrow()
        {
            var activity = new ClearDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with a key named "key"
        ///   When
        ///   * The KeyExistsInDictionary activity is invoked with a key argument "key"
        ///   Then
        ///   * The Result argument should be true
        /// </summary>
        [TestMethod]
        public void KeyExistsInDictionaryReturnTrueWhenKeyExists()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string> { { ExpectedKey, ExpectedValue } };
            var activity = new KeyExistsInDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = ExpectedKey;
            try
            {
                host.TestActivity(input);
                host.AssertOutArgument.IsTrue("Result");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The KeyExistsInDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void KeyExistsInNullDictionaryShouldThrow()
        {
            var activity = new KeyExistsInDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            input.Key = "key";
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }


        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with a key named "key"
        ///   When
        ///   * The ValueExistsInDictionary activity is invoked with a key argument "key"
        ///   Then
        ///   * The Result argument should be true
        /// </summary>
        [TestMethod]
        public void ValueExistsInDictionaryReturnTrueWhenValueExists()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string> { { ExpectedKey, ExpectedValue } };
            var activity = new ValueExistsInDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Value = ExpectedValue;
            try
            {
                host.TestActivity(input);
                host.AssertOutArgument.IsTrue("Result");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The ValueExistsInDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void ValueExistsInNullDictionaryShouldThrow()
        {
            var activity = new ValueExistsInDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            input.Value = "value";
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }


        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with a key named "key"
        ///   When
        ///   * The RemoveFromDictionary activity is invoked with a key argument "key"
        ///   Then
        ///   * The Result argument should be true
        /// </summary>
        [TestMethod]
        public void RemoveFromDictionaryReturnTrueWhenRemoved()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string> { { ExpectedKey, ExpectedValue } };
            var activity = new RemoveFromDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = ExpectedKey;
            try
            {
                host.TestActivity(input);
                host.AssertOutArgument.IsTrue("Result");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The RemoveFromDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void RemoveFromNullDictionaryShouldThrow()
        {
            var activity = new RemoveFromDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            input.Key = "key";
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with a key named "key"
        ///   When
        ///   * The GetFromDictionary activity is invoked with a key argument "key"
        ///   Then
        ///   * The Result argument should be true
        /// * The Out Argument should contain the ExpectedValue
        /// </summary>
        [TestMethod]
        public void GetFromDictionaryReturnTrueWhenGetWithGoodKey()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string> { { ExpectedKey, ExpectedValue } };
            var activity = new GetFromDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = ExpectedKey;
            try
            {
                host.TestActivity(input);
                host.AssertOutArgument.IsTrue("Result");
                host.AssertOutArgument.AreEqual("Value", ExpectedValue);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A Dictionary(Of string, string) with a key named "key"
        ///   When
        ///   * The GetFromDictionary activity is invoked with a key argument "key"
        ///   Then
        ///   * The Result argument should be true
        /// * The Out Argument should contain the ExpectedValue
        /// </summary>
        [TestMethod]
        public void GetFromDictionaryReturnFalseWhenGetWithBadKey()
        {
            const string ExpectedKey = "key";
            const string ExpectedValue = "value";
            var dictionary = new Dictionary<string, string> { { ExpectedKey, ExpectedValue } };
            var activity = new GetFromDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = dictionary;
            input.Key = "Bad Key";
            try
            {
                host.TestActivity(input);
                host.AssertOutArgument.IsFalse("Result");
                host.AssertOutArgument.IsNull("Value");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }


        /// <summary>
        /// Given
        ///   * A null dictionary
        ///   When
        ///   * The GetFromDictionary activity is invoked
        ///   Then
        ///   * An InvalidOperationException should be thrown
        /// </summary>
        [TestMethod]
        public void GetFromNullDictionaryShouldThrow()
        {
            var activity = new GetFromDictionary<string, string>();
            var host = new WorkflowInvokerTest(activity);
            dynamic input = new WorkflowArguments();
            input.Dictionary = null;
            input.Key = "key";
            try
            {
                AssertHelper.Throws<InvalidOperationException>(() => host.TestActivity(input));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}