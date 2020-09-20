using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ImageProcessor
{
    public static class ResizeAndCompress
    {
        [FunctionName("ResizeAndCompress")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Blob("small-images", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer outputContainer)
        {
            var fileInfo = context.GetInput<FileInfo>();
            var resized = await context.CallActivityAsync<FileInfo>("ImageResizer", fileInfo);
            var compressed = await context.CallActivityAsync<FileInfo>("FileCompressor", resized);

            var blockBlob = outputContainer.GetBlockBlobReference(compressed.Name);
            await blockBlob.UploadFromByteArrayAsync(compressed.Content, 0, compressed.Content.Length);
        }

        [FunctionName("ResizeAndCompress_HttpStart")]
        public static async Task HttpStart(
            [BlobTrigger("images/{fileName}", Connection = "AzureWebJobsStorage")] byte[] image, 
            string fileName,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            var fileInfo = new FileInfo
            {
                Name = fileName,
                Content = image
            };

            await starter.StartNewAsync("ResizeAndCompress", "", fileInfo);
        }
    }
}