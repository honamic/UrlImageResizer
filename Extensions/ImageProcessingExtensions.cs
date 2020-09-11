namespace Honamic.UrlImageResizer
{
    public static class ImageProcessingExtensions
    {
        public static string GetResizeName(this ImageProcessing model)
        {
            string size;

            if (model.Width.HasValue && model.Height.HasValue)
            {
                size = $"{model.Width.Value}x{model.Height.Value}";
            }
            else if (model.Width.HasValue)
            {
                size = $"{model.Width.Value}w";
            }
            else
            {
                size = $"{model.Height.Value}h";
            }

            var fileName = System.IO.Path.GetFileName(model.Path);

            var format = model.Format?.ToString() ?? System.IO.Path.GetExtension(fileName).Replace(".","");

            return $"{fileName}-{size}-{model.Quality}-{model.Mode}.{format}".ToLower();
        }
    }
}
