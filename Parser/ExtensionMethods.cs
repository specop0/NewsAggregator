namespace Parser
{
    using System;
    using System.IO;
    using HtmlAgilityPack;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    public static class ExtensionMethods
    {
        public static string ToHtml(this System.Collections.Generic.IEnumerable<NewsEntry?> newsEntries)
        {
            var builder = new System.Text.StringBuilder();
            var htmlDocument = new HtmlDocument();
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<title>Nachrichten</title>");
            builder.AppendLine("<style>");
            builder.AppendLine("body { background: lightgray; }");
            builder.AppendLine("div { margin: 50px; }");
            builder.AppendLine("a { text-decoration: none; }");
            builder.AppendLine("img { max-width: 60%; min-width: 400px; }");
            builder.AppendLine(".title { font-weight: bold; }");
            builder.AppendLine("@media only screen and (min-width: 1000px) { p { width: 50%; } }");
            builder.AppendLine("</style>");
            builder.AppendLine("</head>");

            builder.AppendLine("<body>");
            foreach (var news in newsEntries)
            {
                if (news == null)
                {
                    builder.AppendLine("<hr>");
                    continue;
                }
                builder.AppendLine("<div>");
                builder.AppendLine($"<a href='{news.Url}'>");
                builder.AppendLine($"<img src='{news.ImageData ?? news.ImageUrl}'>");
                builder.AppendLine($"<p class='title'>{news.Title}</p>");
                builder.AppendLine($"<p>{news.Summary}</p>");
                builder.AppendLine($"</a>");
                builder.AppendLine("</div>");
            }
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");
            return builder.ToString();
        }

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
}