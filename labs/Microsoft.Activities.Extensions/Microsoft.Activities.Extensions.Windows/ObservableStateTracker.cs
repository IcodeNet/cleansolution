namespace Microsoft.Activities.Extensions.Windows
{
    using System.Activities;
    using System.ComponentModel;

    using Microsoft.Activities.Extensions.Tracking;

    ///// <summary>
    /////   An observable state tracker
    ///// </summary>
    //public class ObservableStateTracker : StateTracker, INotifyPropertyChanged
    //{
    //    #region Constructors and Destructors

    //    /// <summary>
    //    ///   Initializes a new instance
    //    /// </summary>
    //    /// <param name="rootActivity"> The root activity </param>
    //    /// <param name="maxHistory"> The max history </param>
    //    public ObservableStateTracker(Activity rootActivity, int maxHistory = DefaultMaxHistory)
    //        : base(rootActivity, maxHistory)
    //    {
    //    }

    //    #endregion

    //    #region Public Events

    //    /// <summary>
    //    ///   Occurs when a property value changes.
    //    /// </summary>
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    #endregion

    //    ///// <summary>
    //    /////   Returns the current state of the tracker
    //    ///// </summary>
    //    //public override string CurrentState
    //    //{
    //    //    get
    //    //    {
    //    //        return base.CurrentState;
    //    //    }

    //    //    protected set
    //    //    {
    //    //        base.CurrentState = value;
    //    //        this.OnPropertyChanged("CurrentState");
    //    //    }
    //    //}

    //    ///// <summary>
    //    ///// Returns the previous state
    //    ///// </summary>
    //    //public override string PreviousState
    //    //{
    //    //    get
    //    //    {
    //    //        return base.PreviousState;
    //    //    }

    //    //    protected set
    //    //    {
    //    //        base.PreviousState = value;
    //    //        this.OnPropertyChanged("PreviousState");
    //    //    }
    //    //}

    //    ///// <summary>
    //    ///// Gets the name of the last known state machine
    //    ///// </summary>
    //    //public override string CurrentStateMachine
    //    //{
    //    //    get
    //    //    {
    //    //        return base.CurrentStateMachine;
    //    //    }

    //    //    protected set
    //    //    {
    //    //        base.CurrentStateMachine = value;
    //    //        this.OnPropertyChanged("CurrentStateMachine");
    //    //    }
    //    //}

    //    #region Methods

    //    /// <summary>
    //    ///   Raises a PropertyChanged event
    //    /// </summary>
    //    /// <param name="propertyName"> The property that changed </param>
    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        var handler = this.PropertyChanged;
    //        if (handler != null)
    //        {
    //            handler(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //    #endregion
    //}
}