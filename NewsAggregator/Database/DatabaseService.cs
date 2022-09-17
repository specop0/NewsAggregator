using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NewsAggregator.Database;

public class DatabaseService : RestServiceBase
{
    public DatabaseService(IConfiguration configuration) : base(GetBaseUrl(configuration))
    {
    }

    private const string NewsKey = "News";

    private static string GetBaseUrl(IConfiguration configuration)
    {
        var port = configuration.GetValue<string>("database:Port");
        var authorization = configuration.GetValue<string>("database:Authorization");

        return $"http://localhost:{port}/data/{authorization}/";
    }

    public async Task<ICollection<NewsEntry>> GetEntries()
    {
        var serializedEntries = await this.Do(NewsKey, "GET") ?? string.Empty;
        return JsonSerializer.Deserialize<RestData<NewsEntry[]>>(serializedEntries)?.Data ?? new NewsEntry[0];
    }

    public async Task SetEntries(ICollection<NewsEntry> entries)
    {
        var serializedEntries = JsonSerializer.Serialize(new RestData<NewsEntry[]> { Data = entries.ToArray() });
        await this.Do(NewsKey, "PUT", serializedEntries);
    }
}
