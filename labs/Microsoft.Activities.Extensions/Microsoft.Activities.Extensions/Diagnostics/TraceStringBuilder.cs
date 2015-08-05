// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceStringBuilder.cs" company="Microsoft Open Technologies, Inc.">
//   Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Diagnostics
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Creates formatted strings for tracing
    /// </summary>
    public class TraceStringBuilder
    {
        #region Fields

        /// <summary>
        ///   The string builder.
        /// </summary>
        private readonly StringBuilder stringBuilder;

        /// <summary>
        ///   The close brace.
        /// </summary>
        private char closeBrace = '}';

        /// <summary>
        ///   The open brace.
        /// </summary>
        private char openBrace = '{';

        /// <summary>
        ///   The tab.
        /// </summary>
        private int tabs;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceStringBuilder"/> class.
        /// </summary>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        public TraceStringBuilder(int tabs = 0)
        {
            this.stringBuilder = new StringBuilder();
            this.tabs = tabs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceStringBuilder"/> class.
        /// </summary>
        /// <param name="tabs">
        /// the tabs The tabs 
        /// </param>
        /// <param name="s">
        /// The s. 
        /// </param>
        /// <param name="args">
        /// Arguments to format string 
        /// </param>
        public TraceStringBuilder(int tabs, string s, params object[] args)
        {
            this.tabs = tabs;
            this.stringBuilder = new StringBuilder();
            this.AppendFormat(s, args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceStringBuilder"/> class.
        /// </summary>
        /// <param name="s">
        /// The s. 
        /// </param>
        /// <param name="args">
        /// Arguments to format string 
        /// </param>
        public TraceStringBuilder(string s, params object[] args)
            : this(0, s, args)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the close brace.
        /// </summary>
        public char CloseBrace
        {
            get
            {
                return this.closeBrace;
            }

            set
            {
                this.closeBrace = value;
            }
        }

        /// <summary>
        ///   Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.stringBuilder.Length;
            }
        }

        /// <summary>
        ///   Gets or sets the open brace.
        /// </summary>
        public char OpenBrace
        {
            get
            {
                return this.openBrace;
            }

            set
            {
                this.openBrace = value;
            }
        }

        /// <summary>
        ///   Gets or sets the options.
        /// </summary>
        public WorkflowTraceOptions Options { get; set; }

        /// <summary>
        ///   Gets the tab.
        /// </summary>
        public int Tabs
        {
            get
            {
                return this.tabs;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the indent.
        /// </summary>
        private string TabbedIndent
        {
            get
            {
                return new string('\t', this.tabs);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The append format.
        /// </summary>
        /// <param name="s">
        /// The format. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The format is null
        /// </exception>
        public void Append(string s)
        {
            this.stringBuilder.AppendFormat("{0}{1}", this.TabbedIndent, s ?? string.Empty);
        }

        /// <summary>
        /// Appends a collection.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="enumerable">
        /// The enumerable. 
        /// </param>
        /// <param name="formattingFunc">
        /// The formatting Func. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the collection 
        /// </typeparam>
        public void AppendCollection<T>(
            string name, IEnumerable<T> enumerable, Func<T, int, string> formattingFunc = null)
        {
            var list = enumerable == null ? new List<T>() : enumerable.ToList();

            if (!EnumerableEx.IsNullOrEmpty(list)
                || this.Options.HasFlag(WorkflowTraceOptions.ShowEmptyCollections))
            {
                // Always output the name and possibly colleciton count
                if (this.Options.HasFlag(WorkflowTraceOptions.ShowCollectionCount))
                {
                    this.AppendLine("{0}: {1}", name, list.Count);
                }
                else
                {
                    this.AppendLine("{0}", name);
                }

                if (!EnumerableEx.IsNullOrEmpty(list))
                {
                    using (this.IndentBlock())
                    {
                        foreach (var item in list)
                        {
                            this.AppendLine(
                                !Equals(item, default(T)) ? this.GetItemString(item, formattingFunc) : string.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Appends a comma delimited list
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="enumerable">
        /// The enumerable. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the collection 
        /// </typeparam>
        public void AppendDelimitedList<T>(string name, IEnumerable<T> enumerable)
        {
            var list = enumerable == null ? new List<T>() : enumerable.ToList();

            if (!EnumerableEx.IsNullOrEmpty(list)
                || this.Options.HasFlag(WorkflowTraceOptions.ShowEmptyCollections))
            {
                this.AppendLine("{0}: [{1}]", name, EnumerableEx.ToDelimitedList(list));
            }
        }

        /// <summary>
        /// The append dictionary.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="dictionary">
        /// The dictionary. 
        /// </param>
        /// <typeparam name="TKey">
        /// The type of the key 
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value 
        /// </typeparam>
        public void AppendDictionary<TKey, TValue>(string name, IDictionary<TKey, TValue> dictionary)
        {
            if (!dictionary.IsNullOrEmpty()
                || this.Options.HasFlag(WorkflowTraceOptions.ShowEmptyCollections))
            {
                if (this.Options.HasFlag(WorkflowTraceOptions.ShowCollectionCount))
                {
                    this.AppendLine("{0} count: {1}", name, dictionary == null ? 0 : dictionary.Count);
                }
                else
                {
                    this.AppendLine("{0}", name);
                }

                if (!dictionary.IsNullOrEmpty())
                {
                    using (this.IndentBlock())
                    {
                        if (dictionary != null)
                        {
                            foreach (var kvp in dictionary)
                            {
                                this.AppendProperty(kvp.Key.ToString(), kvp.Value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The append format.
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The format is null
        /// </exception>
        public void AppendFormat(string format, params object[] args)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(format));
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentNullException("format");
            }

            // ReSharper disable CoVariantArrayConversion
            // This conversion is allowed because it was an object[] then converted to string[] and back
            var s = args.IsNullOrEmpty() ? format : string.Format(format, args.Select(this.IndentLines).ToArray());

            // ReSharper restore CoVariantArrayConversion
            this.stringBuilder.Append(this.TabbedIndent + s);
        }

        /// <summary>
        /// Appends another formatted string by trimming the starting tabs
        /// </summary>
        /// <param name="formatted">
        /// The formatted string 
        /// </param>
        public void AppendFormatted(string formatted)
        {
            Contract.Requires(formatted != null);
            if (formatted == null)
            {
                throw new ArgumentNullException("formatted");
            }

            this.Append(formatted.TrimStart());
        }

        /// <summary>
        /// Appends a line
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The format string is invalid
        /// </exception>
        public void AppendLine(string format, params object[] args)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(format));
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentNullException("format");
            }

            this.AppendFormat(format, args);
            this.stringBuilder.AppendLine();
        }

        /// <summary>
        ///   Appends a line
        /// </summary>
        public void AppendLine()
        {
            this.stringBuilder.AppendLine();
        }

        /// <summary>
        /// The append property.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="formattingFunc">
        /// The formatting Func. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the property 
        /// </typeparam>
        public void AppendProperty<T>(string name, T value, Func<T, int, string> formattingFunc = null)
        {
            var dictionary = value as IDictionary<string, object>;
            if (dictionary != null)
            {
                this.AppendDictionary(name, dictionary);
            }
            else
            {
                // ReSharper disable CompareNonConstrainedGenericWithNull
                // Value types will return false when compared with null
                this.AppendLine(
                    "{0}: {1}", name, value != null ? this.GetItemString(value, formattingFunc).Trim() : Constants.Null);

                // ReSharper restore CompareNonConstrainedGenericWithNull
            }
        }

        /// <summary>
        /// The append quoted list.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="list">
        /// The list. 
        /// </param>
        public void AppendQuotedList(string name, IEnumerable<string> list)
        {
            this.AppendDelimitedList(name, list.Select(s => string.Format(CultureInfo.CurrentCulture, "\"{0}\"", s)));
        }

        /// <summary>
        /// Append the title of the object being traced
        /// </summary>
        /// <param name="format">
        /// The format 
        /// </param>
        /// <param name="args">
        /// The args 
        /// </param>
        /// <remarks>
        /// A traceable type should set the title which will not be indented
        /// </remarks>
        public void AppendTitle(string format, params object[] args)
        {
            this.stringBuilder.AppendLine(args.IsNullOrEmpty() ? format : string.Format(format, args));
        }

        /// <summary>
        /// The append title.
        /// </summary>
        /// <param name="o">
        /// The o. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The object must not be empty
        /// </exception>
        public void AppendTitle(object o)
        {
            Contract.Requires(o != null);
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            this.AppendTitle(o.GetType().Name);
        }

        /// <summary>
        ///   The indent.
        /// </summary>
        public void Indent()
        {
            Interlocked.Increment(ref this.tabs);
        }

        /// <summary>
        ///   The indent block.
        /// </summary>
        /// <returns> The System.IDisposable. </returns>
        public IDisposable IndentBlock()
        {
            return new BlockIndenter(this);
        }

        /// <summary>
        ///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns> A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> . </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }

        /// <summary>
        ///   The unindent.
        /// </summary>
        public void Unindent()
        {
            if (this.tabs == 0)
            {
                throw new InvalidOperationException("Cannot unindent, tab level is already zero");
            }

            Interlocked.Decrement(ref this.tabs);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Escapes open and close braces and removes leading tabs
        /// </summary>
        /// <param name="s">
        /// The string to format 
        /// </param>
        /// <returns>
        /// The escaped and trimmed string 
        /// </returns>
        private static string EscapeAndTrim(string s)
        {
            var open = Regex.Replace(s, "{(?!{)", "{{");
            return Regex.Replace(open, "}(?!})", "}}").TrimStart('\t');
        }

        /// <summary>
        /// The append line.
        /// </summary>
        /// <param name="c">
        /// The c. 
        /// </param>
        private void AppendLine(char c)
        {
            this.AppendLine(c.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///   Appends a closing brace and unindents
        /// </summary>
        private void CloseBlock()
        {
            this.Unindent();
            this.stringBuilder.Append(this.TabbedIndent + this.CloseBrace + Environment.NewLine);
        }

        /// <summary>
        /// The get item string.
        /// </summary>
        /// <param name="item">
        /// The item. 
        /// </param>
        /// <param name="formatFunc">
        /// The type of the formattableExtension 
        /// </param>
        /// <typeparam name="T">
        /// The type of the item 
        /// </typeparam>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private string GetItemString<T>(T item, Func<T, int, string> formatFunc)
        {
            var formattable = item as ITraceable;
            if (formattable != null)
            {
                return formattable.ToFormattedString(this.Tabs);
            }

            if (item is Activity)
            {
                var activity = item as Activity;
                return activity.DisplayName;
            }

            if (formatFunc != null)
            {
                return formatFunc(item, this.Tabs);
            }

            return Equals(item, default(T))
                       ? string.Format(CultureInfo.CurrentCulture, "{0}", default(T))
                       : string.Format(CultureInfo.CurrentCulture, "{0}", item);
        }

        /// <summary>
        /// The indent lines.
        /// </summary>
        /// <param name="item">
        /// The item. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private string IndentLines(object item)
        {
            if (item == null)
            {
                return null;
            }

            return Regex.Replace(item.ToString(), "(\r\n)(?![{}\t])", "$&" + this.TabbedIndent);
        }

        /// <summary>
        ///   The append open brace.
        /// </summary>
        private void OpenBlock()
        {
            this.stringBuilder.AppendLine(this.TabbedIndent + this.OpenBrace);
            this.Indent();
        }

        #endregion

        /// <summary>
        ///   The indent block.
        /// </summary>
        private class BlockIndenter : IDisposable
        {
            #region Fields

            /// <summary>
            ///   The trace string builder.
            /// </summary>
            private readonly TraceStringBuilder traceStringBuilder;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BlockIndenter"/> class.
            /// </summary>
            /// <param name="traceStringBuilder">
            /// The trace string builder. 
            /// </param>
            public BlockIndenter(TraceStringBuilder traceStringBuilder)
            {
                this.traceStringBuilder = traceStringBuilder;
                traceStringBuilder.OpenBlock();
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///   The dispose.
            /// </summary>
            public void Dispose()
            {
                var builder = this.traceStringBuilder;
                if (builder != null)
                {
                    builder.CloseBlock();
                }
            }

            #endregion
        }
    }
}