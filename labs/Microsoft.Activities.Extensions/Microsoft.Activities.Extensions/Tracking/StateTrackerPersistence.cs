// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTrackerPersistence.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.DurableInstancing;
    using System.Activities.Persistence;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    /// <summary>
    ///   The state machine state tracker persistence.
    /// </summary>
    public class StateTrackerPersistence : PersistenceParticipant
    {
        #region Static Fields

        /// <summary>
        ///   The Namespace
        /// </summary>
        public static readonly XNamespace Xns = XNamespace.Get("http://schemas.microsoft.com/extensions/persistence");

        #endregion

        #region Fields

        /// <summary>
        ///   The tracker.
        /// </summary>
        private readonly StateTracker tracker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTrackerPersistence"/> class.
        /// </summary>
        /// <param name="tracker">
        /// The tracker. 
        /// </param>
        public StateTrackerPersistence(StateTracker tracker)
        {
            Contract.Requires(tracker != null);
            if (tracker == null)
            {
                throw new ArgumentNullException("tracker");
            }

            this.tracker = tracker;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTrackerPersistence"/> class.
        /// </summary>
        /// <param name="tracker">
        /// The tracker. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        public StateTrackerPersistence(StateTracker tracker, SqlWorkflowInstanceStore instanceStore)
            : this(tracker)
        {
            Contract.Requires(instanceStore != null);
            if (instanceStore == null)
            {
                throw new ArgumentNullException("instanceStore");
            }

            Promote(instanceStore);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets StateTrackerXName.
        /// </summary>
        public static XName StateTrackerXName
        {
            get
            {
                return Xns.GetName("StateTracker");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets DisplayNameXName.
        /// </summary>
        protected static XName DisplayNameXName
        {
            get
            {
                return Xns.GetName("DisplayName");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The promote.
        /// </summary>
        /// <param name="instanceStore">
        /// The instance store. 
        /// </param>
        public static void Promote(SqlWorkflowInstanceStore instanceStore)
        {
            Contract.Requires(instanceStore != null);
            if (instanceStore == null)
            {
                throw new ArgumentNullException("instanceStore");
            }

            instanceStore.Promote("StateTracker", new List<XName> { StateTrackerXName }, null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The collect values.
        /// </summary>
        /// <param name="readWriteValues">
        /// The read write values. 
        /// </param>
        /// <param name="writeOnlyValues">
        /// The write only values. 
        /// </param>
        protected override void CollectValues(
            out IDictionary<XName, object> readWriteValues, out IDictionary<XName, object> writeOnlyValues)
        {
            readWriteValues = null;

            var xml = this.tracker.ToXml();
            writeOnlyValues = new Dictionary<XName, object> { { StateTrackerXName, xml } };

#if DEBUG
            WorkflowTrace.Information("StateTracker Writing XML to Promoted Property StateTracker");
            WorkflowTrace.Information(xml);
#endif
        }

        #endregion
    }
}