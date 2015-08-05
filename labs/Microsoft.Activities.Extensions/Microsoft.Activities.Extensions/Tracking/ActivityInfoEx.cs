// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityInfoEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities;
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   The activity info ex.
    /// </summary>
    public static class ActivityInfoEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get activity.
        /// </summary>
        /// <param name="activityInfo">
        /// The activity info. 
        /// </param>
        /// <returns>
        /// The System.Activities.Activity. 
        /// </returns>
        public static Activity GetActivity(this ActivityInfo activityInfo)
        {
            dynamic activityObj = new ReflectionObject(activityInfo);
            return activityObj.Activity;
        }

        /// <summary>
        /// Returns the id as a string or null if the activity info is null
        /// </summary>
        /// <param name="activityInfo">
        /// The activity info. 
        /// </param>
        /// <returns>
        /// the id as a string or null if the activity info is null 
        /// </returns>
        public static string GetId(this ActivityInfo activityInfo)
        {
            return activityInfo != null ? activityInfo.Id : Constants.Null;
        }

        /// <summary>
        /// Returns the id as a string or null if the activity info is null
        /// </summary>
        /// <param name="activityInfo">
        /// The activity info. 
        /// </param>
        /// <returns>
        /// the id as a string or null if the activity info is null 
        /// </returns>
        public static string GetName(this ActivityInfo activityInfo)
        {
            return activityInfo != null ? activityInfo.Name : Constants.Null;
        }

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="activityInfo">
        /// The activity info. 
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this ActivityInfo activityInfo, string name = "Activity", int tabs = 0)
        {
            return new TraceStringBuilder(
                tabs, "{0} [{1}] \"{2}\"", name, GetId(activityInfo), GetName(activityInfo)).ToString();
        }

        #endregion
    }
}