namespace Parser
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;

    public class Parser
    {
        public IBrowser Browser { get; }

        private Regex unnecessaryWhitespace;
        public Regex UnnecessaryWhitespace => this.unnecessaryWhitespace ?? (this.unnecessaryWhitespace = new Regex("[ ]+$"));

        public Parser(IBrowser browser)
        {
            this.Browser = browser;
        }

        public void GetAndSetImage(NewsEntry newsEntry)
        {
            if (string.IsNullOrWhiteSpace(newsEntry.ImageUrl))
            {
                return;
            }

            try
            {
                var imageData = this.Browser.GetData(newsEntry.ImageUrl);
                var imageString = System.Text.Encoding.UTF8.GetString(imageData);
                if (imageData == null)
                {
                    return;
                }

                using (var inputStream = new MemoryStream(imageData))
                using (var inputImage = Image.FromStream(inputStream))
                using (var outputStream = new MemoryStream())
                {
                    var resizedImage = inputImage;
                    if (inputImage.Size.Width > 450 || inputImage.Size.Height > 450)
                    {
                        var maxLength = inputImage.Size.Width > inputImage.Size.Height
                            ? inputImage.Size.Width
                            : inputImage.Size.Height;
                        var scalingFactor = 450d / maxLength;
                        var newSize = new Size(
                            (int)Math.Round(inputImage.Size.Width * scalingFactor),
                            (int)Math.Round(inputImage.Size.Height * scalingFactor));
                        resizedImage = new Bitmap(inputImage, newSize);
                    }

                    var codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == ImageFormat.Jpeg.Guid);

                    var parameters = new EncoderParameters(1);
                    parameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

                    resizedImage.Save(
                        outputStream,
                        codec,
                        parameters
                    );

                    var convertedImageData = outputStream.ToArray();
                    var convertedBaseImageData = System.Convert.ToBase64String(convertedImageData);
                    newsEntry.ImageData = $"data:image/jpg;base64,{convertedBaseImageData}";

                    resizedImage.Dispose();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public ICollection<NewsEntry> GetHeiseNews(string url)
        {
            var page = this.Browser.GetPage(url);

            var articles = page.DocumentNode
                .Descendants("article")
                .Select(this.ParseHeiseArticle)
                .Where(x => x != null)
                .ToList();
            return articles;
        }

        protected NewsEntry ParseHeiseArticle(HtmlNode article)
        {
            var urlNode = article.Descendants("a").FirstOrDefault();
            var url = urlNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;
            if (string.IsNullOrEmpty(url) || url == "${url}")
            {
                return null;
            }

            var titleNode = article
                .Descendants()
                .FirstOrDefault(x => x.GetAttributeValue("data-component", string.Empty) == "TeaserHeadline");
            if (titleNode == null)
            {
                return null;
            }
            var title = this.UnnecessaryWhitespace.Replace(titleNode.InnerText, string.Empty);

            var summaryNode = article
                .Descendants()
                .FirstOrDefault(x => x.GetAttributeValue("data-component", string.Empty) == "TeaserSynopsis");
            if (summaryNode == null)
            {
                return null;
            }
            var summary = this.UnnecessaryWhitespace.Replace(summaryNode.InnerText, string.Empty);

            var image = article.Descendants("img").FirstOrDefault();
            string imageUrl = string.Empty;
            if (image != null)
            {
                imageUrl = image?.GetAttributeValue("src", string.Empty) ?? string.Empty;
                if (imageUrl.StartsWith("data"))
                {
                    imageUrl = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                image = article.Descendants("a-img").FirstOrDefault();
                if (image != null)
                {
                    imageUrl = image?.GetAttributeValue("src", string.Empty) ?? string.Empty;
                }
            }

            if (url.StartsWith("/"))
            {
                url = "https://www.heise.de" + url;
            }

            if (imageUrl.StartsWith("/"))
            {
                imageUrl = "https://www.heise.de" + imageUrl;
            }

            var newsEntry = new NewsEntry(
                url,
                title,
                summary,
                imageUrl);

            return newsEntry;
        }

        public ICollection<NewsEntry> GetHeiseNews()
        {
            var newsEntries = new HashSet<NewsEntry>();
            for (var i = 1; i <= 3; i++)
            {
                var url = "https://www.heise.de/";
                if (i > 1)
                {
                    url += $"heise-online-3259407.html?p={i}";
                }
                foreach (var news in this.GetHeiseNews(url))
                {
                    newsEntries.Add(news);
                }
            }

            return newsEntries;
        }

        public ICollection<NewsEntry> GetComputerBaseNews()
        {
            var url = "https://www.computerbase.de/";
            var page = this.Browser.GetPage(url);

            var newsEntries = page.DocumentNode
                .Descendants("li")
                .Where(x => x.HasClass("article") || x.HasClass("teaser"))
                .Select(this.ParseComputerBaseArticle)
                .Where(x => x != null)
                .ToList();

            return newsEntries;
        }

        protected NewsEntry ParseComputerBaseArticle(HtmlNode article)
        {
            var titleNode = article.Descendants("a").FirstOrDefault(x => x.HasClass("visited-link"));
            if (titleNode == null)
            {
                return null;
            }

            var url = titleNode.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var title = string.Join(" - ", titleNode.Descendants("span").Select(x => x.InnerText));
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            var summary = string.Empty;
            var summaryNode = article.Descendants("p").FirstOrDefault(x => x.HasClass("article__intro"));
            if (summaryNode != null)
            {
                summary = summaryNode.InnerText;
            }

            var image = article.Descendants("img").FirstOrDefault();
            var imageUrl = image?.GetAttributeValue("src", string.Empty) ?? string.Empty;
            imageUrl = imageUrl.Replace("300x169", "300x300");
            imageUrl = imageUrl.Replace("75x75", "300x300");

            if (url.StartsWith("/"))
            {
                url = "https://www.computerbase.de" + url;
            }

            if (imageUrl.StartsWith("/"))
            {
                imageUrl = "https://www.computerbase.de" + imageUrl;
            }

            var newsEntry = new NewsEntry(
                url,
                title,
                summary,
                imageUrl);

            return newsEntry;
        }

        public ICollection<NewsEntry> GetTagesschauNews()
        {
            var url = "https://www.tagesschau.de/";
            var page = this.Browser.GetPage(url);

            var newsEntries = page.DocumentNode
                .Descendants("div")
                .Where(x => x.HasClass("teaser"))
                .Select(this.ParseTagesschauArticle)
                .Where(x => x != null)
                .ToList();

            return newsEntries;
        }

        protected NewsEntry ParseTagesschauArticle(HtmlNode article)
        {
            var titleNodeA = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__topline"));
            var titleNodeB = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__headline"));
            var summaryNode = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__shorttext"));
            if (titleNodeA == null || titleNodeB == null || summaryNode == null)
            {
                return null;
            }

            var title = string.Join(" - ", titleNodeA.InnerText, titleNodeB.InnerText);
            var summary = summaryNode.InnerText;

            var urlElement = article.Descendants("a").FirstOrDefault();
            if (urlElement == null)
            {
                return null;
            }

            var url = urlElement.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var image = article.Descendants("img").FirstOrDefault();
            var imageUrl = image?.GetAttributeValue("src", string.Empty) ?? string.Empty;

            if (string.IsNullOrEmpty(imageUrl))
            {
                var javascriptImage = article.Descendants("div").FirstOrDefault(x => x.HasClass("ts-mediaplayer"));
                if (javascriptImage != null)
                {
                    var imageData = javascriptImage.GetAttributeValue("data-config", string.Empty);
                    var splittedImageData = imageData.Split(new[] { "&quot;" }, System.StringSplitOptions.RemoveEmptyEntries);
                    imageUrl = splittedImageData.LastOrDefault(x => x.Contains(".jpg") && !x.Contains("audioplayer")) ?? string.Empty;
                }
            }

            if (url.StartsWith("/"))
            {
                url = "https://www.tagesschau.de" + url;
            }

            if (imageUrl.StartsWith("/"))
            {
                imageUrl = "https://www.tagesschau.de" + imageUrl;
            }

            var newsEntry = new NewsEntry(
                url,
                title,
                summary,
                imageUrl);

            return newsEntry;
        }

        public ICollection<NewsEntry> GetRadioLippeNews()
        {
            return this.GetRadioNews(
                "https://www.radiolippe.de/nachrichten/lippe.html",
                "https://www.radiolippe.de/");
        }

        public ICollection<NewsEntry> GetRadioHochstiftNews()
        {
            return this.GetRadioNews(
                "https://www.radiohochstift.de/nachrichten/paderborn-hoexter.html",
                "https://www.radiohochstift.de/");
        }
        public ICollection<NewsEntry> GetRadioNews(string url, string baseUrl)
        {
            var page = this.Browser.GetPage(url);

            var newsEntries = page.DocumentNode
                .Descendants("div")
                .Where(x => x.HasClass("row"))
                .Select(x => this.ParseRadioArticle(x, baseUrl))
                .Where(x => x != null)
                .ToList();

            return newsEntries;
        }

        protected NewsEntry ParseRadioArticle(HtmlNode article, string baseUrl)
        {
            var titleNode = article.Descendants("h4").FirstOrDefault();
            if (titleNode == null)
            {
                return null;
            }

            var url = titleNode.ParentNode.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            if (!url.StartsWith("https"))
            {
                url = baseUrl + url;
            }

            var title = titleNode.InnerText;
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            var summary = string.Empty;
            var summaryNode = article.Descendants("p").FirstOrDefault(x => x.HasClass("bodytext"));
            if (summaryNode != null)
            {
                summary = summaryNode.InnerText;
            }

            var image = article.Descendants("img").FirstOrDefault();
            var imageUrl = image?.GetAttributeValue("src", string.Empty) ?? string.Empty;

            var newsEntry = new NewsEntry(
                url,
                title,
                summary,
                imageUrl);

            return newsEntry;
        }

        public ICollection<NewsEntry> GetWdrBielefeldNews()
        {
            var url = "https://www1.wdr.de/nachrichten/bielefeld-nachrichten-100.html";
            var page = this.Browser.GetPage(url);

            var newsEntries = page.DocumentNode
                .Descendants("div")
                .Where(x => x.HasClass("teaser"))
                .Select(this.ParseWdrBielefeldArticle)
                .Where(x => x != null)
                .ToList();

            return newsEntries;
        }

        protected NewsEntry ParseWdrBielefeldArticle(HtmlNode article)
        {
            var urlElement = article.Descendants("a").FirstOrDefault();
            if (urlElement == null)
            {
                return null;
            }

            var url = urlElement.GetAttributeValue("href", string.Empty);
            var title = urlElement.GetAttributeValue("title", string.Empty);
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title))
            {
                return null;
            }

            var summaryElement = urlElement.Descendants("p").FirstOrDefault(x => x.HasClass("teasertext"));
            var summary = summaryElement?.InnerText ?? "";

            var imageElement = urlElement.Descendants("picture").FirstOrDefault()?.Descendants("img").FirstOrDefault();
            var imageUrl = imageElement?.GetAttributeValue("src", string.Empty) ?? string.Empty;

            if (url.StartsWith("/"))
            {
                url = "https://www1.wdr.de" + url;
            }

            if (imageUrl.StartsWith("/"))
            {
                imageUrl = "https://www1.wdr.de" + imageUrl;
            }

            return new NewsEntry(url, title, summary, imageUrl);
        }
    }
}
