using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public static class ThrottlingActivity
    {
        public const string FunctionName = "ThrottlingActivity";

        [FunctionName(FunctionName)]
        public static async Task Run(
            [ActivityTrigger]IDurableActivityContext context)
        {
            await Task.CompletedTask;
            throw new ThrottledException(DateTime.UtcNow.AddSeconds(5));
        }
    }
}
