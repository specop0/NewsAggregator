using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NewsAggregator.Database;

public class DatabaseService
{
    public DatabaseService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        this.Client = clientFactory.CreateClient(nameof(DatabaseService));
        this.Client.BaseAddress = new Uri(GetBaseUrl(configuration));
    }

    private HttpClient Client { get; }

    private const string NewsKey = "News";

    private static string GetBaseUrl(IConfiguration configuration)
    {
        var url = configuration.GetValue<string>("database:Url") ?? string.Empty;
        if (!url.EndsWith("/"))
        {
            url = $"{url}/";
        }

        return url;
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

    protected async Task<string?> Do(string url, string httpMethod, string? input = null)
    {
        var request = new HttpRequestMessage(new HttpMethod(httpMethod), url);
        if (input != null)
        {
            request.Content = new StringContent(input);
        }

        var response = await this.Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        if (response.Content != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        return null;
    }
}
