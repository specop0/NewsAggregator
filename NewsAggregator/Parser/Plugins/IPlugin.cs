using System.Collections.Generic;
using System.Threading.Tasks;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public interface IPlugin
{
    string Id { get; }
    string Name { get; }
    Task<ICollection<NewsEntry>> GetNews(IBrowser browser);
}
