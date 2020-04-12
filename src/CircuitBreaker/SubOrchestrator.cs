using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Polly.Contrib.AzureFunctions.CircuitBreaker;

namespace CircuitBreaker
{
    public class SubOrchestrator
    {
        public const string FunctionName = "SubOrchestrator";

        private readonly IDurableCircuitBreakerClient circuitBreakerClient;

        public class Input
        {
            public string ActivityFunctionName { get; set; }

            public object ActivityFunctionInput { get; set; }

            public CircuitBreakerOptions CircuitBreakerOptions { get; set; }

            public int ExecutionCount { get; set; }
        }

        public SubOrchestrator(IDurableCircuitBreakerClient circuitBreakerClient)
        {
            this.circuitBreakerClient = circuitBreakerClient;
        }

        [FunctionName(FunctionName)]
        public async Task<object> Run(
            [OrchestrationTrigger]IDurableOrchestrationContext context,
            ILogger log)
        {
            var input = context.GetInput<Input>();

            try
            {
                var output = await context.CallActivityAsync<object>(input.ActivityFunctionName, input.ActivityFunctionInput);
                await this.circuitBreakerClient.RecordSuccess(input.CircuitBreakerOptions.CircuitBreakerId, log, context);
                return output;
            }
            catch (Exception exception)
            {
                await this.circuitBreakerClient.RecordFailure(input.CircuitBreakerOptions.CircuitBreakerId, log, context);
                var handled = await HandleException(exception, context);
                if (handled)
                {
                    context.ContinueAsNew(input);
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task<bool> HandleException(Exception exception, IDurableOrchestrationContext context)
        {
            if (exception is FunctionFailedException ffe)
            {
                exception = ffe.InnerException;
            }

            if (exception is ThrottledException te)
            {
                // Sleep until next retry
                // TODO: We should persist the latest RetryAfterUtc for the circuit breaker ID, because we might
                //       have multiple calls in parallel, each one constantly getting throttled.  Then use the
                //       latest value between the persisted one and the one in the exception.
                await context.CreateTimer(te.RetryAfterUtc, CancellationToken.None);
                return true;
            }

            // TODO: Add other exceptions here as needed, for instance to handle transient network errors

            return false;
        }
    }
}
