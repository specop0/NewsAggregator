using System.Collections.Generic;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    ICollection<NewsEntry> GetNews(IBrowser browser);
}
