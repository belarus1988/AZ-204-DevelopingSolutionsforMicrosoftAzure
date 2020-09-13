using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace ImageProcessor
{
    public static class Utils
    {
        public static string GetFileName(HttpContentHeaders headers)
        {
            if(!headers.Contains("Content-Disposition"))
                return null;

            var headerValues = headers.GetValues("Content-Disposition");
            var header = headerValues.FirstOrDefault();

            var contentDisposition = new ContentDisposition(header);
            return contentDisposition.FileName;
        }
    }
}
