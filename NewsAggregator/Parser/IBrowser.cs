using HtmlAgilityPack;

namespace NewsAggregator.Parser;

public interface IBrowser
{
    HtmlDocument GetPage(string? url);
    byte[] GetData(string? url);
}
