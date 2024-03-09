using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

namespace NewsAggregator.Parser;

public class ExternalBrowser : IBrowser
{
    public HttpClient Client { get; }

    public ExternalBrowser(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        this.Client = clientFactory.CreateClient(nameof(ExternalBrowser));
        this.Client.Timeout = TimeSpan.FromSeconds(30d);
        var externalBrowserUrl = configuration.GetSection("Browser:Url").Get<string>() ?? string.Empty;
        if (!externalBrowserUrl.EndsWith("/"))
        {
            externalBrowserUrl += "/";
        }
        this.Client.BaseAddress = new Uri(externalBrowserUrl);
    }

    public async Task<HtmlDocument> GetPage(string? url)
    {
        var request = new JsonObject
        {
            ["url"] = url
        };
        var response = await this.Client.PostAsync(
            "pageSource",
            new StringContent(request.ToJsonString()));

        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsStringAsync();
        var content = JsonNode.Parse(jsonContent)?["pageSource"]?.GetValue<string>();
        var document = new HtmlDocument();
        document.LoadHtml(content);
        return document;
    }

    public async Task<byte[]> GetImageData(string? url)
    {
        var request = new JsonObject
        {
            ["url"] = url
        };
        var response = await this.Client.PostAsync(
            "screenshot",
            new StringContent(request.ToJsonString()));

        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsStringAsync();
        var content = JsonNode.Parse(jsonContent)?["imageData"]?.GetValue<string>() ?? string.Empty;
        return System.Convert.FromBase64String(content);
    }
}
