// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Quantum;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Quantum.Providers.IonQ.Targets;
using QuantumRNG;
using System;
using System.Threading.Tasks;

namespace Function.Csharp
{
    public static class CreateRandomNumberJob
    {
        [FunctionName("CreateRandomNumberJob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // define the workspace that manages the target machines.
            var workspace = new Workspace
            (
                subscriptionId: Environment.GetEnvironmentVariable("subscriptionId"),
                resourceGroupName: Environment.GetEnvironmentVariable("resourceGroupName"),
                workspaceName: Environment.GetEnvironmentVariable("workspaceName"),
                location: Environment.GetEnvironmentVariable("location")
            );

            // Select the machine that will execute the Q# operation.
            // Target must be enabled in the workspace referenced above.
            var targetId = "ionq.simulator";

            var quantumMachine = new IonQQuantumMachine(
                target: targetId,
                workspace: workspace);

            // Submit the random number job to the target machine.
            var randomNumberJob = quantumMachine.SubmitAsync(GenerateRandomBits.Info, 4);

            return new OkObjectResult($"RandomBits job id: {randomNumberJob.Result.Id}");
        }
    }
}
