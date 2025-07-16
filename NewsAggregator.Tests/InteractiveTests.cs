using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;
using NewsAggregator.Parser;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace NewsAggregator.Tests;

[Ignore("For interactive debugging")]
public class InteractiveTests : TestsBase
{
    public const string FileName = "InteractiveTests.html";

    public IBrowser CreateBrowser()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://seleniumminer:5022/mine/");
        return new ExternalBrowser(httpClient);
    }

    [Test]
    public async Task DownloadHtml()
    {
        var browser = this.CreateBrowser();

        // change URL while debugging
        var url = "https://www.heise.de/heise-online-5128.html?p=2";
        var htmlDocument = await browser.GetPage(url);

        htmlDocument.Save(FileName);
    }

    [Test]
    public void ParseHtml()
    {
        var browser = Substitute.For<IBrowser>();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(File.ReadAllText(FileName));
        browser.GetPage(default).ReturnsForAnyArgs(htmlDocument);

        // change plugin while debugging
        var plugin = new Parser.Plugins.Heise();

        var actualNewsEntries = plugin.GetNews(browser).Result;
        var serializedNewsEntries = JsonConvert.SerializeObject(actualNewsEntries, Formatting.Indented);
        File.WriteAllText(FileName + ".json", serializedNewsEntries);
    }

    [Test]
    public async Task GetAndSetImage()
    {
        var browser = this.CreateBrowser();

        var url = "https://www.tagesschau.de/multimedia/bilder/gruener-pass-italien-101~_v-gross20x9.jpg";
        var newsEntry = new NewsEntry(null, null, null, url);

        await newsEntry.GetAndSetImage(browser);
    }
}
