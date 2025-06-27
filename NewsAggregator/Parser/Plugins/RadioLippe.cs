using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public class RadioLippe : Plugin
{
    public RadioLippe() : base(
        "radiolippe",
        "Radio Lippe")
    {
    }

    public async override Task<ICollection<NewsEntry>> GetNews(IBrowser browser)
    {
        var url = "https://www.radiolippe.de/nachrichten/lippe.html";
        var page = await browser.GetPage(url);

        var newsEntries = page.DocumentNode
            .Descendants("div")
            .Where(x => x.HasClass("news-img-wrap"))
            .Select(this.ParseArticle)
            .OfType<NewsEntry>()
            .ToList();

        return newsEntries;
    }

    private NewsEntry? ParseArticle(HtmlNode node, int arg2)
    {
        var titleNode = node.Descendants("a").FirstOrDefault();
        if (titleNode == null)
        {
            return null;
        }

        var url = titleNode.GetAttributeValue("href", string.Empty);
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }
        if (!url.StartsWith("https"))
        {
            url = new Uri(new Uri("https://www.radiolippe.de/"), url).ToString();
        }

        var title = titleNode.GetAttributeValue("title", string.Empty);
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var summary = string.Empty;

        var image = node.Descendants("picture")
            .SelectMany(
                x => x.Descendants("source")
                    .Where(source => source.GetAttributeValue("data-variant", string.Empty) == "medium"))
            .FirstOrDefault();
        var imageUrl = image?.GetAttributeValue("srcset", string.Empty) ?? string.Empty;

        var newsEntry = new NewsEntry(
            url,
            title,
            summary,
            imageUrl);

        return newsEntry;
    }
}
