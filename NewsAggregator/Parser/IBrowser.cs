using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NewsAggregator.Parser;

public interface IBrowser
{
    Task<HtmlDocument> GetPage(string? url);
    Task<byte[]> GetImageData(string? url);
}
