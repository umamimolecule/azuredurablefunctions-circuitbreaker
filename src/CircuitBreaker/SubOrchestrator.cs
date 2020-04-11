using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace CircuitBreaker
{
    public class SubOrchestrator
    {
        public const string FunctionName = "SubOrchestrator";

        public class Input
        {
            public string ActivityFunctionName { get; set; }

            public object ActivityFunctionInput { get; set; }

            public int RetryCount { get; set; }

            public int? MaximumNumberOfRetries { get; set; }
        }

        [FunctionName(FunctionName)]
        public async Task<object> Run(
            [OrchestrationTrigger]IDurableOrchestrationContext context,
            ILogger log)
        {
            var input = context.GetInput<Input>();

            try
            {
                // Execute activity
                log.LogInformation($"Executing activity {input.ActivityFunctionName} (retry #{input.RetryCount})...");
                var output = await context.CallActivityAsync<object>(input.ActivityFunctionName, input.ActivityFunctionInput);
                log.LogInformation($"Activity {input.ActivityFunctionName} successful!");

                // TODO: Notify circuit-breaker of success

                return output;
            }
            catch (FunctionFailedException ffe)
            {
                if (ffe.InnerException is ThrottledException te)
                {
                    log.LogWarning($"Activity {input.ActivityFunctionName} failed, will retry after {te.RetryAfterUtc:o}");

                    // TODO: Notify circuit-breaker of failure

                    // Go to sleep and retry later
                    await context.CreateTimer(te.RetryAfterUtc, CancellationToken.None);
                    input.RetryCount++;

                    // Check if we're are the maximum number of retries
                    // TODO: You could also set a maximum time instead of max number
                    if (input.MaximumNumberOfRetries.HasValue && input.RetryCount > input.MaximumNumberOfRetries.Value)
                    {
                        throw new MaximumRetriesExceededException(input.MaximumNumberOfRetries.Value, te);
                    }

                    context.ContinueAsNew(input);
                    return null;
                }

                throw;
            }
        }
    }
}
