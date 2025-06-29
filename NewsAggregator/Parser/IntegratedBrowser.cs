using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NewsAggregator.Parser;

public class IntegratedBrowser : IBrowser
{
    public HttpClient Client { get; }

    public IntegratedBrowser(HttpClient httpClient)
    {
        this.Client = httpClient;
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
