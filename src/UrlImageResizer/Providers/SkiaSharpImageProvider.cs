using SkiaSharp;
using System;
using System.Drawing;
using System.IO;

namespace Honamic.UrlImageResizer
{
    public class SkiaSharpImageProvider : IImageProcessor
    {
        public void Processing(ImageProcessing imageProcessing)
        {
            using (var bitmap = SKBitmap.Decode(imageProcessing.Path))
            {
                var newSize = new Size(bitmap.Width, bitmap.Height);

                if (imageProcessing.Width.HasValue)
                {
                    newSize.Width = imageProcessing.Width.Value;
                }

                if (imageProcessing.Height.HasValue)
                {
                    newSize.Height = imageProcessing.Height.Value;
                }

                if (!imageProcessing.Format.HasValue)
                {
                    imageProcessing.Format = DetectFormat(imageProcessing.ContentType);
                }

                using (var surface = SKSurface.Create(new SKImageInfo(newSize.Width, newSize.Height, bitmap.ColorType, bitmap.AlphaType)))
                using (var paint = new SKPaint() { FilterQuality = SKFilterQuality.High })
                {
                    var canvas = surface.Canvas;
                    SetScale(canvas, imageProcessing.Mode, newSize, bitmap);
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                    canvas.Flush();

                    using (var output = File.OpenWrite(imageProcessing.ResizePath))
                    {
                        switch (imageProcessing.Format)
                        {
                            case OutputFormat.Jpeg:
                                surface.Snapshot()
                                    .Encode(SKEncodedImageFormat.Jpeg, imageProcessing.Quality)
                                    .SaveTo(output);
                                break;
                            case OutputFormat.Png:
                                surface.Snapshot()
                                    .Encode(SKEncodedImageFormat.Png, imageProcessing.Quality)
                                    .SaveTo(output);
                                break;
                            case OutputFormat.Gif:
                                surface.Snapshot()
                                    .Encode(SKEncodedImageFormat.Gif, imageProcessing.Quality)
                                    .SaveTo(output);
                                break;
                            case OutputFormat.WebP:
                                surface.Snapshot()
                                    .Encode(SKEncodedImageFormat.Webp, imageProcessing.Quality)
                                    .SaveTo(output);
                                break;
                            case OutputFormat.Bmp:
                                surface.Snapshot()
                                  .Encode(SKEncodedImageFormat.Bmp, imageProcessing.Quality)
                                  .SaveTo(output);
                                break;
                            default:
                                throw new InvalidOperationException($"Format not supported:{imageProcessing.Format}");
                        }

                    }
                }
            }
        }

        private OutputFormat DetectFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                    return OutputFormat.Jpeg;
                case "image/png":
                    return OutputFormat.Png;
                case "image/gif":
                    return OutputFormat.Gif;
                case "image/bmp":
                    return OutputFormat.Bmp;
                case "image/webp":
                    return OutputFormat.WebP;
                default:
                    break;
            }

            return OutputFormat.Jpeg;
        }

        private void SetScale(SKCanvas canvas, FitMode mode, Size newSize, SKBitmap bitmap)
        {
            switch (mode)
            {
                case FitMode.Stretch:
                    var sx = (float)newSize.Width / bitmap.Width;
                    var sy = (float)newSize.Height / bitmap.Height;
                    canvas.Scale(sx, sy);
                    break;
                default:
                    throw new InvalidOperationException($"FitMode not supported:{mode}");
            }
        }
    }
}
