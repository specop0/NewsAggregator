namespace Parser.Plugins
{
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;

    public class Heise : Plugin
    {
        public Heise() : base("heise", "Heise Online")
        {
        }

        public override ICollection<NewsEntry> GetNews(IBrowser browser)
        {
            var newsEntries = new HashSet<NewsEntry>();
            for (var i = 1; i <= 3; i++)
            {
                var url = "https://www.heise.de/";
                if (i > 1)
                {
                    url += $"heise-online-3259407.html?p={i}";
                }
                foreach (var news in this.GetNews(browser, url))
                {
                    newsEntries.Add(news);
                }
            }

            return newsEntries;
        }

        public ICollection<NewsEntry> GetNews(IBrowser browser, string url)
        {
            var page = browser.GetPage(url);

            var articles = page.DocumentNode
                .Descendants("article")
                .Select(this.ParseArticle)
                .OfType<NewsEntry>()
                .ToList();
            return articles;
        }

        protected NewsEntry? ParseArticle(HtmlNode article)
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
    }
}