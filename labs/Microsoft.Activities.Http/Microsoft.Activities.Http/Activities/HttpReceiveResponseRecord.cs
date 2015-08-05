namespace Microsoft.Activities.Http.Activities
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Tracking;

    internal class HttpReceiveResponseRecord : CustomTrackingRecord, ICustomTrackingTrace
    {
        #region Constants and Fields

        private const string ResultId = "ResultId";

        #endregion

        #region Constructors and Destructors

        public HttpReceiveResponseRecord(object result)
            : base(typeof(HttpReceiveResponseRecord).Name)
        {
            this.Result = result;
        }

        protected HttpReceiveResponseRecord(HttpReceiveResponseRecord record)
            : base(record)
        {
        }

        #endregion

        #region Properties

        protected object Result
        {
            get
            {
                return this.Data[ResultId];
            }
            set
            {
                this.Data[ResultId] = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICustomTrackingTrace

        public void Trace(TrackingOptions options)
        {
            System.Diagnostics.Trace.WriteLine(
                string.Format(
                    "{0}: HttpResponse [{1}] \"{2}\" result {3}",
                    this.RecordNumber,
                    this.Activity != null ? this.Activity.Id : "null",
                    this.Activity != null ? this.Activity.Name : "null",
                    this.Result ?? "null"));
            TrackingHelper.TraceInstance(options, this.InstanceId, this.Annotations, null, null, this.Data, this.EventTime);
        }

        #endregion

        #endregion

        #region Methods

        protected override TrackingRecord Clone()
        {
            return new HttpReceiveResponseRecord(this);
        }

        #endregion
    }
}