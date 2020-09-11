using System.Drawing;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Honamic.UrlImageResizer
{
    public class ResizeRequest
    {
        public ResizeRequest(PathString path)
        {
            Path = HttpUtility.UrlDecode(path);
        }

        public string Path { get; }

        public string ContentType { get; set; }

        public int? Quality { get; set; }

        public FitMode? Mode { get; set; }

        public AnchorLocation? Anchor { get; set; }

        public OutputFormat? Format { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public bool IsResizeRequsted()
        {
            return Height.HasValue || Width.HasValue || Format.HasValue;
        }
    }
}