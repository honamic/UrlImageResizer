using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Honamic.UrlImageResizer
{
    internal class UrlImageResizerProvider : IUrlImageResizerProvider
    {
        private readonly string _cachePath;
        private readonly IOptions<UrlImageResizerOptions> options;
        private readonly StaticFileOptions _staticFileOptions;
        private readonly ILogger _logger;
        private readonly IImageProcessor _imageResizer;
        private readonly IFileProvider _fileProvider;
        private readonly ConcurrentDictionary<string, object> pathLocks;

        public UrlImageResizerProvider(IOptions<UrlImageResizerOptions> options,
            IOptions<StaticFileOptions> staticFileOptions,
            ILogger<UrlImageResizerProvider> logger,
            IImageProcessor imageResizer,
            IWebHostEnvironment hostingEnv)
        {
            this.options = options;
            _staticFileOptions = staticFileOptions.Value;
            _logger = logger;
            _fileProvider = _staticFileOptions.FileProvider ?? Helpers.ResolveFileProvider(hostingEnv);
            _imageResizer = imageResizer;
            pathLocks = new ConcurrentDictionary<string, object>();
            _cachePath = Path.Combine(hostingEnv.WebRootPath, options.Value.CachePath);
        }

        public string GetResizedUrl(ResizeRequest resizeRequested)
        {
            var returnPath = resizeRequested.Path;

            if (Limited(resizeRequested))
            {
                if (options.Value.IgnoreLimitation)
                {
                    return returnPath;
                }
                else
                {
                    throw new UrlImageResizerException(resizeRequested, "bad request");
                }
            }

            var fileInfo = _fileProvider.GetFileInfo(resizeRequested.Path);

            if (!fileInfo.Exists)
            {
                _logger.FileNotFound(fileInfo.PhysicalPath);

                if (options.Value.IgnoreFileNotFound)
                {
                    return returnPath;
                }
                else
                {
                    throw new UrlImageResizerException(resizeRequested, "File not found.");
                }
            }

            var imageProcessing = ToImageProcessing(resizeRequested, fileInfo.PhysicalPath);

            var resizeFileInfo = _fileProvider.GetFileInfo(imageProcessing.ResizeUrl);

            if (resizeFileInfo.Exists)
            {
                returnPath = imageProcessing.ResizeUrl;
            }
            else
            {
                var pathLock = pathLocks.GetOrAdd(resizeRequested.Path.ToLower(), new object());

                lock (pathLock)
                {
                    try
                    {
                        if (!resizeFileInfo.Exists)
                        {
                            EnsureDirectoryExist(imageProcessing);

                            _imageResizer.Processing(imageProcessing);
                        }

                        returnPath = imageProcessing.ResizeUrl;

                        resizeFileInfo = _fileProvider.GetFileInfo(imageProcessing.ResizeUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("image url resize failed.", ex);

                        if (!options.Value.IgnoreResizeFailed)
                        {
                            throw new UrlImageResizerException(resizeRequested, "image url resize failed.", ex);
                        }
                    }
                }
            }

            if (returnPath == imageProcessing.ResizeUrl && resizeFileInfo.Exists)
            {
                File.SetLastAccessTimeUtc(imageProcessing.ResizePath, DateTime.UtcNow);
            }

            return returnPath;
        }

        private bool Limited(ResizeRequest resizeRequested)
        {
            if (resizeRequested.Width > options.Value.MaxWidth)
            {
                return true;
            }

            if (resizeRequested.Height > options.Value.MaxHeight)
            {
                return true;
            }

            return false;
        }

        private static void EnsureDirectoryExist(ImageProcessing imageProcessing)
        {
            if (!Directory.Exists(imageProcessing.ResizeDirectory))
            {
                Directory.CreateDirectory(imageProcessing.ResizeDirectory);
            }
        }

        public ImageProcessing ToImageProcessing(ResizeRequest resizeRequest, string filePath)
        {
            var result = new ImageProcessing()
            {
                Url = resizeRequest.Path,
                ContentType = resizeRequest.ContentType,
                Format = resizeRequest.Format ?? options.Value.Default.Format,
                Mode = resizeRequest.Mode ?? options.Value.Default.FitMode,
                Quality = resizeRequest.Quality ?? options.Value.Default.Quality,
                Anchor = resizeRequest.Anchor ?? options.Value.Default.Anchor,
                Height = resizeRequest.Height,
                Width = resizeRequest.Width,
                Path = filePath,
            };

            var resizeFileName = result.GetResizeName();

            var requestedDirectory = Path.GetDirectoryName(result.Url);

            result.ResizeUrl = ToUrlPath(
                Path.Combine(options.Value.CachePath, RemoveRootSlash(requestedDirectory), resizeFileName)
                );

            result.ResizePath = Path.Combine(_cachePath, RemoveRootSlash(requestedDirectory), resizeFileName);

            return result;
        }

        private static string RemoveRootSlash(string orginalPath)
        {
            return orginalPath?.Substring(1) ?? throw new ArgumentNullException(nameof(orginalPath));
        }

        private static string ToUrlPath(string path)
        {
            if (path[0] != '\\')
                path = "\\" + path;

            return path.Replace("\\", "/");
        }
    }
}