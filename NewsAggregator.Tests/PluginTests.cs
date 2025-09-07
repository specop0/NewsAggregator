using System;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NewsAggregator.Database;
using NewsAggregator.Parser;
using NewsAggregator.Parser.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace NewsAggregator.Tests;

public class PluginTests : TestsBase
{
    public required IBrowser Browser { get; set; }

    protected override void SetUp()
    {
        this.Browser = Substitute.For<IBrowser>();
    }

    protected void SustitutePages(params string[] htmlContents)
    {
        var htmlPages = htmlContents.Select(htmlContent =>
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);
            return Task.FromResult(htmlDocument);
        }).ToList();

        this.Browser.GetPage(null).ReturnsForAnyArgs(
            htmlPages.FirstOrDefault(),
            htmlPages.Skip(1).ToArray());
    }

    protected async Task AssertParsing(IPlugin testee, string inputHtmlName, string expectedJsonName)
    {
        // Arrange
        var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
        Assert.That(expectedNewsEntries, Is.Not.Empty, "unit test implementation");

        this.SustitutePages(this.GetResource(inputHtmlName));

        // Act
        var actualNewsEntries = await testee.GetNews(this.Browser);

        // Assert
        Assert.That(actualNewsEntries, Is.EqualTo(expectedNewsEntries));
        Assert.That(
            actualNewsEntries.Select(x => x.ImageUrl).ToArray(),
            Is.EqualTo(expectedNewsEntries!.Select(x => x.ImageUrl).ToArray()));
    }

    [TestCase("heise-1.html", "heise-1.json")]
    [TestCase("heise-2.html", "heise-2.json")]
    [TestCase("heise-3.html", "heise-3.json")]
    public async Task TestHeise(string inputHtmlName, string expectedJsonName)
    {
        // Arrange
        var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
        Assert.That(expectedNewsEntries, Is.Not.Empty, "unit test implementation");

        this.SustitutePages(this.GetResource(inputHtmlName));

        // Act
        var testee = new Heise();

        // Assert
        var actualNewsEntries = await testee.GetNews(this.Browser, string.Empty);
        Assert.That(actualNewsEntries, Is.EqualTo(expectedNewsEntries));
        Assert.That(
            actualNewsEntries.Select(x => x.ImageUrl).ToArray(),
            Is.EqualTo(expectedNewsEntries!.Select(x => x.ImageUrl).ToArray()));
    }

    [TestCase("computerbase.html", "computerbase.json", "computerbase")]
    [TestCase("tagesschau.html", "tagesschau.json", "tagesschau")]
    [TestCase("wdr-bielefeld.html", "wdr-bielefeld.json", "wdrbielefeld")]
    [TestCase("radiohochstift.html", "radiohochstift.json", "radiohochstift")]
    [TestCase("radiolippe.html", "radiolippe.json", "radiolippe")]
    public async Task TestPlugin(string inputHtmlName, string expectedJsonName, string pluginId)
    {
        var testee = Plugins.GetPlugins().Single(x => x.Id == pluginId);
        await this.AssertParsing(testee, inputHtmlName, expectedJsonName);
    }

    [Test]
    public async Task TestGetAndSetImage()
    {
        // Arrange
        var jImage = JObject.Parse(this.GetResource("image.json"));
        var inputString = jImage.Value<string>("input") ?? throw new NullReferenceException();
        var inputData = System.Convert.FromBase64String(inputString);
        this.Browser.GetImageData(null).ReturnsForAnyArgs(Task.FromResult(inputData));

        var newsEntry = new NewsEntry(null, null, null, "image url");

        // Act
        await newsEntry.GetAndSetImage(this.Browser);

        // Assert
        var outputString = jImage.Value<string>("output");
        var expectedImageData = $"data:image/jpg;base64,{outputString}";
        Assert.That(newsEntry.ImageData, Is.EqualTo(expectedImageData));
    }
}
