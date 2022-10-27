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
            .Where(x => x.HasClass("row"))
            .Select(x => this.ParseArticle(x))
            .OfType<NewsEntry>()
            .ToList();

        return newsEntries;
    }

    protected NewsEntry? ParseArticle(HtmlNode article)
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
            url = this.BaseUrl + url;
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
}
