using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Quantum;
using Microsoft.Azure.Quantum.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Function.Csharp
{
    public static class GetRandomNumberJob
    {
        [FunctionName("GetRandomNumberJob")]
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

            var jobStorageHelper = new LinkedStorageJobHelper(workspace);

            string jobId = req.Query["jobId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            jobId = jobId ?? data?.jobId;

            // Get the job details from the workspace.
            var job = workspace.GetJob(jobId);
            var status = job.Details.Status;

            // If the job is in 'succeeded' state, download and return the results
            if (job.Succeeded)
            {
                string result;
                using (var stream = new MemoryStream())
                {
                    jobStorageHelper.DownloadJobOutputAsync(jobId, stream).Wait();
                    stream.Seek(0, SeekOrigin.Begin);
                    result = new StreamReader(stream).ReadToEnd();
                }

                return new OkObjectResult($"Job {jobId} result: {result}");
            }
            // otherwise it just prints the job status.
            return new OkObjectResult($"Job {jobId} status: {status}");
        }
    }
}