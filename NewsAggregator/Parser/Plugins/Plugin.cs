using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NewsAggregator.Database;

namespace NewsAggregator.Parser.Plugins;

public abstract class Plugin : IPlugin
{
    protected Plugin(string id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public string Id { get; }
    public string Name { get; }
    public abstract Task<ICollection<NewsEntry>> GetNews(IBrowser browser);

    private Regex? unnecessaryWhitespace;
    protected Regex UnnecessaryWhitespace => this.unnecessaryWhitespace ?? (this.unnecessaryWhitespace = new Regex("[ ]+$"));
}