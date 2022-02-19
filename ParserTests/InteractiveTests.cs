namespace ParserTests
{
    using System.IO;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using NSubstitute;
    using NUnit.Framework;
    using Parser;

    [Ignore("For interactive debugging")]
    public class InteractiveTests : TestsBase
    {
        public const string FileName = "InteractiveTests.html";

        [Test]
        public void DownloadHtml()
        {
            var browser = new Browser();

            // change URL while debugging
            var url = "https://www.heise.de/heise-online-3259407.html?p=1";
            var htmlDocument = browser.GetPage(url);

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

            var actualNewsEntries = plugin.GetNews(browser);
            var serializedNewsEntries = JsonConvert.SerializeObject(actualNewsEntries, Formatting.Indented);
            File.WriteAllText(FileName + ".json", serializedNewsEntries);
        }

        [Test]
        public void GetAndSetImage()
        {
            var browser = new Browser();

            var url = "https://www.tagesschau.de/multimedia/bilder/gruener-pass-italien-101~_v-gross20x9.jpg";
            var newsEntry = new NewsEntry(null, null, null, url);

            newsEntry.GetAndSetImage(browser);
        }
    }
}