using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public class Tageschau : Plugin
{
    public Tageschau() : base("tagesschau", "Tagesschau")
    {
    }

    public override async Task<ICollection<NewsEntry>> GetNews(IBrowser browser)
    {
        var url = "https://www.tagesschau.de/";
        var page = await browser.GetPage(url);

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
        var titleNodeA = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__topline"));
        var titleNodeB = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__headline"));
        var summaryNode = article.Descendants().FirstOrDefault(x => x.HasClass("teaser__shorttext"));
        if (titleNodeA == null || titleNodeB == null || summaryNode == null)
        {
            return null;
        }

        var title = string.Join(" - ", titleNodeA.InnerText, titleNodeB.InnerText);
        var summary = summaryNode.InnerText;

        var url = article.Descendants("a")
            .Select(x => x.GetAttributeValue("href", string.Empty))
            .FirstOrDefault(x => x?.EndsWith(".mp3") == false);

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

            if (url.StartsWith("//wetter.tagesschau.de"))
            {
                url = $"https:{url}";
            }
            else
            {
                url = "https://www.tagesschau.de" + url;
            }
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
}
