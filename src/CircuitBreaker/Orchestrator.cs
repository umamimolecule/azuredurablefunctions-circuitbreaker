using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public class Orchestrator
    {
        public const string FunctionName = "Orchestrator";

        [FunctionName(FunctionName)]
        public async Task Run([OrchestrationTrigger]IDurableOrchestrationContext context)
        {
            SubOrchestrator.Input subOrchestratorInput = new SubOrchestrator.Input()
            {
                ActivityFunctionName = ThrottlingActivity.FunctionName,
                ActivityFunctionInput = null,
                MaximumNumberOfRetries = 3,
            };

            await context.CallSubOrchestratorAsync(SubOrchestrator.FunctionName, subOrchestratorInput);
        }
    }
}
