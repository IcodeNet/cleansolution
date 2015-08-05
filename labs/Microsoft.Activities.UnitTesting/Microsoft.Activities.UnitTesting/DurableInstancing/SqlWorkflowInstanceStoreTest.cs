namespace Microsoft.Activities.UnitTesting.DurableInstancing
{
    using System;
    using System.Activities.DurableInstancing;
    using System.Runtime.DurableInstancing;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.DurableInstancing;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   A test instance store
    /// </summary>
    public class SqlWorkflowInstanceStoreTest : SqlDatabaseTest
    {
        #region Fields

        /// <summary>
        ///   The instance store
        /// </summary>
        private SqlWorkflowInstanceStore instanceStore;

        /// <summary>
        /// The instance view
        /// </summary>
        private InstanceView instanceView = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlWorkflowInstanceStoreTest" /> class.
        /// </summary>
        public SqlWorkflowInstanceStoreTest()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlWorkflowInstanceStoreTest" /> class.
        /// </summary>
        /// <param name="databaseName"> The database name. </param>
        public SqlWorkflowInstanceStoreTest(string databaseName)
            : base(databaseName)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlWorkflowInstanceStoreTest" /> class.
        /// </summary>
        /// <param name="databaseName"> The database name. </param>
        /// <param name="dataSource"> The data source. </param>
        public SqlWorkflowInstanceStoreTest(string databaseName, string dataSource)
            : base(databaseName, dataSource)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the instance store
        /// </summary>
        public SqlWorkflowInstanceStore InstanceStore
        {
            get
            {
                return this.instanceStore ?? (this.instanceStore = new SqlWorkflowInstanceStore(this.ConnectionString));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Asserts that the database scripts have run
        /// </summary>
        /// <remarks>
        ///   This does not check everything
        /// </remarks>
        public void AssertDatabaseIsConfigured()
        {
            this.AssertProcs();
            this.AssertTables();
            this.AssertViews();
        }

        /// <summary>
        ///   Asserts that the procedures exists
        /// </summary>
        public void AssertProcs()
        {
            AssertHelper.IsTrue(
                this.ProcedureExists,
                "InsertRunnableInstanceEntry",
                "Could not find StoredProcedure InsertRunnableInstanceEntry");
            AssertHelper.IsTrue(
                this.ProcedureExists, "RecoverInstanceLocks", "Could find StoredProcedure RecoverInstanceLocks");
            AssertHelper.IsTrue(
                this.ProcedureExists, "InsertDefinitionIdentity", "Could find StoredProcedure InsertDefinitionIdentity");
            AssertHelper.IsTrue(this.ProcedureExists, "CreateLockOwner", "Could find StoredProcedure CreateLockOwner");
            AssertHelper.IsTrue(this.ProcedureExists, "DeleteLockOwner", "Could find StoredProcedure DeleteLockOwner");
            AssertHelper.IsTrue(this.ProcedureExists, "ExtendLock", "Could find StoredProcedure ExtendLock");
            AssertHelper.IsTrue(this.ProcedureExists, "AssociateKeys", "Could find StoredProcedure AssociateKeys");
            AssertHelper.IsTrue(this.ProcedureExists, "CompleteKeys", "Could find StoredProcedure CompleteKeys");
            AssertHelper.IsTrue(this.ProcedureExists, "FreeKeys", "Could find StoredProcedure FreeKeys");
            AssertHelper.IsTrue(this.ProcedureExists, "CreateInstance", "Could find StoredProcedure CreateInstance");
            AssertHelper.IsTrue(this.ProcedureExists, "LockInstance", "Could find StoredProcedure LockInstance");
            AssertHelper.IsTrue(this.ProcedureExists, "UnlockInstance", "Could find StoredProcedure UnlockInstance");
            AssertHelper.IsTrue(
                this.ProcedureExists, "DetectRunnableInstances", "Could find StoredProcedure DetectRunnableInstances");
            AssertHelper.IsTrue(
                this.ProcedureExists,
                "GetActivatableWorkflowsActivationParameters",
                "Could find StoredProcedure GetActivatableWorkflowsActivationParameters");
            AssertHelper.IsTrue(this.ProcedureExists, "LoadInstance", "Could find StoredProcedure LoadInstance");
            AssertHelper.IsTrue(
                this.ProcedureExists, "TryLoadRunnableInstance", "Could find StoredProcedure TryLoadRunnableInstance");
            AssertHelper.IsTrue(this.ProcedureExists, "DeleteInstance", "Could find StoredProcedure DeleteInstance");
            AssertHelper.IsTrue(
                this.ProcedureExists, "CreateServiceDeployment", "Could find StoredProcedure CreateServiceDeployment");
            AssertHelper.IsTrue(this.ProcedureExists, "SaveInstance", "Could find StoredProcedure SaveInstance");
            AssertHelper.IsTrue(
                this.ProcedureExists, "InsertPromotedProperties", "Could find StoredProcedure InsertPromotedProperties");
            AssertHelper.IsTrue(
                this.ProcedureExists,
                "GetWorkflowInstanceStoreVersion",
                "Could find StoredProcedure GetWorkflowInstanceStoreVersion");
        }

        /// <summary>
        ///   Asserts that the tables exist
        /// </summary>
        public void AssertTables()
        {
            AssertHelper.IsTrue(this.TableExists, "InstancesTable", "Could not find InstancesTable");
            AssertHelper.IsTrue(this.TableExists, "RunnableInstancesTable", "Could not find RunnableInstancesTable");
            AssertHelper.IsTrue(this.TableExists, "KeysTable", "Could not find KeysTable");
            AssertHelper.IsTrue(this.TableExists, "LockOwnersTable", "Could not find LockOwnersTable");
            AssertHelper.IsTrue(
                this.TableExists, "InstanceMetadataChangesTable", "Could not find InstanceMetadataChangesTable");
            AssertHelper.IsTrue(this.TableExists, "ServiceDeploymentsTable", "Could not find ServiceDeploymentsTable");
            AssertHelper.IsTrue(
                this.TableExists, "InstancePromotedPropertiesTable", "Could not find InstancePromotedPropertiesTable");
            AssertHelper.IsTrue(
                this.TableExists,
                "SqlWorkflowInstanceStoreVersionTable",
                "Could not find SqlWorkflowInstanceStoreVersionTable");
            AssertHelper.IsTrue(this.TableExists, "DefinitionIdentityTable", "Could not find DefinitionIdentityTable");
            AssertHelper.IsTrue(this.TableExists, "IdentityOwnerTable", "Could not find IdentityOwnerTable");
            AssertHelper.IsTrue(this.TableExists, "InstancesTable", "Could not find InstancesTable");
        }

        /// <summary>
        ///   Asserts that the views exist
        /// </summary>
        public void AssertViews()
        {
            AssertHelper.IsTrue(this.ViewExists, "ServiceDeployments", "Could not find ServiceDeployments");
            AssertHelper.IsTrue(
                this.ViewExists, "InstancePromotedProperties", "Could not find InstancePromotedProperties");
            AssertHelper.IsTrue(this.ViewExists, "Instances", "Could not find Instances");
        }

        /// <summary>
        ///   Runs the schema and logic scripts on an existing test database
        /// </summary>
        public void ConfigureInstanceStore()
        {
            SqlWorkflowInstanceStoreManager.ConfigureInstanceStore(this.DatabaseName, this.ConnectionString);
        }

        /// <summary>
        ///   Creates an instance store
        /// </summary>
        /// <param name="dropIfExists"> Drop the database if it exists </param>
        /// <returns> The SqlWorkflowInstanceStore. </returns>
        public SqlWorkflowInstanceStore CreateInstanceStore(bool dropIfExists = false)
        {
            this.instanceStore = SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                this.DatabaseName, this.ConnectionString, dropIfExists);
            return this.instanceStore;
        }


        /// <summary>
        ///   Initializes the instance store
        /// </summary>
        public void CreateOwner()
        {
            using (var instanceHandle = new DisposableInstanceHandle(this.InstanceStore))
            {
                this.instanceView = this.InstanceStore.Execute(
                    instanceHandle, new CreateWorkflowOwnerCommand(), Global.Timeout);
                this.InstanceStore.DefaultInstanceOwner = this.instanceView.InstanceOwner;
            }
        }

        /// <summary>
        ///   Delete the owner
        /// </summary>
        public void DeleteOwner()
        {
            using (var instanceHandle = new DisposableInstanceHandle(this.InstanceStore))
            {
                this.InstanceStore.Execute(instanceHandle, new DeleteWorkflowOwnerCommand(), Global.Timeout);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Dispose of the resource
        /// </summary>
        /// <param name="disposing"> The disposing flag. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.instanceView != null)
                {
                    this.DeleteOwner();
                }

                base.Dispose(true);
            }
        }

        #endregion
    }
}