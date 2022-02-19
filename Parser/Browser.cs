namespace Parser
{
    using System.Net.Http;
    using HtmlAgilityPack;

    public class Browser : IBrowser
    {
        public HttpClient Client { get; }

        public Browser()
        {
            this.Client = new HttpClient();
        }

        public HtmlDocument GetPage(string? url)
        {
            var response = this.Client.GetAsync(url).Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        public byte[] GetData(string? url)
        {
            var response = this.Client.GetAsync(url).Result;

            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsByteArrayAsync().Result;
        }
    }
}
