namespace Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class Database
    {
        protected HashSet<NewsEntry> ExistingEntries { get; } = new HashSet<NewsEntry>();
        protected List<NewsEntry> NewDatabaseEntries { get; } = new List<NewsEntry>();
        protected const string JsonStorageFile = "database.json";
        protected const int MaxEntries = 400;

        public Database()
        {
            if (File.Exists(JsonStorageFile))
            {
                var existingEntries = JsonConvert.DeserializeObject<NewsEntry[]>(File.ReadAllText(JsonStorageFile)) ?? new NewsEntry[0];
                existingEntries = existingEntries.Take(MaxEntries).ToArray();
                this.ExistingEntries.AddRange(existingEntries);
                this.NewDatabaseEntries.AddRange(this.ExistingEntries);
            }
        }

        public ICollection<NewsEntry> GetEntries()
        {
            return this.ExistingEntries.Take(70).ToList();
        }

        public ICollection<NewsEntry?> ParseEntries(IBrowser browser)
        {
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

            var entries = new List<NewsEntry?>();
            var isFirstNewEntry = true;
            foreach (var newEntry in allParsedEntries.AsEnumerable().Reverse())
            {
                if (this.ExistingEntries.TryGetValue(newEntry, out var oldEntry))
                {
                    // reorder old entry (has image and should not be removed)
                    this.NewDatabaseEntries.Remove(oldEntry);
                    this.NewDatabaseEntries.Insert(0, oldEntry);
                    continue;
                }

                // load image and set it as base64 encoded image
                newEntry.GetAndSetImage(browser);

                // return new entry
                if (isFirstNewEntry)
                {
                    entries.Insert(0, null);
                    isFirstNewEntry = false;
                }
                entries.Insert(0, newEntry);

                // save new entry
                this.ExistingEntries.Add(newEntry);
                this.NewDatabaseEntries.Insert(0, newEntry);
            }

            // no new entry found
            if (isFirstNewEntry)
            {
                entries.Insert(0, null);
            }

            // return some old entries
            entries.AddRange(
                this.NewDatabaseEntries
                    // skip already present entries (has 1 additional <null> entry)
                    .Skip(entries.Count - 1)
                    .Take(entries.Count > 10 ? 70 : 140));

            // delete ancient entries to reduce memory usage
            while (this.NewDatabaseEntries.Count > MaxEntries)
            {
                var ancientEntryIndex = this.NewDatabaseEntries.Count - 1;
                var ancientEntry = this.NewDatabaseEntries[ancientEntryIndex];
                this.NewDatabaseEntries.RemoveAt(ancientEntryIndex);
                this.ExistingEntries.Remove(ancientEntry);
            }

            return entries;
        }

        public void Save()
        {
            File.WriteAllText(JsonStorageFile, JsonConvert.SerializeObject(this.NewDatabaseEntries, Formatting.Indented));
        }
    }
}