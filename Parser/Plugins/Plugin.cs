namespace Parser.Plugins
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public abstract class Plugin : IPlugin
    {
        protected Plugin(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; }
        public string Name { get; }
        public abstract ICollection<NewsEntry> GetNews(IBrowser browser);


        private Regex? unnecessaryWhitespace;
        protected Regex UnnecessaryWhitespace => this.unnecessaryWhitespace ?? (this.unnecessaryWhitespace = new Regex("[ ]+$"));

    }
}