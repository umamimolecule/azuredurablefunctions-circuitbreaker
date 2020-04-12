using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Polly.Contrib.AzureFunctions.CircuitBreaker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CircuitBreaker
{
    public class OrchestrationClient
    {
        public const string FunctionName = "OrchestrationClient";

        private readonly IDurableCircuitBreakerClient circuitBreakerClient;

        public OrchestrationClient(IDurableCircuitBreakerClient circuitBreakerClient)
        {
            this.circuitBreakerClient = circuitBreakerClient;
        }

        [FunctionName(FunctionName)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableClient client,
            ILogger log)
        {
            var circuitBreakerId = "abc123";
            if (!await this.circuitBreakerClient.IsExecutionPermitted(circuitBreakerId, log, client))
            {
                log?.LogError($"{FunctionName}: Service unavailable.");
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var orchestratorInput = new Orchestrator.Input()
            {
                CircuitBreakerId = circuitBreakerId,
            };

            var instanceId = await client.StartNewAsync(Orchestrator.FunctionName, orchestratorInput);
            var instanceManagementPayload = client.CreateHttpManagementPayload(instanceId);
            return new ObjectResult(instanceManagementPayload) { StatusCode = 202 };
        }
    }
}
