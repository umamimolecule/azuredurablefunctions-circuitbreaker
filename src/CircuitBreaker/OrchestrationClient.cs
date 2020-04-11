using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CircuitBreaker
{
    public static class OrchestrationClient
    {
        public const string FunctionName = "OrchestrationClient";

        [FunctionName(FunctionName)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            var instanceId = await client.StartNewAsync(Orchestrator.FunctionName);
            var instanceManagementPayload = client.CreateHttpManagementPayload(instanceId);
            return new ObjectResult(instanceManagementPayload) { StatusCode = 202 };
        }
    }
}
