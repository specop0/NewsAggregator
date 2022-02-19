namespace Parser.Database
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RestServiceBase
    {
        protected RestServiceBase(string baseUrl)
        {
            this.Client = new HttpClient();
            this.Client.BaseAddress = new Uri(baseUrl);
        }

        private HttpClient Client { get; }

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
}