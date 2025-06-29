using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsAggregator.Database;
using NewsAggregator.Model;
using NewsAggregator.Parser;

namespace NewsAggregator.UseCases;

public class GetNewsHandler
{
    public GetNewsHandler(DatabaseService database, IBrowser browser)
    {
        this.Database = database;
        this.Browser = browser;
    }

    private DatabaseService Database { get; }
    private IBrowser Browser { get; }

    private static News ConvertNews(NewsEntry news)
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

    public async Task<News[]> Invoke(bool isLatest)
    {
        List<NewsEntry> relevantEntries;
        const int newsCount = 260;
        if (!isLatest)
        {
            relevantEntries = (await this.Database.GetEntries()).Take(newsCount).ToList();
        }
        else
        {
            var entries = await GetNews();

            // return new entries
            relevantEntries = new List<NewsEntry>(entries.New);

            // return some old entries
            var oldEntriesCount = entries.New.Count > 10 ? 0 : newsCount;
            relevantEntries.AddRange(entries.Old.Take(oldEntriesCount));
        }

        var news = relevantEntries.Select(ConvertNews).ToArray();
        return news;
    }

    private async Task<(ICollection<NewsEntry> New, ICollection<NewsEntry> Old)> GetNews()
    {
        var oldEntries = (await this.Database.GetEntries()).ToList();
        var oldEntriesSet = new HashSet<NewsEntry>(oldEntries);

        var newEntries = new List<NewsEntry>();

        var oldEntryIndex = 0;
        foreach (var newEntry in await this.GetLatestNews())
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
                await newEntry.GetAndSetImage(this.Browser);

                newEntries.Add(newEntry);
            }
        }

        // save at most 400 entries
        await this.Database.SetEntries(newEntries.Concat(oldEntries).Take(400).ToArray());

        return (newEntries, oldEntries);
    }

    private async Task<ICollection<NewsEntry>> GetLatestNews()
    {
        var allParsedEntries = new List<NewsEntry>();
        var plugins = NewsAggregator.Parser.Plugins.Plugins.GetPlugins();
        foreach (var plugin in plugins)
        {
            try
            {
                allParsedEntries.AddRange(await plugin.GetNews(this.Browser));
            }
            catch (Exception exception)
            {
                allParsedEntries.Add(new NewsEntry(string.Empty, exception.Message, exception.StackTrace, string.Empty));
            }
        }

        return allParsedEntries;
    }
}