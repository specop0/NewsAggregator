using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public class Computerbase : Plugin
{
    public Computerbase() : base("computerbase", "ComputerBase")
    {
    }

    public override async Task<ICollection<NewsEntry>> GetNews(IBrowser browser)
    {
        var url = "https://www.computerbase.de/";
        var page = await browser.GetPage(url);

        var newsEntries = page.DocumentNode
            .Descendants("li")
            .Where(x => x.HasClass("article") || x.HasClass("teaser"))
            .Select(this.ParseArticle)
            .OfType<NewsEntry>()
            .ToList();

        return newsEntries;
    }

    protected NewsEntry? ParseArticle(HtmlNode article)
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
}
