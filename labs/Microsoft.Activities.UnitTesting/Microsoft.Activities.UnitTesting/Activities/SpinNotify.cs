namespace Microsoft.Activities.UnitTesting.Activities
{
    using System;

    /// <summary>
    /// The spin notify.
    /// </summary>
    public class SpinNotify
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public Action<int, int> LoopComplete { get; set; }

        #endregion
    }
}