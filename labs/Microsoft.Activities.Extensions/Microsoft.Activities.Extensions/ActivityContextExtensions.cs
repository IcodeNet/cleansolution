namespace Microsoft.Activities.Extensions
{
    using System.Activities;
    using System.Activities.Tracking;

    /// <summary>
    ///   The activity context extensions.
    /// </summary>
    public static class ActivityContextExtensions
    {
        #region Public Methods

        /// <summary>
        ///   Gets the value of an argument or the default value if there is no expression
        /// </summary>
        /// <param name = "context">
        ///   The context.
        /// </param>
        /// <param name = "argument">
        ///   The argument.
        /// </param>
        /// <param name = "defaultValue">
        ///   The default value.
        /// </param>
        /// <typeparam name = "T">
        ///   The type of the argument
        /// </typeparam>
        /// <returns>
        ///   the value of an argument or the default value if there is no expression
        /// </returns>
        /// <remarks>
        ///   Default values are not supported on InOutArguments because we cannot tell if the value was set by your code or 
        ///   if it is simply the default(T) because no value was set
        /// </remarks>
        /// <example>
        ///   An activity that has an optional in argument
        ///   <code source = "Examples\CSDocExamples\ActivityWithOptionalArgsContext.cs" lang = "CSharp">
        ///   </code>
        /// </example>
        public static T GetValue<T>(this ActivityContext context, InArgument<T> argument, T defaultValue)
        {
            var value = context.GetValue(argument);

            // If the value is the default value and the user did not supply an expression)
            if (Equals(value, default(T)) && argument.Expression == null)
            {
                // Use the default value instead
                value = defaultValue;
            }

            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Adds a tracking record that is not a CustomTrackingRecord
        /// </summary>
        /// <param name = "context">The activity context</param>
        /// <param name = "record">The tracking record</param>
        /// <remarks>
        ///   This method uses reflection to access a private member of the ActivityContext class
        ///   This is done in order to support adding TrackingRecords of any type so that InvokeWorkflow can
        ///   provide tracking of inner workflows.
        ///   Using Reflection like this is dangerous because it accesses something outside of the public 
        ///   interface of a class and may easily be broken in future versions.
        ///   This extension method is marked internal to limit the exposure of this code outside of 
        ///   this library.
        /// </remarks>
        internal static void Track(this ActivityContext context, TrackingRecord record)
        {
            dynamic ctx = new ReflectionObject(context);

            dynamic executor = new ReflectionObject(ctx.executor);

            if (executor.ShouldTrack)
            {
                executor.AddTrackingRecord(record);
            }
        }

        #endregion
    }
}