using System;

namespace Honamic.UrlImageResizer
{
    public class UrlImageResizerException : Exception
    {
        public ResizeRequest ResizeRequest { get; set; }

        public UrlImageResizerException(ResizeRequest resizeRequest, string message, Exception ex = null) : base(message, ex)
        {
            ResizeRequest = resizeRequest;
        }
    }
}
