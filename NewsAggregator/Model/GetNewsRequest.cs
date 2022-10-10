using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace NewsAggregator.Model;

public class GetNewsRequest
{
    [SwaggerSchema("'true' if latest news shall be returned, 'false' otherwise. Default is 'true'")]
    [JsonPropertyName("isLatest")]
    public bool? IsLatest { get; set; } = true;
}