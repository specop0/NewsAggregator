using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NewsAggregator.Model;

public class GetNewsRequest
{
    [Description("'true' if latest news shall be returned, 'false' otherwise. Default is 'false'")]
    [JsonPropertyName("isLatest")]
    public bool? IsLatest { get; set; } = false;
}