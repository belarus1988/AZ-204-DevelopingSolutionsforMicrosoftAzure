using System.IO;
using Microsoft.Azure.WebJobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ImageProcessor
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static FileInfo Run([ActivityTrigger] IDurableActivityContext context)
        {
            var fileInfo = context.GetInput<FileInfo>();
            byte[] result;

            using (var ms = new MemoryStream())
            {
                using (var image = Image.Load(fileInfo.Content, out IImageFormat format))
                {
                    image.Mutate(x => x.Resize(256, 256));
                    image.Save(ms, format);
                }
                result = ms.ToArray();
            }

            return new FileInfo
            {
                Name = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}_small{Path.GetExtension(fileInfo.Name)}",
                Content = result
            };
        }
    }
}
