// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowServiceTestHost.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Activities.Hosting;
    using System.Activities.Tracking;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.DurableInstancing;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.Activities.Description;
    using System.ServiceModel.Description;
    using System.Threading;
    using System.Xaml;
    using System.Xml;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Persistence;
    using Microsoft.Activities.UnitTesting.Tracking;

    using WorkflowServiceHost = System.ServiceModel.Activities.WorkflowServiceHost;

    /// <summary>
    ///   The workflow service test host.
    /// </summary>
    public sealed class WorkflowServiceTestHost : IDisposable
    {
        #region Constants

        /// <summary>
        ///   The default timeout.
        /// </summary>
        public const int DefaultTimeout = 1000;

        #endregion

        #region Fields

        /// <summary>
        ///   The deleted event.
        /// </summary>
        private readonly ManualResetEvent deletedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The host closed event.
        /// </summary>
        private readonly ManualResetEvent hostClosedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The unloaded event.
        /// </summary>
        private readonly ManualResetEvent unloadedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The service URI
        /// </summary>
        private Uri serviceUri;

        /// <summary>
        ///   The time to persist
        /// </summary>
        private TimeSpan timeToPersist;

        /// <summary>
        ///   The time to unload
        /// </summary>
        private TimeSpan timeToUnload;

        /// <summary>
        ///   The tracking.
        /// </summary>
        private MemoryTrackingParticipant tracking;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowServiceTestHost"/> class.
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        public WorkflowServiceTestHost(WorkflowService workflowService, EndpointAddress serviceEndpoint)
            : this(workflowService, serviceEndpoint != null ? serviceEndpoint.Uri : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowServiceTestHost"/> class.
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        public WorkflowServiceTestHost(WorkflowService workflowService, Uri serviceUri)
        {
            Contract.Requires(workflowService != null);
            Contract.Requires(serviceUri != null);

            if (workflowService == null)
            {
                throw new ArgumentNullException("workflowService");
            }

            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            this.WorkflowService = workflowService;
            this.ServiceUri = serviceUri;

            // Fix for Issue 7835
            // Create the host in the ctor
            this.CreateWorkflowServiceHost();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowServiceTestHost"/> class.
        /// </summary>
        /// <param name="workflowServiceFile">
        /// The workflow service file. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        public WorkflowServiceTestHost(string workflowServiceFile, EndpointAddress serviceEndpoint)
            : this(workflowServiceFile, serviceEndpoint != null ? serviceEndpoint.Uri : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowServiceTestHost"/> class.
        /// </summary>
        /// <param name="workflowServiceFile">
        /// The workflow service file. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        public WorkflowServiceTestHost(string workflowServiceFile, Uri serviceUri)
            : this((WorkflowService)XamlServices.Load(workflowServiceFile), serviceUri)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets a service address based on the Uri
        /// </summary>
        public EndpointAddress EndpointAddress
        {
            get
            {
                return new EndpointAddress(this.serviceUri);
            }
        }

        /// <summary>
        ///   Gets Host.
        /// </summary>
        public WorkflowServiceHost Host { get; private set; }

        /// <summary>
        ///   Gets or sets the instance store
        /// </summary>
        /// <remarks>
        ///   Defaults to MemoryStore
        /// </remarks>
        public InstanceStore InstanceStore
        {
            get
            {
                Debug.Assert(this.Host != null, "Host is null");
                Debug.Assert(this.Host.DurableInstancingOptions != null, "DurableInstancingOptions is null");

                if (this.Host.DurableInstancingOptions.InstanceStore == null)
                {
                    MemoryStore.Reset();
                    this.Host.DurableInstancingOptions.InstanceStore = new MemoryStore();
                }

                return this.Host.DurableInstancingOptions.InstanceStore;
            }

            set
            {
                Debug.Assert(this.Host != null, "Host is null");
                Debug.Assert(this.Host.DurableInstancingOptions != null, "DurableInstancingOptions is null");
                this.Host.DurableInstancingOptions.InstanceStore = value;
            }
        }

        /// <summary>
        ///   Gets or sets ServiceUri.
        /// </summary>
        public Uri ServiceUri
        {
            get
            {
                return this.serviceUri;
            }

            set
            {
                if (this.IsOpen())
                {
                    throw new InvalidOperationException("Cannot change the ServiceURI after it has been opened");
                }

                this.serviceUri = value;
            }
        }

        /// <summary>
        ///   Gets or sets TimeToPersist.
        /// </summary>
        public TimeSpan TimeToPersist
        {
            get
            {
                return this.timeToPersist;
            }

            set
            {
                if (this.IsOpen())
                {
                    throw new InvalidOperationException("Cannot change TimeToPersist once the host is open");
                }

                this.timeToPersist = value;
            }
        }

        /// <summary>
        ///   Gets or sets TimeToUnload.
        /// </summary>
        public TimeSpan TimeToUnload
        {
            get
            {
                return this.timeToUnload;
            }

            set
            {
                if (this.IsOpen())
                {
                    throw new InvalidOperationException("Cannot change TimeToUnload once the host is open");
                }

                this.timeToUnload = value;
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
        ///   Gets or sets TrackingProfile.
        /// </summary>
        public TrackingProfile TrackingProfile
        {
            get
            {
                Debug.Assert(this.Tracking != null, "this.Tracking == null");
                return this.Tracking.TrackingProfile;
            }

            set
            {
                Debug.Assert(this.Tracking != null, "this.Tracking == null");
                this.Tracking.TrackingProfile = value;
            }
        }

        /// <summary>
        ///   Gets the Extensions manager
        /// </summary>
        public WorkflowInstanceExtensionManager WorkflowExtensions
        {
            get
            {
                Debug.Assert(this.Host != null, "Host is null");
                return this.Host.WorkflowExtensions;
            }
        }

        /// <summary>
        ///   Gets or sets WorkflowService.
        /// </summary>
        public WorkflowService WorkflowService { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            WorkflowService workflowService, Uri serviceUri, params IServiceBehavior[] behaviors)
        {
            return Open(workflowService, serviceUri, null, behaviors);
        }

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            WorkflowService workflowService, 
            Uri serviceUri, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(workflowService != null);
            Contract.Requires(serviceUri != null);

            if (workflowService == null)
            {
                throw new ArgumentNullException("workflowService");
            }

            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            var workflowServiceTestHost = new WorkflowServiceTestHost(workflowService, serviceUri);

            if (instanceStore != null)
            {
                workflowServiceTestHost.InstanceStore = instanceStore;
            }

            if (behaviors != null)
            {
                Debug.Assert(workflowServiceTestHost.Host != null, "workflowServiceTestHost.Host != null");
                Debug.Assert(
                    workflowServiceTestHost.Host.Description != null, "workflowServiceTestHost.Host.Description != null");
                Debug.Assert(
                    workflowServiceTestHost.Host.Description.Behaviors != null, 
                    "workflowServiceTestHost.Host.Description.Behaviors != null");
                foreach (var serviceBehavior in behaviors)
                {
                    workflowServiceTestHost.Host.Description.Behaviors.Add(serviceBehavior);
                }
            }

            workflowServiceTestHost.Open();
            return workflowServiceTestHost;
        }

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            WorkflowService workflowService, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(workflowService != null);
            if (workflowService == null)
            {
                throw new ArgumentNullException("workflowService");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open(workflowService, serviceEndpoint.Uri, instanceStore, behaviors);
        }

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowServiceFile">
        /// The workflow service file. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            string workflowServiceFile, EndpointAddress serviceEndpoint, params IServiceBehavior[] behaviors)
        {
            return Open(workflowServiceFile, serviceEndpoint, null, behaviors);
        }

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowServiceFile">
        /// The workflow service file. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance store 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            string workflowServiceFile, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(workflowServiceFile));
            if (string.IsNullOrWhiteSpace(workflowServiceFile))
            {
                throw new ArgumentNullException("workflowServiceFile");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open(
                (WorkflowService)XamlServices.Load(workflowServiceFile), serviceEndpoint.Uri, instanceStore, behaviors);
        }

        /// <summary>
        /// Opens the host
        /// </summary>
        /// <param name="workflowServiceFile">
        /// The workflow service file. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// The workflow service host 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            string workflowServiceFile, Uri serviceUri, InstanceStore instanceStore, params IServiceBehavior[] behaviors)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(workflowServiceFile));
            if (string.IsNullOrWhiteSpace(workflowServiceFile))
            {
                throw new ArgumentNullException("workflowServiceFile");
            }

            Contract.Requires(serviceUri != null);
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            return Open((WorkflowService)XamlServices.Load(workflowServiceFile), serviceUri, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="stream">
        /// The stream. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            Stream stream, Uri serviceUri, InstanceStore instanceStore, params IServiceBehavior[] behaviors)
        {
            Contract.Requires(stream != null);
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Contract.Requires(serviceUri != null);
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            return Open((WorkflowService)XamlServices.Load(stream), serviceUri, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="stream">
        /// The stream. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance store 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            Stream stream, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(stream != null);
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open((WorkflowService)XamlServices.Load(stream), serviceEndpoint, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="textReader">
        /// The text reader. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            TextReader textReader, Uri serviceUri, InstanceStore instanceStore, params IServiceBehavior[] behaviors)
        {
            Contract.Requires(textReader != null);
            if (textReader == null)
            {
                throw new ArgumentNullException("textReader");
            }

            Contract.Requires(serviceUri != null);
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            return Open((WorkflowService)XamlServices.Load(textReader), serviceUri, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="textReader">
        /// The text reader. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            TextReader textReader, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(textReader != null);
            if (textReader == null)
            {
                throw new ArgumentNullException("textReader");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open((WorkflowService)XamlServices.Load(textReader), serviceEndpoint, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="xamlReader">
        /// The XAML reader. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            XamlReader xamlReader, Uri serviceUri, InstanceStore instanceStore, params IServiceBehavior[] behaviors)
        {
            Contract.Requires(xamlReader != null);
            if (xamlReader == null)
            {
                throw new ArgumentNullException("xamlReader");
            }

            Contract.Requires(serviceUri != null);
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            return Open((WorkflowService)XamlServices.Load(xamlReader), serviceUri, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="xamlReader">
        /// The XAML reader. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            XamlReader xamlReader, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(xamlReader != null);
            if (xamlReader == null)
            {
                throw new ArgumentNullException("xamlReader");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open((WorkflowService)XamlServices.Load(xamlReader), serviceEndpoint, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="xmlReader">
        /// The xml reader. 
        /// </param>
        /// <param name="serviceUri">
        /// The service URI. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            XmlReader xmlReader, Uri serviceUri, InstanceStore instanceStore, params IServiceBehavior[] behaviors)
        {
            Contract.Requires(xmlReader != null);
            if (xmlReader == null)
            {
                throw new ArgumentNullException("xmlReader");
            }

            Contract.Requires(serviceUri != null);
            if (serviceUri == null)
            {
                throw new ArgumentNullException("serviceUri");
            }

            return Open((WorkflowService)XamlServices.Load(xmlReader), serviceUri, instanceStore, behaviors);
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="xmlReader">
        /// The xml reader. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <param name="instanceStore">
        /// The instance Store. 
        /// </param>
        /// <param name="behaviors">
        /// The behaviors. 
        /// </param>
        /// <returns>
        /// An opened WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(
            XmlReader xmlReader, 
            EndpointAddress serviceEndpoint, 
            InstanceStore instanceStore, 
            params IServiceBehavior[] behaviors)
        {
            Contract.Requires(xmlReader != null);
            if (xmlReader == null)
            {
                throw new ArgumentNullException("xmlReader");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open((WorkflowService)XamlServices.Load(xmlReader), serviceEndpoint, instanceStore, behaviors);
        }

        /// <summary>
        /// Creates and Opens a WorkflowServiceTestHost
        /// </summary>
        /// <param name="workflowService">
        /// The workflow service. 
        /// </param>
        /// <param name="serviceEndpoint">
        /// The service endpoint. 
        /// </param>
        /// <returns>
        /// The WorkflowServiceTestHost 
        /// </returns>
        public static WorkflowServiceTestHost Open(WorkflowService workflowService, EndpointAddress serviceEndpoint)
        {
            Contract.Requires(workflowService != null);
            if (workflowService == null)
            {
                throw new ArgumentNullException("workflowService");
            }

            Contract.Requires(serviceEndpoint != null);
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint");
            }

            return Open(workflowService, serviceEndpoint.Uri);
        }

        /// <summary>
        ///   Closes the host
        /// </summary>
        public void Close()
        {
            if (this.Host != null)
            {
                this.Host.Close();
                this.WaitForHostClosed();
            }
        }

        /// <summary>
        ///   The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///   Determines if the host is open
        /// </summary>
        /// <returns> true if the host is open. </returns>
        public bool IsOpen()
        {
            return this.Host != null && this.Host.State == CommunicationState.Opened;
        }

        /// <summary>
        ///   Opens the host
        /// </summary>
        public void Open()
        {
            if (this.IsOpen())
            {
                throw new InvalidOperationException("Host is already open");
            }

            // Add the idle behavior
            Debug.Assert(this.Host != null, "Host is null");
            Debug.Assert(this.Host.WorkflowExtensions != null, "Host.WorkflowExtensions is null");
            this.Host.WorkflowExtensions.Add(
                new WorkflowIdleBehavior { TimeToPersist = this.TimeToPersist, TimeToUnload = this.TimeToUnload });

            // Add the instance store
            Debug.Assert(this.Host.DurableInstancingOptions != null, "Host.DurableInstancingOptions is null");
            this.Host.DurableInstancingOptions.InstanceStore = this.InstanceStore;

            WorkflowTrace.Information("Host: Opening {0}", this.ServiceUri);
            this.Host.Open();
        }

        /// <summary>
        /// Waits for the host to be closed
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the host closed before the timeout 
        /// </returns>
        public bool WaitForHostClosed(int timeout = DefaultTimeout)
        {
            Debug.Assert(this.hostClosedEvent != null, "Host closed event is null");
            var result = this.hostClosedEvent.WaitOne(timeout);
            this.hostClosedEvent.Reset();
            return result;
        }

        /// <summary>
        /// Waits for the workflow instance to be deleted
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the instance was deleted before the timeout 
        /// </returns>
        public bool WaitForInstanceDeleted(int timeout = DefaultTimeout)
        {
            Debug.Assert(this.deletedEvent != null, "deletedEvent is null");
            var result = this.deletedEvent.WaitOne(timeout);
            this.deletedEvent.Reset();
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        public bool WaitForInstanceDeleted(TimeSpan timeout)
        {
            Debug.Assert(this.unloadedEvent != null, "this.unloadedEvent is null");
            var result = this.unloadedEvent.WaitOne(timeout);
            this.unloadedEvent.Reset();
            return result;
        }

        /// <summary>
        /// Waits for the instance to be unloaded.
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the instance was deleted before the timeout 
        /// </returns>
        public bool WaitForInstanceUnloaded(int timeout = DefaultTimeout)
        {
            Debug.Assert(this.unloadedEvent != null, "this.unloadedEvent is null");
            var result = this.unloadedEvent.WaitOne(timeout);
            this.unloadedEvent.Reset();
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Adds the service debug behavior
        /// </summary>
        private void AddServiceDebugBehavior()
        {
            Debug.Assert(this.Host != null, "Host is null");
            Debug.Assert(this.Host.Description != null, "Host.Description is null");
            Debug.Assert(this.Host.Description.Behaviors != null, "Host.Description.Behaviors is null");
            var serviceDebug = this.Host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (serviceDebug == null)
            {
                this.Host.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                serviceDebug.IncludeExceptionDetailInFaults = true;
            }
        }

        /// <summary>
        ///   Creates the workflow service host.
        /// </summary>
        private void CreateWorkflowServiceHost()
        {
            this.Host = new WorkflowServiceHost(this.WorkflowService, new[] { this.ServiceUri });

            // Setup the tracking participant to signal when the WorkflowInstance enters states
            Debug.Assert(this.Tracking != null, "Tracking is null");
            this.Tracking.WaitForWorkflowInstanceRecord(WorkflowInstanceRecordState.Deleted, this.deletedEvent);
            this.Tracking.WaitForWorkflowInstanceRecord(WorkflowInstanceRecordState.Unloaded, this.unloadedEvent);

            Debug.Assert(this.WorkflowExtensions != null, "WorkflowExtensions is null");
            this.WorkflowExtensions.Add(this.Tracking);
            this.AddServiceDebugBehavior();

            Debug.Assert(this.Host != null, "Host is null");
            this.Host.Closed += (o, e) =>
                    this.hostClosedEvent.Set();
        }

        /// <summary>
        /// Disposes of the WorkflowServiceTestHost
        /// </summary>
        /// <param name="disposing">
        /// The disposing. 
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.Close();

            if (this.deletedEvent != null)
            {
                this.deletedEvent.Close();
            }

            if (this.hostClosedEvent != null)
            {
                this.hostClosedEvent.Close();
            }

            if (this.unloadedEvent != null)
            {
                this.unloadedEvent.Close();
            }
        }

        #endregion
    }
}