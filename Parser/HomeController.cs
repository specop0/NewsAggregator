namespace Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Parser.Database;

    public class HomeController : Controller
    {
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var entries = await this.GetNews();

            // return new entries
            var relevantEntries = new List<NewsEntry?>(entries.New);
            relevantEntries.Add(null);

            // return some old entries
            var oldEntriesCount = entries.New.Count > 10 ? 70 : 140;
            relevantEntries.AddRange(entries.Old.Take(oldEntriesCount));

            return this.Content(relevantEntries.ToHtml(), "text/html;charset=utf-8");
        }

        private async Task<(ICollection<NewsEntry> New, ICollection<NewsEntry> Old)> GetNews()
        {
            var database = this.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
            var browser = this.HttpContext.RequestServices.GetRequiredService<Browser>();

            var oldEntries = (await database.GetEntries()).ToList();
            var oldEntriesSet = new HashSet<NewsEntry>(oldEntries);

            var newEntries = new List<NewsEntry>();

            foreach (var newEntry in this.GetLatestNews().Reverse())
            {
                if (oldEntriesSet.TryGetValue(newEntry, out var oldEntry))
                {
                    // reorder old entry (has image and should not be removed)
                    oldEntries.Remove(oldEntry);
                    oldEntries.Insert(0, oldEntry);
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
            var plugins = Plugins.Plugins.GetPlugins();
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
}