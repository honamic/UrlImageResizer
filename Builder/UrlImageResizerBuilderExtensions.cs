using Honamic.UrlImageResizer;
using System;


namespace Microsoft.AspNetCore.Builder
{
    public static class UrlImageResizerBuilderExtensions
    {
        /// <summary>
        /// Adds middleware for dynamically url image resize.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        public static IApplicationBuilder UseUrlImageResizer(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<UrlImageResizerMiddleware>();
        }
    }
}
