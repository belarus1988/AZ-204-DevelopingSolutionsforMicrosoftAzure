using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace ImageProcessor
{
    public static class ResizeAndCompress
    {
        [FunctionName("ResizeAndCompress")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Blob("small-images", System.IO.FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer outputContainer)
        {
            var fileInfo = context.GetInput<FileInfo>();
            var resized = await context.CallActivityAsync<FileInfo>("ImageResizer", fileInfo);
            var compressed = await context.CallActivityAsync<FileInfo>("FileCompressor", resized);

            var blockBlob = outputContainer.GetBlockBlobReference(compressed.Name);
            await blockBlob.UploadFromByteArrayAsync(compressed.Content, 0, compressed.Content.Length);
        }

        [FunctionName("ResizeAndCompress_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            var fileInfo = new FileInfo
            {
                Name = Utils.GetFileName(req.Content.Headers),
                Content = await req.Content.ReadAsByteArrayAsync()
            };
            string instanceId = await starter.StartNewAsync("ResizeAndCompress", "", fileInfo);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}