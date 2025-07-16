using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public abstract class RadioAudoMediaService : Plugin
{
    protected RadioAudoMediaService(string id, string name, string baseUrl, string newsUrl) : base(id, name)
    {
        this.BaseUrl = baseUrl;
        this.NewsUrl = newsUrl;
    }

    public string BaseUrl { get; }
    public string NewsUrl { get; }

    public override async Task<ICollection<NewsEntry>> GetNews(IBrowser browser)
    {
        var page = await browser.GetPage(this.NewsUrl);

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
            url = new Uri(new Uri(this.BaseUrl), url).ToString();
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
