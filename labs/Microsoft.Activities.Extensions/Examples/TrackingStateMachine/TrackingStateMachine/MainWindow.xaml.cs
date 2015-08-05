namespace TrackingStateMachine
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.Activities.Extensions;

    using TrackingStateMachine.Activities;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : INotifyPropertyChanged, IWorkflowView, IDisposable
    {
        #region Fields

        /// <summary>
        ///   Waits on the closing tasks
        /// </summary>
        private readonly TaskWaiter closingTasks = new TaskWaiter();

        /// <summary>
        ///   The model.
        /// </summary>
        private readonly WorkflowModel model;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.NewCommand = new RelayCommand(this.ExecuteNew);
            this.LoadCommand = new RelayCommand(this.ExecuteLoad, this.CanExecuteLoad);
            this.OpenCommand = new RelayCommand(this.ExecuteOpen);
            this.ClearCommand = new RelayCommand(this.ExecuteClear, this.CanExecuteClear);
            this.UnloadCommand = new RelayCommand(this.ExecuteUnload, this.CanExecuteUnload);
            this.TriggerCommand = new RelayCommand<StateTrigger>(this.ExecuteTrigger, this.CanExecuteTrigger);

            this.Loaded += this.MainWindowLoaded;
            this.TrackingBuilder = new StringBuilder();
            var textListener = new TextListener(this);

            // Trace.Listeners.Add(textListener);
            Trace.Listeners.Add(textListener);
            WorkflowTrace.Source.Listeners.Add(textListener);

            this.model = new WorkflowModel(this);
            this.Closing += this.ViewClosing;
            this.Closed += this.ViewClosed;

            this.InitializeComponent();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the Clear command
        /// </summary>
        public ICommand ClearCommand { get; private set; }

        /// <summary>
        ///   Gets the Load command
        /// </summary>
        public ICommand LoadCommand { get; private set; }

        /// <summary>
        ///   Gets Model.
        /// </summary>
        public WorkflowModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        ///   Gets the New command
        /// </summary>
        public ICommand NewCommand { get; private set; }

        /// <summary>
        ///   Gets the Load command
        /// </summary>
        public ICommand OpenCommand { get; private set; }

        /// <summary>
        ///   Gets Tracking.
        /// </summary>
        public string Tracking
        {
            get
            {
                return this.TrackingBuilder != null ? this.TrackingBuilder.ToString() : null;
            }
        }

        /// <summary>
        ///   Gets the Load command
        /// </summary>
        public ICommand TriggerCommand { get; private set; }

        /// <summary>
        ///   Gets the Unload command
        /// </summary>
        public ICommand UnloadCommand { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the TrackingBuilder.
        /// </summary>
        private StringBuilder TrackingBuilder { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Disposes of the view
        /// </summary>
        public void Dispose()
        {
            if (this.closingTasks != null)
            {
                this.closingTasks.Dispose();
            }
        }

        /// <summary>
        ///   The notify changed.
        /// </summary>
        /// <param name="property"> The property. </param>
        public void NotifyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        ///   The refesh workflow state.
        /// </summary>
        public void Refresh()
        {
            // Update the command bindings
            RequeryCommands();

            // Update the current state
            if (this.listInstances != null)
            {
                Dispatch(() => this.listInstances.Focus());
            }
        }

        /// <summary>
        ///   Dispatch the removal of a workflow
        /// </summary>
        /// <param name="i"> The index </param>
        public void RemoveWorkflow(int i)
        {
            Dispatch(() => this.Model.Workflows.RemoveAt(i));
        }

        /// <summary>
        ///   The view is closed
        /// </summary>
        /// <param name="sender"> The view The sender. </param>
        /// <param name="e"> The event args. </param>
        public void ViewClosed(object sender, EventArgs e)
        {
            this.closingTasks.Wait();
        }

        /// <summary>
        ///   The view is closing
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event args. </param>
        public void ViewClosing(object sender, CancelEventArgs e)
        {
            foreach (var workflowInstance in
                this.Model.Workflows.Where(
                    workflowInstance =>
                    workflowInstance != null && workflowInstance.Host != null && !workflowInstance.Host.IsAborted()))
            {
                this.closingTasks.Add(
                    Task.Factory.FromAsync(workflowInstance.Host.BeginUnload, workflowInstance.Host.EndUnload, null));
            }
        }

        /// <summary>
        ///   The write exception.
        /// </summary>
        /// <param name="exception"> The exception. </param>
        /// <param name="error"> The error. </param>
        public void WriteException(Exception exception, string error)
        {
            WorkflowTrace.Information(error);
            WorkflowTrace.Information(exception.Message);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The dispatch.
        /// </summary>
        /// <param name="action"> The action. </param>
        private static void Dispatch(Action action)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        ///   The requery commands.
        /// </summary>
        private static void RequeryCommands()
        {
            Dispatch(CommandManager.InvalidateRequerySuggested);
        }

        /// <summary>
        ///   The can execute clear.
        /// </summary>
        /// <param name="obj"> The obj. </param>
        /// <returns> True if clear can execute </returns>
        private bool CanExecuteClear(object obj)
        {
            return this.TrackingBuilder.Length > 0;
        }

        /// <summary>
        ///   Determines if the Load command can be executed
        /// </summary>
        /// <param name="obj"> The obj. </param>
        /// <returns> true, if the Load command is enabled, false if not. </returns>
        private bool CanExecuteLoad(object obj)
        {
            return this.Model.SelectedIndex != -1 && !this.Model.IsLoaded;
        }

        /// <summary>
        ///   Determines if a trigger can execute
        /// </summary>
        /// <param name="trigger"> The trigger </param>
        /// <returns> true if it can execute, false if not </returns>
        private bool CanExecuteTrigger(StateTrigger trigger)
        {
            return this.Model.CanExecute(trigger);
        }

        /// <summary>
        ///   Determines if the Unload command can be executed
        /// </summary>
        /// <param name="obj"> The obj. </param>
        /// <returns> true, if the Unload command is enabled, false if not. </returns>
        private bool CanExecuteUnload(object obj)
        {
            return this.Model.IsLoaded;
        }

        /// <summary>
        ///   The execute clear.
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private void ExecuteClear(object obj)
        {
            this.TrackingBuilder.Clear();
            this.NotifyChanged("Tracking");
        }

        /// <summary>
        ///   Executes a Load workflow
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private void ExecuteLoad(object obj)
        {
            this.Model.Load();
            this.listInstances.Focus();
            this.listInstances.ScrollIntoView(this.listInstances.SelectedItem);
        }

        /// <summary>
        ///   Executes a new workflow
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private void ExecuteNew(object obj)
        {
            this.Model.New();
            this.listInstances.Focus();
            this.listInstances.ScrollIntoView(this.listInstances.SelectedItem);
        }

        /// <summary>
        ///   Executes a Open workflow
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private void ExecuteOpen(object obj)
        {
            var dialog = new DialogOpen();
            var result = dialog.ShowDialog();
            if (result.GetValueOrDefault(false))
            {
                Guid instanceId;
                if (Guid.TryParse(dialog.textId.Text, out instanceId))
                {
                    this.Model.Open(instanceId);
                }
                else
                {
                    MessageBox.Show(this, "Invalid Guid", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }

            this.listInstances.Focus();
            this.listInstances.ScrollIntoView(this.listInstances.SelectedItem);
        }

        /// <summary>
        ///   Executes a trigger on the state machine
        /// </summary>
        /// <param name="trigger"> The trigger </param>
        private void ExecuteTrigger(StateTrigger trigger)
        {
            this.Model.ResumeBookmark(trigger);
        }

        /// <summary>
        ///   Executes a Unload workflow
        /// </summary>
        /// <param name="obj"> The obj. </param>
        private void ExecuteUnload(object obj)
        {
            this.Model.Unload();
            this.listInstances.Focus();
            this.listInstances.ScrollIntoView(this.listInstances.SelectedItem);
        }

        /// <summary>
        ///   The main window has been loaded
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.Model.LoadInstances(this);
        }

        /// <summary>
        ///   The scroll to end.
        /// </summary>
        private void ScrollToEnd()
        {
            if (this.ScrollViewerTrace != null)
            {
                Dispatch(this.ScrollViewerTrace.ScrollToEnd);
            }
        }

        /// <summary>
        ///   The track.
        /// </summary>
        /// <param name="message"> The message. </param>
        private void Track(string message)
        {
            this.TrackingBuilder.AppendLine(message);
            this.NotifyChanged("Tracking");
            this.ScrollToEnd();
        }

        #endregion

        /// <summary>
        ///   The text listener.
        /// </summary>
        private class TextListener : TraceListener
        {
            #region Constants

            /// <summary>
            /// Leading tag for traces
            /// </summary>
            private const string MicrosoftActivitiesInformation = "Microsoft.Activities.Extensions Information: 0 : ";

            #endregion

            #region Fields

            /// <summary>
            ///   The main window.
            /// </summary>
            private readonly MainWindow mainWindow;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///   Initializes a new instance of the <see cref="TextListener" /> class.
            /// </summary>
            /// <param name="mainWindow"> The main window. </param>
            internal TextListener(MainWindow mainWindow)
            {
                this.mainWindow = mainWindow;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///   When overridden in a derived class, writes the specified message to the listener you create in the derived class.
            /// </summary>
            /// <param name="message"> A message to write. </param>
            /// <filterpriority>2</filterpriority>
            public override void Write(string message)
            {
                if (IgnoreMessage(message))
                {
                    return;
                }

                if (message.StartsWith(MicrosoftActivitiesInformation))
                {
                    message = message.Substring(MicrosoftActivitiesInformation.Length);
                }

                this.mainWindow.Track(message);
            }

            /// <summary>
            ///   When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
            /// </summary>
            /// <param name="message"> A message to write. </param>
            /// <filterpriority>2</filterpriority>
            public override void WriteLine(string message)
            {
                if (IgnoreMessage(message))
                {
                    return;
                }

                if (message.StartsWith(MicrosoftActivitiesInformation))
                {
                    message = message.Substring(MicrosoftActivitiesInformation.Length);
                }

                this.mainWindow.Track(message);
            }

            #endregion

            #region Methods

            /// <summary>
            ///   The ignore message.
            /// </summary>
            /// <param name="message"> The message. </param>
            /// <returns> The System.Boolean. </returns>
            private static bool IgnoreMessage(string message)
            {
                return message.StartsWith("WorkflowDebugger")
                       || message.StartsWith("Instrumentation for debugger fails");
            }

            #endregion
        }
    }
}