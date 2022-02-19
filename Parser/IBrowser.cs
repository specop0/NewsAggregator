namespace Parser
{
    using HtmlAgilityPack;

    public interface IBrowser
    {
        HtmlDocument GetPage(string url);
        byte[] GetData(string url);
    }
}
