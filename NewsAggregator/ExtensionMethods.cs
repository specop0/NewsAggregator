using System;
using System.IO;
using NewsAggregator.Database;
using NewsAggregator.Parser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace NewsAggregator;

public static class ExtensionMethods
{
    public static void GetAndSetImage(this NewsEntry newsEntry, IBrowser browser)
    {
        if (string.IsNullOrWhiteSpace(newsEntry.ImageUrl))
        {
            return;
        }

        try
        {
            var imageData = browser.GetData(newsEntry.ImageUrl);
            if (imageData == null)
            {
                return;
            }

            var resizedImageData = imageData.ResizeImageIfTooLarge(450);

            var serializedImageData = System.Convert.ToBase64String(resizedImageData);
            newsEntry.ImageData = $"data:image/jpg;base64,{serializedImageData}";
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static byte[] ResizeImageIfTooLarge(this byte[] imageData, int maxWidthOrHeight)
    {
        using (var inputStream = new MemoryStream(imageData))
        using (var inputImage = Image.Load(inputStream))
        using (var outputStream = new MemoryStream())
        {
            var width = inputImage.Width;
            var height = inputImage.Height;

            var maxLength = Math.Max(width, height);
            if (maxLength < maxWidthOrHeight)
            {
                return imageData;
            }

            var scalingFactor = (double)maxWidthOrHeight / maxLength;
            var newSize = new Size(
                (int)Math.Round(width * scalingFactor),
                (int)Math.Round(height * scalingFactor));

            using (var resizedImage = inputImage.Clone(imageContext => imageContext.Resize(newSize)))
            {
                resizedImage.Save(
                    outputStream,
                    new JpegEncoder
                    {
                        Quality = 70
                    }
                );
                var convertedImageData = outputStream.ToArray();
                return convertedImageData;
            }
        }
    }
}
