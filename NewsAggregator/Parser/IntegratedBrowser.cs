using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NewsAggregator.Parser;

public class IntegratedBrowser : IBrowser
{
    public HttpClient Client { get; }

    public IntegratedBrowser(IHttpClientFactory clientFactory)
    {
        this.Client = clientFactory.CreateClient(nameof(IntegratedBrowser));
        this.Client.Timeout = TimeSpan.FromSeconds(30d);
    }

    public async Task<HtmlDocument> GetPage(string? url)
    {
        var response = await this.Client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = new HtmlDocument();
        document.LoadHtml(content);
        return document;
    }

    public async Task<byte[]> GetImageData(string? url)
    {
        var response = await this.Client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }
}
