// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizedDayOfWeekExtension.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System;
    using System.Globalization;
    using System.Windows.Markup;

    /// <summary>
    /// The localized day of week extension.
    /// </summary>
    internal class LocalizedDayOfWeekExtension : MarkupExtension
    {
        #region Properties

        /// <summary>
        /// Gets or sets Day.
        /// </summary>
        public DayOfWeek Day { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides a value.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)this.Day];
        }

        #endregion
    }
}