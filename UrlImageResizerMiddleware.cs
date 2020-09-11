using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;

namespace Honamic.UrlImageResizer
{
    public class UrlImageResizerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUrlImageResizerProvider _provider;
        private readonly IOptions<UrlImageResizerOptions> urlImageResizerOptions;
        private readonly StaticFileOptions _options;
        private readonly PathString _matchUrl;
        private readonly ILogger _logger;
        private readonly IContentTypeProvider _contentTypeProvider;

        /// <summary>
        /// Initialize the Url Image Resizer middleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="provider"></param>
        public UrlImageResizerMiddleware(RequestDelegate next, IUrlImageResizerProvider provider, IWebHostEnvironment hostingEnv, IOptions<StaticFileOptions> options, IOptions<UrlImageResizerOptions> urlImageResizerOptions, ILoggerFactory loggerFactory)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (hostingEnv == null)
            {
                throw new ArgumentNullException("hostingEnv");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }
            this._next = next;
            this._options = options.Value;
            this._contentTypeProvider = options.Value.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            this._matchUrl = this._options.RequestPath;
            this._logger = loggerFactory.CreateLogger<UrlImageResizerMiddleware>();

            _next = next;
            _provider = provider;
            this.urlImageResizerOptions = urlImageResizerOptions;
        }

        /// <summary>
        /// Invoke the middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (urlImageResizerOptions.Value.Disable)
            {
                _logger.UrlResizerDisabled();
            }
            else if (!ValidateNoEndpoint(context))
            {
                _logger.EndpointMatched();
            }
            else if (!ValidateMethod(context))
            {
                _logger.RequestMethodNotSupported(context.Request.Method);
            }
            else if (!ValidatePath(context, this._matchUrl, out var path))
            {
                _logger.PathMismatch(path);
            }
            else if (!ValidateImageType(path, out var contentType))
            {
                _logger.FileTypeNotSupported(path);
            }
            else
            {
                await ToResizedUrl(context, contentType);
            }

            await _next(context);
        }

        private Task ToResizedUrl(HttpContext context, string contentType)
        {
            var resizeRequested = context.CreateFormHttpContext();

            resizeRequested.ContentType = contentType;

            if (resizeRequested.IsResizeRequsted())
            {
                context.Request.Path = _provider.GetResizedUrl(resizeRequested);
            }

            return Task.CompletedTask;
        }

        private static bool ValidateNoEndpoint(HttpContext context)
        {
            return (context.GetEndpoint() == null);
        }

        private static bool ValidateMethod(HttpContext context)
        {
            string method = context.Request.Method;

            bool flag = false;

            if (HttpMethods.IsGet(method))
            {
                flag = true;
            }
            else if (HttpMethods.IsHead(method))
            {
                flag = true;
            }

            return flag;
        }

        internal static bool ValidatePath(HttpContext context, PathString matchUrl, out PathString subPath)
        {
            return Helpers.TryMatchPath(context, matchUrl, false, out subPath);
        }

        internal bool ValidateImageType(PathString path, out string contentType)
        {
            if (!_contentTypeProvider.TryGetContentType(path.Value, out contentType))
            {
                return false;
            }

            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            return contentType.Split('/')[0] == "image";
        }

    }
}