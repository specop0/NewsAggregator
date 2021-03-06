namespace ParserTests
{
    using System;
    using System.Linq;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using Parser;
    using Parser.Plugins;

    public class PluginTests : TestsBase
    {
        public PluginTests()
        {
            // TODO: NUnit setup vs non nullable property
            this.Browser = Substitute.For<IBrowser>();
        }

        public IBrowser Browser { get; set; }

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
                return htmlDocument;
            }).ToList();

            this.Browser.GetPage(null).ReturnsForAnyArgs(
                htmlPages.FirstOrDefault(),
                htmlPages.Skip(1).ToArray());
        }

        protected void AssertParsing(IPlugin testee, string inputHtmlName, string expectedJsonName)
        {
            // Arrange
            var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
            CollectionAssert.IsNotEmpty(expectedNewsEntries, "unit test implementation");

            this.SustitutePages(this.GetResource(inputHtmlName));

            // Act
            var actualNewsEntries = testee.GetNews(this.Browser);

            // Assert
            CollectionAssert.AreEqual(expectedNewsEntries, actualNewsEntries);
        }

        [TestCase("heise-1.html", "heise-1.json")]
        [TestCase("heise-2.html", "heise-2.json")]
        [TestCase("heise-3.html", "heise-3.json")]
        public void TestHeise(string inputHtmlName, string expectedJsonName)
        {
            // Arrange
            var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
            CollectionAssert.IsNotEmpty(expectedNewsEntries, "unit test implementation");

            this.SustitutePages(this.GetResource(inputHtmlName));

            // Act
            var testee = new Heise();

            // Assert
            var actualNewsEntries = testee.GetNews(this.Browser, string.Empty);
            CollectionAssert.AreEqual(expectedNewsEntries, actualNewsEntries);
        }

        [TestCase("computerbase.html", "computerbase.json", "computerbase")]
        [TestCase("tagesschau.html", "tagesschau.json", "tagesschau")]
        [TestCase("wdr-bielefeld.html", "wdr-bielefeld.json", "wdrbielefeld")]
        [TestCase("radiohochstift.html", "radiohochstift.json", "radiohochstift")]
        [TestCase("radiolippe.html", "radiolippe.json", "radiolippe")]
        public void TestPlugin(string inputHtmlName, string expectedJsonName, string pluginId)
        {
            var testee = Plugins.GetPlugins().Single(x => x.Id == pluginId);
            this.AssertParsing(testee, inputHtmlName, expectedJsonName);
        }

        [Test]
        public void TestGetAndSetImage()
        {
            // Arrange
            var jImage = JObject.Parse(this.GetResource("image.json"));
            var inputString = jImage.Value<string>("input") ?? throw new NullReferenceException();
            var inputData = System.Convert.FromBase64String(inputString);
            this.Browser.GetData(null).ReturnsForAnyArgs(inputData);

            var newsEntry = new NewsEntry(null, null, null, "image url");

            // Act
            newsEntry.GetAndSetImage(this.Browser);

            // Assert
            var outputString = jImage.Value<string>("output");
            var expectedImageData = $"data:image/jpg;base64,{outputString}";
            Assert.AreEqual(expectedImageData, newsEntry.ImageData);
        }
    }
}