namespace Honamic.UrlImageResizer
{
    public class UrlImageResizerOptions
    {
        public UrlImageResizerOptions()
        {
            IgnoreFileNotFound = true;
            CachePath = "_resize_cache";
            Default = new Default();
            MaxWidth = 1920;
            MaxHeight = 1080;
        }

        public bool Disable { get; set; }

        public bool IgnoreFileNotFound { get; set; }

        public bool IgnoreResizeFailed { get; set; }

        public bool IgnoreLimitation { get; set; }

        internal string CachePath { get; set; }

        public int MaxWidth { get; set; }

        public int MaxHeight { get; set; }

        public Default Default { get; set; }
    }

    public class Default
    {
        public Default()
        {
            Format = null;
            FitMode = FitMode.Stretch;
            Quality = 90;
            Anchor = AnchorLocation.Center;
        }

        public FitMode FitMode { get; set; }

        public OutputFormat? Format { get; set; }

        public AnchorLocation Anchor { get; set; }

        public int Quality { get; set; }
    }
}
