using System;
using Honamic.UrlImageResizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    public static class UrlImageResizerServiceExtensions
    {
        public static IServiceCollection AddUrlImageResizer(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IUrlImageResizerProvider, UrlImageResizerProvider>();
            
             services.TryAddSingleton<IImageProcessor, SkiaSharpImageProvider>();
            
            return services;
        }

        public static IServiceCollection AddUrlImageResizer(this IServiceCollection services, Action<UrlImageResizerOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);

            services.TryAddSingleton<IUrlImageResizerProvider, UrlImageResizerProvider>();

            services.TryAddSingleton<IImageProcessor, SkiaSharpImageProvider>();

            return services;
        }
    }
}
