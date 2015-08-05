// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTracker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.DurableInstancing;
    using System.Activities.Statements;
    using System.Activities.Statements.Tracking;
    using System.Activities.Tracking;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel.Activities;
    using System.Text;
    using System.Xml;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.DurableInstancing;

#if NET401_OR_GREATER

    /// <summary>
    ///   Tracks StateMachines in a Workflow
    /// </summary>
    /// <remarks>
    ///   A StateTracker Is Used For
    ///   * Tracking the state and state history of a state machine
    ///   * Determining the possible transitions of a state machine
    ///   A StateTracker is used with
    ///   * An activity which contains 0..n state machines
    ///   * State Machines may contain other state machines
    ///   * Most of the time the activity contains only 1 state machine
    ///   A StateTracker is
    ///   * A TrackingParticipant
    ///   A StateTracker has
    ///   * A collection of StateMachineInfo 
    ///   Threading
    ///   * The thread that reads the tracker is different than the thread that does the tracking.
    ///   therefore StateTracker must be safe for concurrent reading / writing.
    /// </remarks>
    [DataContract]
    public class StateTracker : TypedTrackingParticipant, IStateMachineInfo
    {
        #region Constants

        /// <summary>
        ///   Finds existing instances
        /// </summary>
        private const string FindExistingInstanceSql = @"SELECT Value1
FROM [System.Activities.DurableInstancing].[InstancePromotedProperties]
WHERE PromotionName = @PromotionName 
AND InstanceId = @InstanceId";

        /// <summary>
        ///   The find existing instances sql.
        /// </summary>
        private const string FindExistingInstancesSql = @"SELECT Value1      
FROM [System.Activities.DurableInstancing].[InstancePromotedProperties]
WHERE PromotionName = @PromotionName";

        #endregion

        #region Fields

        /// <summary>
        ///   The max history.
        /// </summary>
        private readonly int maxHistory;

        /// <summary>
        ///   The state machines.
        /// </summary>
        private readonly ConcurrentDictionary<string, StateMachineInfo> stateMachines =
            new ConcurrentDictionary<string, StateMachineInfo>();

        /// <summary>
        ///   The current state machine.
        /// </summary>
        private StateMachineInfo currentStateMachine;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTracker"/> class.
        /// </summary>
        /// <param name="maxHistory">
        /// The max history. 
        /// </param>
        public StateTracker(int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            this.maxHistory = maxHistory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTracker"/> class.
        /// </summary>
        /// <param name="stateTrackerSurrogated">
        /// The state tracker surrogated. 
        /// </param>
        public StateTracker(StateTrackerSurrogated stateTrackerSurrogated)
        {
            foreach (var stateMachine in stateTrackerSurrogated.StateMachines)
            {
                var machine = stateMachine;
                this.currentStateMachine = this.stateMachines.AddOrUpdate(
                    GetKey(stateMachine), s => machine, (s, info) => machine);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the last known state of the current state machine
        /// </summary>
        public string CurrentState
        {
            get
            {
                return this.CurrentStateMachine != null ? this.CurrentStateMachine.CurrentState : null;
            }
        }

        /// <summary>
        ///   Gets the instance ID of the state machine
        /// </summary>
        public Guid InstanceId
        {
            get
            {
                return this.CurrentStateMachine != null ? this.CurrentStateMachine.InstanceId : Guid.Empty;
            }
        }

        /// <summary>
        ///   Gets the last known instance state
        /// </summary>
        public ActivityInstanceState InstanceState
        {
            get
            {
                return this.CurrentStateMachine != null
                           ? this.CurrentStateMachine.InstanceState
                           : ActivityInstanceState.Executing;
            }
        }

        /// <summary>
        ///   Gets the name of the state machine
        /// </summary>
        public string Name
        {
            get
            {
                return this.CurrentStateMachine != null ? this.CurrentStateMachine.Name : null;
            }
        }

        /// <summary>
        ///   Gets the possible transitions
        /// </summary>
        public ReadOnlyCollection<string> PossibleTransitions
        {
            get
            {
                return this.CurrentStateMachine != null
                           ? this.CurrentStateMachine.PossibleTransitions
                           : new ReadOnlyCollection<string>(new string[0]);
            }
        }

        /// <summary>
        ///   Gets the previous state
        /// </summary>
        public string PreviousState
        {
            get
            {
                return this.CurrentStateMachine != null ? this.CurrentStateMachine.PreviousState : null;
            }
        }

        /// <summary>
        ///   Gets the state history
        /// </summary>
        public ReadOnlyCollection<string> StateHistory
        {
            get
            {
                return this.CurrentStateMachine != null
                           ? this.CurrentStateMachine.StateHistory
                           : new ReadOnlyCollection<string>(new string[0]);
            }
        }

        /// <summary>
        ///   Gets the state machines.
        /// </summary>
        [DataMember]
        public ReadOnlyCollection<StateMachineInfo> StateMachines
        {
            get
            {
                return new ReadOnlyCollection<StateMachineInfo>(this.stateMachines.Values.ToList());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the current state machine.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is more than one tracked state machine</exception>
        public StateMachineInfo CurrentStateMachine
        {
            get
            {
                if (this.stateMachines.Count > 1)
                {
                    throw new InvalidOperationException(SR.More_than_one_state_machine_is_tracked);
                }

                return this.currentStateMachine;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attaches a StateTracker to a WorkflowApplication
        /// </summary>
        /// <param name="workflowApplication">
        /// The WorkflowApplication to attach to 
        /// </param>
        /// <param name="maxHistory">
        /// The maximum number of state history entries 
        /// </param>
        /// <returns>
        /// The StateTracker attached 
        /// </returns>
        public static StateTracker Attach(
            WorkflowApplication workflowApplication, int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            return Attach(workflowApplication, null, maxHistory);
        }

        /// <summary>
        /// Attaches a StateTracker to a WorkflowApplication
        /// </summary>
        /// <param name="workflowApplication">
        /// The WorkflowApplication to attach to 
        /// </param>
        /// <param name="sqlWorkflowInstanceStore">
        /// The instance store 
        /// </param>
        /// <param name="maxHistory">
        /// The maximum number of state history entries 
        /// </param>
        /// <returns>
        /// The StateTracker attached 
        /// </returns>
        public static StateTracker Attach(
            WorkflowApplication workflowApplication, 
            SqlWorkflowInstanceStore sqlWorkflowInstanceStore, 
            int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            var stateTracker = new StateTracker(maxHistory);
            workflowApplication.Extensions.Add(stateTracker);

            if (sqlWorkflowInstanceStore != null)
            {
                if (sqlWorkflowInstanceStore.IsReadOnly())
                {
                    throw new InvalidOperationException("Instance store is read only - cannot promote properties");
                }

                StateTrackerPersistence.Promote(sqlWorkflowInstanceStore);
                var persistence = new StateTrackerPersistence(stateTracker);
                workflowApplication.Extensions.Add(persistence);
            }

            return stateTracker;
        }

        /// <summary>
        /// Attaches a StateTracker to a WorkflowServiceHost
        /// </summary>
        /// <param name="workflowServiceHost">
        /// The host 
        /// </param>
        /// <param name="maxHistory">
        /// The maximum history 
        /// </param>
        /// <returns>
        /// The Microsoft.Activities.Extensions.Tracking.StateTracker. 
        /// </returns>
        public static StateTracker Attach(
            WorkflowServiceHost workflowServiceHost, int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            return Attach(workflowServiceHost, null, maxHistory);
        }

        /// <summary>
        /// Attaches a StateTracker to a WorkflowServiceHost
        /// </summary>
        /// <param name="workflowServiceHost">
        /// The host 
        /// </param>
        /// <param name="sqlWorkflowInstanceStore">
        /// The instance store 
        /// </param>
        /// <param name="maxHistory">
        /// The maximum history 
        /// </param>
        /// <returns>
        /// The Microsoft.Activities.Extensions.Tracking.StateTracker. 
        /// </returns>
        public static StateTracker Attach(
            WorkflowServiceHost workflowServiceHost, 
            SqlWorkflowInstanceStore sqlWorkflowInstanceStore, 
            int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            Contract.Requires(workflowServiceHost != null);
            if (workflowServiceHost == null)
            {
                throw new ArgumentNullException("workflowServiceHost");
            }

            var stateTracker = new StateTracker(maxHistory);
            workflowServiceHost.WorkflowExtensions.Add(stateTracker);

            if (sqlWorkflowInstanceStore != null)
            {
                if (sqlWorkflowInstanceStore.IsReadOnly())
                {
                    throw new InvalidOperationException("Instance store is read only - cannot promote properties");
                }

                StateTrackerPersistence.Promote(sqlWorkflowInstanceStore);
                var persistence = new StateTrackerPersistence(stateTracker);
                workflowServiceHost.WorkflowExtensions.Add(persistence);
            }

            return stateTracker;
        }

        /// <summary>
        /// The load instance.
        /// </summary>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        /// <returns>
        /// The Microsoft.Activities.Extensions.Tracking.StateTracker. 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The instance
        /// </exception>
        public static StateTracker LoadInstance(Guid instanceId, string connectionString)
        {
            Contract.Requires(instanceId != Guid.Empty);
            if (instanceId == Guid.Empty)
            {
                throw new ArgumentException("instanceId");
            }

            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(FindExistingInstanceSql, connection);
                command.Parameters.AddWithValue("@PromotionName", "StateTracker");
                command.Parameters.AddWithValue("@InstanceId", instanceId);
                connection.Open();
                var dataReader = command.ExecuteReader();
                var instance = dataReader.Read() ? Parse(dataReader.GetString(0)) : null;

                if (instance == null)
                {
                    throw new KeyNotFoundException("Cannot find instance id " + instanceId);
                }

                return instance;
            }
        }

        /// <summary>
        /// The load instances.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        /// <returns>
        /// The System.Collections.ObjectModel.ReadOnlyCollection`1[T - &gt; Microsoft.Activities.Extensions.Tracking.StateTracker]. 
        /// </returns>
        public static ReadOnlyCollection<StateTracker> LoadInstances(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(FindExistingInstancesSql, connection);
                command.Parameters.AddWithValue("@PromotionName", "StateTracker");
                connection.Open();
                var list = new List<StateTracker>();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    list.Add(Parse(dataReader.GetString(0)));
                }

                dataReader.Close();
                return new ReadOnlyCollection<StateTracker>(list);
            }
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="xml">
        /// The xml. 
        /// </param>
        /// <returns>
        /// The Microsoft.Activities.Extensions.Tracking.StateTracker. 
        /// </returns>
        public static StateTracker Parse(string xml)
        {
            var surrogate = new StateTrackerSurrogateSelector();
            var serializer = new DataContractSerializer(
                typeof(StateTracker), new Type[0], short.MaxValue, false, false, surrogate);
            var buffer = new StringReader(xml);
            using (var reader = XmlReader.Create(buffer))
            {
                return (StateTracker)serializer.ReadObject(reader);
            }
        }

        /// <summary>
        ///   The to xml.
        /// </summary>
        /// <returns> The System.String. </returns>
        public string ToXml()
        {
            var surrogate = new StateTrackerSurrogateSelector();
            var serializer = new DataContractSerializer(
                typeof(StateTracker), new Type[0], short.MaxValue, false, false, surrogate);
            var buffer = new StringBuilder();
            using (
                var writer = XmlWriter.Create(
                    buffer, 
                    new XmlWriterSettings
                        {
                            Indent = true, 
                            OmitXmlDeclaration = true, 
                            NamespaceHandling = NamespaceHandling.OmitDuplicates
                        }))
            {
                serializer.WriteObject(writer, this);
            }

            return buffer.ToString();
        }

        /// <summary>
        ///   The trace.
        /// </summary>
        public void Trace()
        {
            var tsb = new TraceStringBuilder();
            tsb.AppendLine(this.GetType().Name);
            tsb.AppendCollection("StateMachines", this.StateMachines);
            WorkflowTrace.Information(tsb.ToString());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The track.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        protected override void Track(StateMachineStateRecord record, TimeSpan timeout)
        {
            this.AddOrUpdateStateMachine(record);
        }

        /// <summary>
        /// The track.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An unknown ActivityInstanceState was encountered
        /// </exception>
        protected override void Track(ActivityStateRecord record, TimeSpan timeout)
        {
            // If the record is a state machine
            if (record.Activity.TypeName
                == typeof(StateMachine).FullName)
            {
                this.AddOrUpdateStateMachine(record);
            }
        }

        /// <summary>
        /// The track.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        protected override void Track(WorkflowInstanceAbortedRecord record, TimeSpan timeout)
        {
            this.UpdateInstanceState(record);
        }

        /// <summary>
        /// The get key.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private static string GetKey(ActivityStateRecord record)
        {
            return record.InstanceId + record.Activity.Name;
        }

        /// <summary>
        /// Adds or Updates a state machine
        /// </summary>
        /// <param name="record">
        /// The tracking record 
        /// </param>
        /// <remarks>
        /// In cases where the workflow is loaded, the StateMachineStateRecord may be the first tracking record
        /// </remarks>
        private void AddOrUpdateStateMachine(StateMachineStateRecord record)
        {
            var history = this.maxHistory;
            this.currentStateMachine = this.stateMachines.AddOrUpdate(
                GetKey(record), 
                s =>
                    {
                        var info = new StateMachineInfo(history)
                            {
                                Name = record.Activity.Name, 
                                InstanceState = ActivityInstanceState.Executing, 
                                InstanceId = record.InstanceId
                            };
                        info.UpdateState(record);
                        return info;
                    }, 
                (s, info) =>
                    {
                        info.InstanceState = ActivityInstanceState.Executing;
                        info.UpdateState(record);
                        return info;
                    });
        }

        /// <summary>
        /// The add state machine.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        private void AddOrUpdateStateMachine(ActivityStateRecord record)
        {
            var history = this.maxHistory;
            this.currentStateMachine = this.stateMachines.AddOrUpdate(
                GetKey(record), 
                s =>
                new StateMachineInfo(history)
                    {
                        Name = record.Activity.Name, 
                        InstanceState = record.GetInstanceState(), 
                        InstanceId = record.InstanceId
                    }, 
                (s, info) =>
                    {
                        info.InstanceState = record.GetInstanceState();
                        return info;
                    });
        }

        /// <summary>
        /// The get key.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private string GetKey(StateMachineInfo stateMachine)
        {
            return stateMachine.InstanceId + stateMachine.Name;
        }

        /// <summary>
        /// The get key.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private string GetKey(StateMachineStateRecord record)
        {
            return record.InstanceId + record.StateMachineName;
        }

        /// <summary>
        /// The update instance state.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        private void UpdateInstanceState(WorkflowInstanceAbortedRecord record)
        {
            var query = from stateMachine in this.stateMachines.Values
                        where stateMachine.InstanceId == record.InstanceId
                        select stateMachine;

            foreach (var stateMachine in query)
            {
                stateMachine.Abort(record);
            }
        }

        /// <summary>
        /// The update instance state.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        private void UpdateInstanceState(ActivityStateRecord record)
        {
            var stateMachineInfo = this.stateMachines[GetKey(record)];
            stateMachineInfo.UpdateState(record);
        }

        #endregion
    }

#endif
}