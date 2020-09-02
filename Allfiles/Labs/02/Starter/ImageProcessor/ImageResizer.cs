using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            byte[] result;

            using (MemoryStream ms = new MemoryStream())
            {
                using (var image = Image.Load(req.Body, out IImageFormat format))
                {
                    image.Mutate(x => x.Resize(256, 256));
                    image.Save(ms, format);
                }
                result = ms.ToArray();
            }

            var fileName = Utils.GetFileName(req) ?? "image";
            return new FileContentResult(result, "image/jpeg")
            {
                FileDownloadName = $"{Path.GetFileNameWithoutExtension(fileName)}_small.jpeg"
            };
        }
    }
}
