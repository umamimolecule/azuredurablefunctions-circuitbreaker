using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public static class DurableOrchestrationContextExtensions
    {
        public static Task ExecuteActivityWithCircuitBreaker(this IDurableOrchestrationContext context, string functionName, CircuitBreakerOptions circuitBreakerOptions, object input)
        {
            return ExecuteActivityWithCircuitBreaker<object>(context, functionName, circuitBreakerOptions, input);
        }

        public static Task<T> ExecuteActivityWithCircuitBreaker<T>(this IDurableOrchestrationContext context, string functionName, CircuitBreakerOptions circuitBreakerOptions, object input)
        {
            SubOrchestrator.Input subOrchestratorInput = new SubOrchestrator.Input()
            {
                ActivityFunctionName = functionName,
                ActivityFunctionInput = input,
                CircuitBreakerOptions = circuitBreakerOptions,
            };

            return context.CallSubOrchestratorAsync<T>(SubOrchestrator.FunctionName, subOrchestratorInput);
        }
    }
}
