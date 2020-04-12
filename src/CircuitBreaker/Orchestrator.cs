using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public class Orchestrator
    {
        public const string FunctionName = "Orchestrator";

        public class Input
        {
            public string CircuitBreakerId { get; set; }
        }

        [FunctionName(FunctionName)]
        public async Task Run([OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            var input = context.GetInput<Input>();

            var circuitBreakerOptions = new CircuitBreakerOptions()
            {
                CircuitBreakerId = input.CircuitBreakerId,
            };

            await context.ExecuteActivityWithCircuitBreaker(ThrottlingActivity.FunctionName, circuitBreakerOptions, null);
        }
    }
}
