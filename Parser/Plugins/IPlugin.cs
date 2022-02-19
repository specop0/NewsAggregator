namespace Parser.Plugins
{
    using System.Collections.Generic;

    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        ICollection<NewsEntry> GetNews(IBrowser browser);
    }
}