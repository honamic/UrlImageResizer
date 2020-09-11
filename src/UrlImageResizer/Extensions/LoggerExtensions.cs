using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honamic.UrlImageResizer
{
    internal static class LoggerExtensions
    {
        private static Action<ILogger, string, Exception> _methodNotSupported = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "MethodNotSupported"), "{Method} requests are not supported");
        private static Action<ILogger, string, string, Exception> _imageFileResized = LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(2, "FileServed"), "Sending file. Request path: '{VirtualPath}'. Physical path: '{PhysicalPath}'");
        private static Action<ILogger, string, Exception> _pathMismatch = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(3, "PathMismatch"), "The request path {Path} does not match the path filter");
        private static Action<ILogger, string, Exception> _fileTypeNotSupported = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(4, "FileTypeNotSupported"), "The request path {Path} does not match a supported file type");
        private static Action<ILogger, string, Exception> _fileNotFound = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(5, "FileNotFound"), "The request path {Path} does not match an existing file");
        private static Action<ILogger, Exception> _endpointMatched = LoggerMessage.Define(LogLevel.Debug, new EventId(15, "EndpointMatched"), "Static files was skipped as the request already matched an endpoint.");
        private static Action<ILogger, Exception> _urlResizerDisabled = LoggerMessage.Define(LogLevel.Debug, new EventId(15, "EndpointMatched"), "Url image resizer was disabled");

        public static void UrlResizerDisabled(this ILogger logger)
        {
            _urlResizerDisabled(logger, null);
        }

        public static void EndpointMatched(this ILogger logger)
        {
            _endpointMatched(logger, null);
        }

        public static void FileNotFound(this ILogger logger, string path)
        {
            _fileNotFound(logger, path, null);
        }

        public static void ImageFileResized(this ILogger logger, string virtualPath, string physicalPath)
        {
            if (string.IsNullOrEmpty(physicalPath))
            {
                physicalPath = "N/A";
            }
            _imageFileResized(logger, virtualPath, physicalPath, null);
        }

        public static void FileTypeNotSupported(this ILogger logger, string path)
        {
            _fileTypeNotSupported(logger, path, null);
        }

        public static void PathMismatch(this ILogger logger, string path)
        {
            _pathMismatch(logger, path, null);
        }

        public static void RequestMethodNotSupported(this ILogger logger, string method)
        {
            _methodNotSupported(logger, method, null);
        }
    }


}
