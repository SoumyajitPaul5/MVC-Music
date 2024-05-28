using SkiaSharp;

namespace MVC_Music.Utilities
{
    public static class ResizeImage
    {
        // Shrinks an image and encodes it in WebP format
        public static Byte[] shrinkImageWebp(Byte[] originalImage, int max_height = 100, int max_width = 120)
        {
            using SKMemoryStream sourceStream = new SKMemoryStream(originalImage);
            using SKCodec codec = SKCodec.Create(sourceStream);
            sourceStream.Seek(0);

            using SKImage image = SKImage.FromEncodedData(SKData.Create(sourceStream));
            int newHeight = image.Height;
            int newWidth = image.Width;

            // Resize logic for height
            if (max_height > 0 && newHeight > max_height)
            {
                double scale = (double)max_height / newHeight;
                newHeight = max_height;
                newWidth = (int)Math.Floor(newWidth * scale);
            }

            // Resize logic for width
            if (max_width > 0 && newWidth > max_width)
            {
                double scale = (double)max_width / newWidth;
                newWidth = max_width;
                newHeight = (int)Math.Floor(newHeight * scale);
            }

            var info = codec.Info.ColorSpace.IsSrgb ? new SKImageInfo(newWidth, newHeight) : new SKImageInfo(newWidth, newHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul, SKColorSpace.CreateSrgb());
            using SKSurface surface = SKSurface.Create(info);
            using SKPaint paint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High };

            surface.Canvas.Clear(SKColors.White);
            var rect = new SKRect(0, 0, newWidth, newHeight);
            surface.Canvas.DrawImage(image, rect, paint);
            surface.Canvas.Flush();

            using SKImage newImage = surface.Snapshot();
            using SKData newImageData = newImage.Encode(SKEncodedImageFormat.Webp, 100);

            return newImageData.ToArray();
        }

        // ViewModel to store image content and MIME type
        public class ImageVM
        {
            public Byte[] Content { get; set; }
            public string MimeType { get; set; }
        }

        // Shrinks an image and encodes it in the specified format
        public static ImageVM shrinkImage(Byte[] originalImage, int max_height = 100, int max_width = 120, SKEncodedImageFormat selectedFormat = SKEncodedImageFormat.Webp, int quality = 100)
        {
            using SKMemoryStream sourceStream = new SKMemoryStream(originalImage);
            using SKCodec codec = SKCodec.Create(sourceStream);
            sourceStream.Seek(0);

            using SKImage image = SKImage.FromEncodedData(SKData.Create(sourceStream));
            int newHeight = image.Height;
            int newWidth = image.Width;

            // Resize logic for height
            if (max_height > 0 && newHeight > max_height)
            {
                double scale = (double)max_height / newHeight;
                newHeight = max_height;
                newWidth = (int)Math.Floor(newWidth * scale);
            }

            // Resize logic for width
            if (max_width > 0 && newWidth > max_width)
            {
                double scale = (double)max_width / newWidth;
                newWidth = max_width;
                newHeight = (int)Math.Floor(newHeight * scale);
            }

            var info = codec.Info.ColorSpace.IsSrgb ? new SKImageInfo(newWidth, newHeight) : new SKImageInfo(newWidth, newHeight, SKImageInfo.PlatformColorType, SKAlphaType.Premul, SKColorSpace.CreateSrgb());
            using SKSurface surface = SKSurface.Create(info);
            using SKPaint paint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High };

            surface.Canvas.Clear(SKColors.White);
            var rect = new SKRect(0, 0, newWidth, newHeight);
            surface.Canvas.DrawImage(image, rect, paint);
            surface.Canvas.Flush();

            using SKImage newImage = surface.Snapshot();
            using SKData newImageData = newImage.Encode(selectedFormat, quality);

            ImageVM imageVM = new ImageVM
            {
                Content = newImageData.ToArray(),
                MimeType = selectedFormat switch
                {
                    SKEncodedImageFormat.Bmp => "image/bmp",
                    SKEncodedImageFormat.Gif => "image/gif",
                    SKEncodedImageFormat.Ico => "image/vnd.microsoft.icon",
                    SKEncodedImageFormat.Jpeg => "image/jpeg",
                    SKEncodedImageFormat.Png => "image/png",
                    SKEncodedImageFormat.Wbmp => "image/wbmp",
                    SKEncodedImageFormat.Webp => "image/webp",
                    SKEncodedImageFormat.Pkm => "image/octet-stream",
                    SKEncodedImageFormat.Ktx => "image/ktx",
                    SKEncodedImageFormat.Astc => "image/png",
                    SKEncodedImageFormat.Dng => "image/DNG",
                    SKEncodedImageFormat.Heif => "image/heif",
                    _ => "image/jpeg",
                }
            };

            return imageVM;
        }
    }
}
