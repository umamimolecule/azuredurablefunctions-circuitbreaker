using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public static class ThrottlingActivity
    {
        public const string FunctionName = "ThrottlingActivity";

        private static DateTime dateLastCalledUtc = DateTime.MinValue;

        [FunctionName(FunctionName)]
        public static async Task Run(
            [ActivityTrigger]IDurableActivityContext context)
        {
            await Task.CompletedTask;

            // Simulate throttling if this activity is called more often than once every 10 seconds
            var durationSinceLastCall = DateTime.UtcNow.Subtract(dateLastCalledUtc);
            dateLastCalledUtc = DateTime.UtcNow;
            if (durationSinceLastCall < TimeSpan.FromSeconds(10))
            {
                throw new ThrottledException(DateTime.UtcNow.AddSeconds(15));
            }
        }
    }
}
