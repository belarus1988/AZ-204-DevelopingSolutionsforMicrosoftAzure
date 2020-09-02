using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace ImageProcessor
{
    public static class Utils
    {
        public static string GetFileName(HttpRequest req)
        {
            if(!req.Headers.TryGetValue("Content-Disposition", out var header))
                return null;

            var contentDisposition = new ContentDisposition(header);
            return contentDisposition.FileName;
        }
    }
}
