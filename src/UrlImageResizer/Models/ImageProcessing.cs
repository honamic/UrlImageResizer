namespace Honamic.UrlImageResizer
{
    public class ImageProcessing
    {
        public string Url { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }

        public string ResizeUrl { get; set; }

        public string ResizePath { get; set; }

        public int Quality { get; set; }

        public string ResizeDirectory =>  System.IO.Path.GetDirectoryName(ResizePath);

        public FitMode Mode { get; set; }

        public OutputFormat? Format { get; set; }

        public AnchorLocation Anchor { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string GetUniqName()
        {
            /// logo.png-500x600-Stretch.jpeg
            /// logo.png-500w-Stretch.jpeg
            /// logo.png-600h-Stretch.jpeg

            string size;

            if (Width.HasValue && Height.HasValue)
            {
                size = $"{Width.Value}x{Height.Value}";
            }
            else if (Width.HasValue)
            {
                size = $"{Width.Value}w";
            }
            else
            {
                size = $"{Height.Value}h";
            }

            var fileName = System.IO.Path.GetFileName(Path);

            return $"{fileName}-{size}-{Mode}.{Format}".ToLower();
        }

    }
}