using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewsAggregator.Model;

public class Error
{
    [Required]
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [Required]
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}