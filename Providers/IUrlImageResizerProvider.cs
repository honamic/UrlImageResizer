namespace Honamic.UrlImageResizer
{
    public interface IUrlImageResizerProvider
    {
        string GetResizedUrl(ResizeRequest resizeRequested);
    }
}