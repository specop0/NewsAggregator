using System.Text.Json.Serialization;
using NewsAggregator.Database;
using NewsAggregator.Model;

namespace NewsAggregator;

[JsonSerializable(typeof(GetNewsRequest))]
[JsonSerializable(typeof(GetNewsResponse))]
[JsonSerializable(typeof(RestData<NewsEntry[]>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}