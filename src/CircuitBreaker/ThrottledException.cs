using System;
using System.Runtime.Serialization;

namespace CircuitBreaker
{
    [Serializable]
    public class ThrottledException : Exception
    {
        public ThrottledException(DateTime retryAfter)
            : base($"Too many requests.  Retry after {retryAfter:o}")
        {
            this.RetryAfterUtc = retryAfter;
        }

        public ThrottledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RetryAfterUtc = info.GetDateTime(nameof(RetryAfterUtc));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(RetryAfterUtc), RetryAfterUtc);
        }

        public DateTime RetryAfterUtc { get; private set; }
    }
}
