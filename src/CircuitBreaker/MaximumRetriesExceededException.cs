using System;
using System.Runtime.Serialization;

namespace CircuitBreaker
{
    [Serializable]
    public class MaximumRetriesExceededException : Exception
    {
        public MaximumRetriesExceededException(int maximumNumberOfRetries, Exception innerException = null)
            : base($"Maximum number of {maximumNumberOfRetries} retries has been reached", innerException)
        {
            this.MaximumNumberOfRetries = maximumNumberOfRetries;
        }

        public MaximumRetriesExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.MaximumNumberOfRetries = info.GetInt32(nameof(this.MaximumNumberOfRetries));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.MaximumNumberOfRetries), this.MaximumNumberOfRetries);
        }

        public int MaximumNumberOfRetries { get; private set; }
    }
}
