using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NewsAggregator.Model;

[Description("The news.")]
public class GetNewsResponse
{
    [JsonPropertyName("items")]
    public News[] Items { get; set; } = [];
}