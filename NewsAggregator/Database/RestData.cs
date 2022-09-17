using System.Text.Json.Serialization;

namespace NewsAggregator.Database;
public class RestData<T>
{
    [JsonPropertyName("$data")]
    public T? Data { get; set; }
}

