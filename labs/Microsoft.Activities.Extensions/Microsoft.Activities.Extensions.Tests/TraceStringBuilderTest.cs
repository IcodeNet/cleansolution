// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceStringBuilderTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The trace string builder test.
    /// </summary>
    [TestClass]
    public class TraceStringBuilderTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * Append is invoked
        ///   Then
        ///   * The string will be appended
        /// </summary>
        [TestMethod]
        public void AppendAppendsText()
        {
            // Arrange
            const string Expected = "test";

            var tsb = new TraceStringBuilder();

            // Act
            tsb.Append(Expected);
            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder constructed with a tabs count of 2
        ///   When
        ///   * Append is invoked with a value of "test"
        ///   Then
        ///   * The string returned will be "\t\ttest"
        /// </summary>
        [TestMethod]
        public void AppendFormatWithArgsAndTabsAppendsText()
        {
            // Arrange
            const string Expected = "test";
            var tsb = new TraceStringBuilder(2);

            // Act
            tsb.Append(Expected);
            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual("\t\t" + Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * AppendFormat is invoked with a format string and parameter
        ///   Then
        ///   * The string will be appended formatted with the parameter
        /// </summary>
        [TestMethod]
        public void AppendFormatWithArgsAppendsText()
        {
            // Arrange
            const string Expected = "test";
            const string Format = Expected + " {0}";
            var tsb = new TraceStringBuilder();

            // Act
            tsb.AppendFormat(Format, 1);
            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected + " 1", actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * AppendFormat is invoked with a null format string 
        ///   Then
        ///   * ArgumentNullException is thrown with the name "format"
        /// </summary>
        [TestMethod]
        public void AppendFormatWithNullFormatThrowsArgumentNull()
        {
            const string Expected = @"Value cannot be null.
Parameter name: format";

            // Arrange
            var tsb = new TraceStringBuilder();

            // Act / Assert
            AssertHelper.Throws<ArgumentNullException>(() => tsb.AppendFormat(null), Expected);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * AppendLine is invoked with a null format string 
        ///   Then
        ///   * ArgumentNullException is thrown with the name "format"
        /// </summary>
        [TestMethod]
        public void AppendLineWithNullFormatThrowsArgumentNull()
        {
            const string Expected = @"Value cannot be null.
Parameter name: format";

            // Arrange
            var tsb = new TraceStringBuilder();

            // Act / Assert
            AssertHelper.Throws<ArgumentNullException>(() => tsb.AppendLine(null), Expected);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * AppendLine is invoked
        ///   Then
        ///   * The string will be appended
        ///   * The result will end with a newline
        /// </summary>
        [TestMethod]
        public void AppendLineWithoutArgsAppendsText()
        {
            // Arrange
            const string Expected = "test";

            var tsb = new TraceStringBuilder();

            // Act
            tsb.AppendLine(Expected);
            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected + Environment.NewLine, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder 
        ///   * A Property which implements ITracable
        ///   When
        ///   * Append is invoked with a value of "test"
        ///   Then
        ///   * The string returned will be "\t\ttest"
        /// </summary>
        [TestMethod]
        public void AppendPropertyAppendsTraceable()
        {
            // Arrange
            const string Expected = @"	Test
	Prop: TestTracable
	Children
	{
		ChildTraceable
		{
		}
	
		ChildTraceable
		{
			Strings
			{
				0
			}
		}
	
		ChildTraceable
		{
			Strings
			{
				0
				1
			}
		}
	
		ChildTraceable
		{
			Strings
			{
				0
				1
				2
			}
		}
	
	}
";

            var tsb = new TraceStringBuilder(1);
            var prop = new TestTracable();

            // Act
            tsb.AppendLine("Test");
            tsb.AppendProperty("Prop", prop);
            var actual = tsb.ToString();
            Trace.WriteLine(actual);

            // Assert
            Assert.AreEqual(Expected, actual, this.StringsDifferAt(Expected, actual));
        }

        [TestMethod]
        public void AppendPropertyIndentsNewLines()
        {
            // Arrange
            const string Expected = @"	Test
	MultiLine: Line 1
	Line 2
	Line 3
";
            const string MultiLine = @"Line 1
Line 2
Line 3";

            var tsb = new TraceStringBuilder(1);
            
            // Act
            tsb.AppendLine("Test");
            tsb.AppendProperty("MultiLine", MultiLine);
            var actual = tsb.ToString();
            Trace.WriteLine(actual);

            // Assert
            Assert.AreEqual(Expected, actual, this.StringsDifferAt(Expected, actual));
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder("Test")
        ///   When
        ///   * CloseBrace is set to "{"
        ///   Then
        ///   * An open brace will will be escaped to use "{{" when formatting
        /// </summary>
        [TestMethod]
        public void CloseBraceEscapesCurlyBrace()
        {
            // Arrange
            const string Expected = @"Test{
}
";

            // Act
            var tsb = new TraceStringBuilder("Test") { CloseBrace = '}' };
            using (tsb.IndentBlock())
            {
            }

            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * The TraceStringBuilder ctor is invoked with a format string and parameter
        ///   Then
        ///   * The string will be appended formatted with the parameter
        /// </summary>
        [TestMethod]
        public void CtorWithArgsAppendsText()
        {
            // Arrange
            const string Expected = "test";
            const string Format = Expected + " {0}";

            // Act
            var tsb = new TraceStringBuilder(Format, 1);
            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected + " 1", actual);
        }

        /// <summary>
        ///   Given
        ///   * A dictionary with 2 key value pairs
        ///   When
        ///   * AppendDictionary is invoked
        ///   Then
        ///   * The dictionary and name should be appended
        /// </summary>
        [TestMethod]
        public void DictionaryShouldAppend()
        {
            const string Expected =
                @"Test
{
	One: 1
	Two: 2
}
";

            // Arrange
            var dictionary = new Dictionary<string, int> { { "One", 1 }, { "Two", 2 }, };
            var tsb = new TraceStringBuilder();

            // Act
            tsb.AppendDictionary("Test", dictionary);

            // Assert
            Assert.AreEqual(Expected, tsb.ToString());
        }

        /// <summary>
        ///   Given
        ///   * A dictionary with two key value pairs
        ///   * A TraceStringBuilder with Options set to TraceOptions.ShowCollectionCount 
        ///   When
        ///   * AppendDictionary is invoked
        ///   Then
        ///   * The name of the dictionary and account is appended
        ///   * The dictionary is appended
        /// </summary>
        [TestMethod]
        public void DictionaryShouldAppendCollectionCount()
        {
            const string Expected =
                @"Test count: 2
{
	One: 1
	Two: 2
}
";

            // Arrange
            var dictionary = new Dictionary<string, int> { { "One", 1 }, { "Two", 2 }, };
            var tsb = new TraceStringBuilder { Options = WorkflowTraceOptions.ShowCollectionCount };

            // Act
            tsb.AppendDictionary("Test", dictionary);

            // Assert
            Assert.AreEqual(Expected, tsb.ToString());
        }

        /// <summary>
        ///   Given
        ///   * An empty dictionary 
        ///   * A TraceStringBuilder with default Options
        ///   When
        ///   * AppendDictionary is invoked
        ///   Then
        ///   * Nothing is appended
        /// </summary>
        [TestMethod]
        public void DictionaryShouldNotShowEmptyCollection()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();
            var tsb = new TraceStringBuilder();

            // Act
            tsb.AppendDictionary("Test", dictionary);

            // Assert
            Assert.AreEqual(string.Empty, tsb.ToString());
        }

        /// <summary>
        ///   Given
        ///   * An empty dictionary 
        ///   * A TraceStringBuilder with Options set to TraceOptions.ShowEmptyCollections 
        ///   When
        ///   * AppendDictionary is invoked
        ///   Then
        ///   * The name of the dictionary and account is appended
        ///   * The dictionary is appended
        /// </summary>
        [TestMethod]
        public void DictionaryShouldShowEmptyCollectionWithFlag()
        {
            const string Expected =
                @"Test
";

            // Arrange
            var dictionary = new Dictionary<string, int>();
            var tsb = new TraceStringBuilder { Options = WorkflowTraceOptions.ShowEmptyCollections };

            // Act
            tsb.AppendDictionary("Test", dictionary);

            // Assert
            Assert.AreEqual(Expected, tsb.ToString());
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder("Test")
        ///   When
        ///   * IndentBlock is invoked
        ///   Then
        ///   * An open brace and close brace will be written on different lines
        /// </summary>
        [TestMethod]
        public void IndentBlockAddsOpenCloseBrace()
        {
            // Arrange
            const string Expected = @"Test{
}
";

            // Act
            var tsb = new TraceStringBuilder("Test");
            using (tsb.IndentBlock())
            {
            }

            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * Indent is invoked
        ///   Then
        ///   * The the tab count will be incremented
        /// </summary>
        [TestMethod]
        public void IndentIncrementsTab()
        {
            // Arrange
            const int Expected = 1;

            var tsb = new TraceStringBuilder();

            // Act
            tsb.Indent();
            var actual = tsb.Tabs;

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder("Test")
        ///   When
        ///   * OpenBrace is set to "{"
        ///   Then
        ///   * An open brace will will be escaped to use "{{" when formatting
        /// </summary>
        [TestMethod]
        public void OpenBraceEscapesCurlyBrace()
        {
            // Arrange
            const string Expected = @"Test{
}
";

            // Act
            var tsb = new TraceStringBuilder("Test") { OpenBrace = '{' };
            using (tsb.IndentBlock())
            {
            }

            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder("Test") with OpenBrace is set to "["
        ///   When
        ///   * IndentBlock is invoked
        ///   Then
        ///   * An open brace and close brace will be written on different lines
        /// </summary>
        [TestMethod]
        public void OpenBraceSetsOpenBrace()
        {
            // Arrange
            const string Expected = @"Test[
}
";

            // Act
            var tsb = new TraceStringBuilder("Test") { OpenBrace = '[' };
            using (tsb.IndentBlock())
            {
            }

            var actual = tsb.ToString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * Unindent is invoked
        ///   Then
        ///   * The the tab count will be decremented
        /// </summary>
        [TestMethod]
        public void UnindentDecrementsTab()
        {
            // Arrange
            const int Expected = 0;

            var tsb = new TraceStringBuilder();
            tsb.Indent();

            // Act
            tsb.Unindent();
            var actual = tsb.Tabs;

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        ///   Given
        ///   * A TraceStringBuilder
        ///   When
        ///   * Unindent is invoked
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void UnindentZeroThrowsInvalidOperation()
        {
            // Arrange
            var tsb = new TraceStringBuilder();

            // Act / Assert
            AssertHelper.Throws<InvalidOperationException>(tsb.Unindent);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The unescape.
        /// </summary>
        /// <param name="c">
        /// The c. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private static string Unescape(char c)
        {
            switch (c)
            {
                case '\t':
                    return "\\t";
                case '\r':
                    return "\\r";
                case '\n':
                    return "\\n";
                default:
                    return c.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// The max.
        /// </summary>
        /// <param name="a">
        /// The a. 
        /// </param>
        /// <param name="b">
        /// The b. 
        /// </param>
        /// <returns>
        /// The System.Int32. 
        /// </returns>
        private int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// The strings differ at.
        /// </summary>
        /// <param name="expected">
        /// The expected. 
        /// </param>
        /// <param name="actual">
        /// The actual. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private string StringsDifferAt(string expected, string actual)
        {
            for (var i = 0; i < this.Max(expected.Length, actual.Length); i++)
            {
                if (i >= expected.Length)
                {
                    return string.Format(
                        "expected.Length is {0}, actual.Length is {1} actual[{2}] is '{3}'", 
                        expected.Length, 
                        actual.Length, 
                        actual.Length - 1, 
                        Unescape(actual[actual.Length - 1]));
                }

                if (i >= actual.Length)
                {
                    return string.Format(
                        "actual.Length is {0}, expected.Length is {1} expected[{2}] is '{3}'", 
                        actual.Length, 
                        expected.Length, 
                        expected.Length - 1, 
                        Unescape(expected[expected.Length - 1]));
                }

                if (expected[i] != actual[i])
                {
                    return string.Format(
                        "expected[{0}] is '{1}' actual[{0}] is '{2}'", i, Unescape(expected[i]), Unescape(actual[i]));
                }
            }

            return "expected and actual are equal";
        }

        #endregion

        /// <summary>
        ///   The test tracable.
        /// </summary>
        public class TestTracable : ITraceable
        {
            #region Constructors and Destructors

            /// <summary>
            ///   Initializes a new instance of the <see cref="TestTracable" /> class.
            /// </summary>
            public TestTracable()
            {
                this.Children = new List<ChildTraceable>();
                for (var i = 0; i < 4; i++)
                {
                    this.Children.Add(new ChildTraceable(i));
                }
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets or sets the strings.
            /// </summary>
            public IEnumerable<ChildTraceable> Strings { get; set; }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the children.
            /// </summary>
            protected IList<ChildTraceable> Children { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The to formatted string.
            /// </summary>
            /// <param name="tabs">
            /// the tabs The tabs. 
            /// </param>
            /// <returns>
            /// The System.String. 
            /// </returns>
            public string ToFormattedString(int tabs = 0)
            {
                var tsb = new TraceStringBuilder(tabs);
                tsb.AppendLine(this.GetType().Name);
                tsb.AppendCollection("Children", this.Children);
                return tsb.ToString();
            }

            #endregion

            /// <summary>
            /// The child traceable.
            /// </summary>
            public class ChildTraceable : ITraceable
            {
                #region Constructors and Destructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ChildTraceable"/> class.
                /// </summary>
                /// <param name="count">
                /// The count.
                /// </param>
                public ChildTraceable(int count)
                {
                    var list = new List<string>();
                    for (var i = 0; i < count; i++)
                    {
                        list.Add(i.ToString());
                    }

                    this.Strings = list;
                }

                #endregion

                #region Public Properties

                /// <summary>
                /// Gets or sets the strings.
                /// </summary>
                public IEnumerable<string> Strings { get; set; }

                #endregion

                #region Implementation of ITraceable

                /// <summary>
                /// The to formatted string.
                /// </summary>
                /// <param name="tabs"> the tabs
                /// The tabs.
                /// </param>
                /// <returns>
                /// The System.String.
                /// </returns>
                public string ToFormattedString(int tabs = 0)
                {
                    var tsb = new TraceStringBuilder(tabs);
                    tsb.AppendTitle(this.GetType().Name);
                    using (tsb.IndentBlock())
                    {
                        tsb.AppendCollection("Strings", this.Strings);
                    }

                    return tsb.ToString();
                }

                #endregion
            }
        }
    }
}