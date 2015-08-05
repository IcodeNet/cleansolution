namespace Microsoft.Activities.Extensions.Tests
{
    //using System;
    //using System.Collections.ObjectModel;
    //using System.ComponentModel;
    //using System.Diagnostics.Contracts;

    //using Microsoft.Activities.Extensions.Tracking;
    //using Microsoft.Activities.Extensions.Windows;

    ///// <summary>
    ///// Observes change notifications from a StateTracker
    ///// </summary>
    //public class StateTrackerObserver
    //{
    //    #region Fields

    //    /// <summary>
    //    /// The property changed notifications
    //    /// </summary>
    //    private readonly Collection<PropertyChangedEventArgs> propertyChangedArgs =
    //        new Collection<PropertyChangedEventArgs>();

    //    #endregion

    //    #region Constructors and Destructors

    //    /// <summary>
    //    /// Initializes a new instance
    //    /// </summary>
    //    /// <param name="tracker">The tracker</param>
    //    public StateTrackerObserver(ObservableStateTracker tracker)
    //    {
    //        Contract.Requires(tracker != null);
    //        if (tracker == null)
    //        {
    //            throw new ArgumentNullException("tracker");
    //        }

    //        tracker.PropertyChanged += (sender, args) => this.propertyChangedArgs.Add(args);
    //    }

    //    #endregion

    //    #region Public Properties

    //    /// <summary>
    //    /// Gets the PropertyChangedArgs
    //    /// </summary>
    //    public ReadOnlyCollection<PropertyChangedEventArgs> PropertyChangedArgs
    //    {
    //        get
    //        {
    //            return new ReadOnlyCollection<PropertyChangedEventArgs>(this.propertyChangedArgs);
    //        }
    //    }

    //    #endregion

    //    /// <summary>
    //    /// Outputs a trace of the observer
    //    /// </summary>
    //    public void Trace()
    //    {
    //        var tsb = new TraceStringBuilder() { Options = TraceOptions.ShowCollectionCount };
    //        tsb.AppendLine(this.GetType().FullName);
    //        tsb.AppendCollection("PropertyChangedArgs", this.PropertyChangedArgs, (args, i) => args.PropertyName);
    //        WorkflowTrace.Information(tsb.ToString());
    //    }
    //}
}