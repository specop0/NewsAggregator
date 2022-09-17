using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NewsAggregator.Database;
using NewsAggregator.Model;
using NewsAggregator.Parser;

namespace NewsAggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NewsController : ControllerBase
{
    protected News ConvertNews(NewsEntry news)
    {
        return new News
        {
            Title = news.Title,
            Summary = news.Summary,
            Url = news.Url,
            ImageData = news.ImageData,
            ImageUrl = news.ImageUrl,
        };
    }

    /// <summary>
    /// Gets the news.
    /// </summary>
    /// <param name="isLatest"><c>true</c> if latest news shall be returned, <c>false</c> otherwise. Default is <c>true</c>.</param>
    /// <response code="200">Returns the news.</response>
    [HttpGet]
    [OperationId("getNews")]
    [ProducesResponseType(typeof(IEnumerable<News>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] bool? isLatest)
    {
        List<NewsEntry> relevantEntries;
        const int newsCount = 260;
        if (isLatest == false)
        {
            var databaseService = this.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
            relevantEntries = (await databaseService.GetEntries()).Take(newsCount).ToList();
        }
        else
        {
            var entries = await this.GetNews();

            // return new entries
            relevantEntries = new List<NewsEntry>(entries.New);

            // return some old entries
            var oldEntriesCount = entries.New.Count > 10 ? 0 : newsCount;
            relevantEntries.AddRange(entries.Old.Take(oldEntriesCount));
        }

        return this.Ok(relevantEntries.Select(this.ConvertNews).ToArray());
    }

    private async Task<(ICollection<NewsEntry> New, ICollection<NewsEntry> Old)> GetNews()
    {
        var database = this.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
        var browser = this.HttpContext.RequestServices.GetRequiredService<Browser>();

        var oldEntries = (await database.GetEntries()).ToList();
        var oldEntriesSet = new HashSet<NewsEntry>(oldEntries);

        var newEntries = new List<NewsEntry>();

        var oldEntryIndex = 0;
        foreach (var newEntry in this.GetLatestNews())
        {
            if (oldEntriesSet.TryGetValue(newEntry, out var oldEntry))
            {
                // reorder old entry (has image and should not be removed)
                oldEntries.Remove(oldEntry);
                oldEntries.Insert(oldEntryIndex, oldEntry);
                oldEntryIndex++;
            }
            else
            {
                // load image and set it as base64 encoded image
                newEntry.GetAndSetImage(browser);

                newEntries.Add(newEntry);
            }
        }

        // save at most 400 entries
        await database.SetEntries(newEntries.Concat(oldEntries).Take(400).ToArray());

        return (newEntries, oldEntries);
    }

    private ICollection<NewsEntry> GetLatestNews()
    {
        var browser = this.HttpContext.RequestServices.GetRequiredService<Browser>();

        var allParsedEntries = new List<NewsEntry>();
        var plugins = NewsAggregator.Parser.Plugins.Plugins.GetPlugins();
        foreach (var plugin in plugins)
        {
            try
            {
                allParsedEntries.AddRange(plugin.GetNews(browser));
            }
            catch (Exception exception)
            {
                allParsedEntries.Add(new NewsEntry(string.Empty, exception.Message, exception.StackTrace, string.Empty));
            }
        }

        return allParsedEntries;
    }
}
