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
            var relevantEntries = await this.ParseEntries();

            return this.Content(relevantEntries.ToHtml(), "text/html;charset=utf-8");
        }

        private async Task<ICollection<NewsEntry?>> ParseEntries()
        {
            var database = this.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
            var browser = this.HttpContext.RequestServices.GetRequiredService<Browser>();

            var oldEntries = new HashSet<NewsEntry>(await database.GetEntries());
            var entriesToSave = new List<NewsEntry>(oldEntries);

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

            var entriesToReturn = new List<NewsEntry?>();
            var isFirstNewEntry = true;
            foreach (var newEntry in allParsedEntries.AsEnumerable().Reverse())
            {
                if (oldEntries.TryGetValue(newEntry, out var oldEntry))
                {
                    // reorder old entry (has image and should not be removed)
                    entriesToSave.Remove(oldEntry);
                    entriesToSave.Insert(0, oldEntry);
                    continue;
                }

                // load image and set it as base64 encoded image
                newEntry.GetAndSetImage(browser);

                // return new entry
                if (isFirstNewEntry)
                {
                    entriesToReturn.Insert(0, null);
                    isFirstNewEntry = false;
                }
                entriesToReturn.Insert(0, newEntry);

                // save new entry
                oldEntries.Add(newEntry);
                entriesToSave.Insert(0, newEntry);
            }

            // no new entry found
            if (isFirstNewEntry)
            {
                entriesToReturn.Insert(0, null);
            }

            // return some old entries
            entriesToReturn.AddRange(
                entriesToSave
                    // skip already present entries (has 1 additional <null> entry)
                    .Skip(entriesToReturn.Count - 1)
                    .Take(entriesToReturn.Count > 10 ? 70 : 140));

            // save at most 400 entries
            await database.SetEntries(entriesToSave.Take(400).ToArray());

            return entriesToReturn;
        }
    }
}