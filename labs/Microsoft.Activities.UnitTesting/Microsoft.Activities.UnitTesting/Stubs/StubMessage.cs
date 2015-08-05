// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StubMessage.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///   The stub message.
    /// </summary>
    [Serializable]
    public class StubMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StubMessage"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message Type. 
        /// </param>
        public StubMessage(StubMessageType messageType)
        {
            this.MessageType = messageType;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="StubMessage" /> class.
        /// </summary>
        public StubMessage()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets BookmarkName.
        /// </summary>
        public string BookmarkName
        {
            get
            {
                return string.Format("{0}|{1}", this.Contract, this.Operation);
            }
        }

        /// <summary>
        ///   Gets or sets Content.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        ///   Gets or sets Contract.
        /// </summary>
        public string Contract { get; set; }

        /// <summary>
        ///   Gets or sets MessageType.
        /// </summary>
        public StubMessageType MessageType { get; set; }

        /// <summary>
        ///   Gets or sets Operation.
        /// </summary>
        public string Operation { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Parameters.
        /// </summary>
        /// <exception cref="InvalidOperationException">The content is not a dictionary</exception>
        protected IDictionary<string, object> Parameters
        {
            get
            {
                var dictionary = this.Content as IDictionary<string, object>;
                if (dictionary != null)
                {
                    return dictionary;
                }

                throw new InvalidOperationException("Content is not an IDictionary<string,object>");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets a parameter from the message content if it is a dictionary
        /// </summary>
        /// <param name="key">
        /// The key. 
        /// </param>
        /// <returns>
        /// The parameter. 
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The content is null or not a dictionary
        /// </exception>
        public object Parameter(string key)
        {
            var dictionary = this.Content as IDictionary<string, object>;
            if (dictionary != null)
            {
                return dictionary[key];
            }

            throw new InvalidOperationException("Content is not an IDictionary<string,object>");
        }

        /// <summary>
        ///   The to string.
        /// </summary>
        /// <returns> The string. </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} | {1} | {2}", this.MessageType, this.Contract, this.Operation);

            if (this.Content is IDictionary<string, object>)
            {
                sb.Append(" | {");
                var index = 0;
                foreach (var parameter in this.Parameters)
                {
                    sb.AppendFormat("{0}={1}", parameter.Key, parameter.Value);
                    index++;
                    if (index < this.Parameters.Count)
                    {
                        sb.Append(',');
                    }
                }

                sb.Append('}');
            }
            else
            {
                sb.AppendFormat(" | {{{0}}}", this.Content ?? "(null)");
            }

            return sb.ToString();
        }

        #endregion
    }
}