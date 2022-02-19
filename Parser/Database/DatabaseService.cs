namespace Parser.Database
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class DatabaseService : RestServiceBase
    {
        public DatabaseService(IConfiguration configuration) : base(GetBaseUrl(configuration))
        {
        }

        private const string NewsKey = "News";

        private static string GetBaseUrl(IConfiguration configuration)
        {
            var database = configuration.Get<ParserConfiguration>().Database;

            return $"http://localhost:{database.Port}/data/{database.Authorization}/";
        }

        public async Task<ICollection<NewsEntry>> GetEntries()
        {
            var serializedEntries = await this.Do(NewsKey, "GET") ?? string.Empty;
            return JsonConvert.DeserializeObject<RestData<NewsEntry[]>>(serializedEntries)?.Data ?? new NewsEntry[0];
        }

        public async Task SetEntries(ICollection<NewsEntry> entries)
        {
            var serializedEntries = JsonConvert.SerializeObject(new RestData<NewsEntry[]> { Data = entries.ToArray() });
            await this.Do(NewsKey, "PUT", serializedEntries);
        }
    }
}