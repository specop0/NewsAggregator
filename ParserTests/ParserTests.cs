namespace ParserTests
{
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using Parser;

    public class ParserTests : TestsBase
    {
        public Parser Testee { get; set; }

        protected override void SetUp()
        {
            this.Testee = new Parser(Substitute.For<IBrowser>());
        }

        protected void SustitutePages(params string[] htmlContents)
        {
            var htmlPages = htmlContents.Select(htmlContent =>
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
                return htmlDocument;
            }).ToList();

            this.Testee.Browser.GetPage(null).ReturnsForAnyArgs(
                htmlPages.FirstOrDefault(),
                htmlPages.Skip(1).ToArray());
        }

        protected void AssertParsing(string inputHtmlName, string expectedJsonName, string methodName)
        {
            var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
            CollectionAssert.IsNotEmpty(expectedNewsEntries, "unit test implementation");

            this.SustitutePages(this.GetResource(inputHtmlName));

            var method = this.Testee.GetType().GetMethod(methodName);
            Assert.IsNotNull(method, methodName);
            var actualNewsEntries = (ICollection<NewsEntry>)method.Invoke(this.Testee, null);
            CollectionAssert.AreEqual(expectedNewsEntries, actualNewsEntries);
        }

        [TestCase("heise-1.html", "heise-1.json")]
        [TestCase("heise-2.html", "heise-2.json")]
        [TestCase("heise-3.html", "heise-3.json")]
        public void TestHeise(string inputHtmlName, string expectedJsonName)
        {
            var expectedNewsEntries = JsonConvert.DeserializeObject<NewsEntry[]>(this.GetResource(expectedJsonName));
            CollectionAssert.IsNotEmpty(expectedNewsEntries, "unit test implementation");

            this.SustitutePages(this.GetResource(inputHtmlName));

            var actualNewsEntries = this.Testee.GetHeiseNews(string.Empty);
            CollectionAssert.AreEqual(expectedNewsEntries, actualNewsEntries);
        }

        [TestCase("computerbase.html", "computerbase.json")]
        public void TestComputerBase(string inputHtmlName, string expectedJsonName)
        {
            this.AssertParsing(inputHtmlName, expectedJsonName, nameof(Parser.GetComputerBaseNews));
        }

        [TestCase("tagesschau.html", "tagesschau.json")]
        public void TestTagesschau(string inputHtmlName, string expectedJsonName)
        {
            this.AssertParsing(inputHtmlName, expectedJsonName, nameof(Parser.GetTagesschauNews));
        }


        [TestCase("wdr-bielefeld.html", "wdr-bielefeld.json")]
        public void TestWdrBielefeld(string inputHtmlName, string expectedJsonName)
        {
            this.AssertParsing(inputHtmlName, expectedJsonName, nameof(Parser.GetWdrBielefeldNews));
        }

        [TestCase("radiohochstift.html", "radiohochstift.json", "GetRadioHochstiftNews")]
        [TestCase("radiolippe.html", "radiolippe.json", "GetRadioLippeNews")]
        public void TestRadio(string inputHtmlName, string expectedJsonName, string methodName)
        {
            this.AssertParsing(inputHtmlName, expectedJsonName, methodName);
        }

        [Test]
        public void TestGetAndSetImage()
        {
            var jImage = JObject.Parse(this.GetResource("image.json"));
            var inputString = jImage.Value<string>("input");
            var inputData = System.Convert.FromBase64String(inputString);
            this.Testee.Browser.GetData(null).ReturnsForAnyArgs(inputData);

            var newsEntry = new NewsEntry(null, null, null, "image url");
            this.Testee.GetAndSetImage(newsEntry);

            var outputString = jImage.Value<string>("output");
            var expectedImageData = $"data:image/jpg;base64,{outputString}";
            Assert.AreEqual(expectedImageData, newsEntry.ImageData);
        }
    }
}