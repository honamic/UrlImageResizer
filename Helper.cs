using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;

namespace Honamic.UrlImageResizer
{
    internal static class Helpers
    {
        // Methods
        internal static bool IsGetOrHeadMethod(string method)
        {
            return (HttpMethods.IsGet(method) || HttpMethods.IsHead(method));
        }

        internal static bool PathEndsInSlash(PathString path)
        {
            return path.Value.EndsWith("/", (StringComparison)StringComparison.Ordinal);
        }

        internal static IFileProvider ResolveFileProvider(IWebHostEnvironment hostingEnv)
        {
            if (hostingEnv.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }
            return hostingEnv.WebRootFileProvider;
        }

        internal static bool TryMatchPath(HttpContext context, PathString matchUrl, bool forDirectory, out PathString subpath)
        {
            PathString path = context.Request.Path;

            if (forDirectory && !PathEndsInSlash(path))
            {
                path += new PathString("/");
            }

            return path.StartsWithSegments(matchUrl, out subpath);
        }
    }
}
