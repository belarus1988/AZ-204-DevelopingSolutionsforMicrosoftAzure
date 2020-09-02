using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

namespace ImageResizer
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            byte[] result;

            using(MemoryStream ms = new MemoryStream())
            using (var image = Image.Load(req.Body, out IImageFormat format))
            {
                image.Mutate(x => x.Resize(256, 256));
                image.Save(ms, format);
                
                ms.Position = 0;
                result = ms.ToArray();
            }

            return new FileContentResult(result, "image/jpeg");
        }
    }
}
