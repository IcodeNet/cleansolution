// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilTimeViewModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System;
    using System.Activities.Presentation.Model;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The delay until time view model.
    /// </summary>
    public class DelayUntilTimeViewModel : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The model item.
        /// </summary>
        private readonly ModelItem modelItem;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayUntilTimeViewModel"/> class.
        /// </summary>
        /// <param name="modelItem">
        /// The model item.
        /// </param>
        public DelayUntilTimeViewModel(ModelItem modelItem)
        {
            this.modelItem = modelItem;
            var occurrenceDays = this.modelItem.Properties[DelayUntilTimeDesigner.OccurenceDays];

            if (occurrenceDays != null)
            {
                if (occurrenceDays.Collection != null)
                {
                    occurrenceDays.Collection.CollectionChanged += this.OnFlagsCollectionChanged;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot find property " + DelayUntilTimeDesigner.OccurenceDays);
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///   The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether IsFridayChecked.
        /// </summary>
        public bool IsFridayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Friday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Friday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsMondayChecked.
        /// </summary>
        public bool IsMondayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Monday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Monday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsSaturdayChecked.
        /// </summary>
        public bool IsSaturdayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Saturday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Saturday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsSundayChecked.
        /// </summary>
        public bool IsSundayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Sunday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Sunday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsThursdayChecked.
        /// </summary>
        public bool IsThursdayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Thursday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Thursday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsTuesdayChecked.
        /// </summary>
        public bool IsTuesdayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Tuesday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Tuesday);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsWednesdayChecked.
        /// </summary>
        public bool IsWednesdayChecked
        {
            get
            {
                return this.GetDayOfWeekChecked(DayOfWeek.Wednesday);
            }

            set
            {
                this.SetDayOfWeekChecked(value, DayOfWeek.Wednesday);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get day of week checked.
        /// </summary>
        /// <param name="day">
        /// The day of week.
        /// </param>
        /// <returns>
        /// true if the day is in the collection
        /// </returns>
        private bool GetDayOfWeekChecked(DayOfWeek day)
        {
            var occurenceDays = this.GetOccurenceDays();
            return occurenceDays.Contains(day);
        }

        /// <summary>
        /// The get occurence days.
        /// </summary>
        /// <returns>
        /// The occurence days collection
        /// </returns>
        private ModelItemCollection GetOccurenceDays()
        {
            var property = this.modelItem.Properties[DelayUntilTimeDesigner.OccurenceDays];
            if (property != null)
            {
                return property.Collection;
            }

            throw new InvalidOperationException("Cannot find property " + DelayUntilTimeDesigner.OccurenceDays);
        }

        /// <summary>
        /// The notify property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The on flags collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void OnFlagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged("IsSundayChecked");
            this.NotifyPropertyChanged("IsMondayChecked");
            this.NotifyPropertyChanged("IsTuesdayChecked");
            this.NotifyPropertyChanged("IsWednesdayChecked");
            this.NotifyPropertyChanged("IsThursdayChecked");
            this.NotifyPropertyChanged("IsFridayChecked");
            this.NotifyPropertyChanged("IsSaturdayChecked");
        }

        /// <summary>
        /// The set day of week checked.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="day">
        /// The day of week.
        /// </param>
        private void SetDayOfWeekChecked(bool value, DayOfWeek day)
        {
            Contract.Requires(this.modelItem != null);
            var occurenceDays = this.GetOccurenceDays();
            if (value)
            {
                occurenceDays.Add(day);
            }
            else
            {
                for (int i = 0; i < occurenceDays.Count; i++)
                {
                    if (occurenceDays[i].GetCurrentValue().Equals(day))
                    {
                        occurenceDays.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}