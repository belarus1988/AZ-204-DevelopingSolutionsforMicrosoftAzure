using System.IO;
using Microsoft.Azure.WebJobs;
using System.IO.Compression;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace ImageProcessor
{
    public static class FileCompressor
    {
        [FunctionName("FileCompressor")]
        public static FileInfo Run([ActivityTrigger] IDurableActivityContext context)
        {
            var fileInfo = context.GetInput<FileInfo>();
            byte[] result;

            using (var input = new MemoryStream(fileInfo.Content))
            {
                using var output = new MemoryStream();
                using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
                {
                    var zipItem = zip.CreateEntry(fileInfo.Name);
                    using (var entryStream = zipItem.Open())
                    {
                        input.CopyTo(entryStream);
                    }
                }

                result = output.ToArray();
            }

            return new FileInfo
            {
                Name = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}.zip",
                Content = result
            };
        }
    }
}
