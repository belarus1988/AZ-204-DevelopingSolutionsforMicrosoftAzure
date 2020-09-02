using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace ImageProcessor
{
    public static class FileCompressor
    {
        [FunctionName("FileCompressor")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            byte[] result;
            var fileName = Utils.GetFileName(req);

            using (var ms = new MemoryStream())
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {

                        var zipItem = zip.CreateEntry(fileName);
                        using (var entryStream = zipItem.Open())
                        {
                            req.Body.CopyTo(entryStream);
                        }
                    }
                }

                result = ms.ToArray();
            }

            return new FileContentResult(result, "application/zip")
            {
                FileDownloadName = string.IsNullOrEmpty(fileName) 
                    ? "download.zip" 
                    : $"{Path.GetFileNameWithoutExtension(fileName)}.zip"
            };
        }
    }
}
