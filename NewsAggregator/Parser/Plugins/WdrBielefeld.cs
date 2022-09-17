using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public class WdrBielefeld : Plugin
{
    public WdrBielefeld() : base("wdrbielefeld", "WDR Bielefeld")
    {
    }

    public override ICollection<NewsEntry> GetNews(IBrowser browser)
    {
        var url = "https://www1.wdr.de/nachrichten/bielefeld-nachrichten-100.html";
        var page = browser.GetPage(url);

        var newsEntries = page.DocumentNode
            .Descendants("div")
            .Where(x => x.HasClass("teaser"))
            .Select(this.ParseArticle)
            .OfType<NewsEntry>()
            .ToList();

        return newsEntries;
    }

    protected NewsEntry? ParseArticle(HtmlNode article)
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
