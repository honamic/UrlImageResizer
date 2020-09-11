using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Honamic.UrlImageResizer
{
    public static class ResizeRequestExtensions
    {
        public static ResizeRequest CreateFormHttpContext(this HttpContext httpContext)
        {
            var resizeRequest = new ResizeRequest(httpContext.Request.Path);

            var queryCollection = httpContext.Request.Query;

            resizeRequest.Width = queryCollection.ExtractValue<int?>("width", "w");

            resizeRequest.Height = queryCollection.ExtractValue<int?>("height", "h");

            resizeRequest.Format = queryCollection.ExtractValue<OutputFormat?>("format", "f");

            resizeRequest.Quality = queryCollection.ExtractValue<int?>("quality", "q");

            resizeRequest.Anchor = queryCollection.ExtractValue<AnchorLocation?>("anchor", "a");

            resizeRequest.Mode = queryCollection.ExtractValue<FitMode?>("mode", "m");

            return resizeRequest;
        }

        private static T ExtractValue<T>(this IQueryCollection queryCollection, params string[] names)
        {
            T result = default;

            foreach (var name in names)
            {
                result = queryCollection.ExtractValue<T>(name);

                if (result != null)
                {
                    return result;
                }
            }

            return result;
        }

        private static T ExtractValue<T>(this IQueryCollection queryCollection, string name)
        {
            var value = queryCollection[name].FirstOrDefault();

            return TConverter.ChangeType<T>(value);
        }
    }
}