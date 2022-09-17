using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewsAggregator.Model;

// https://www.odata.org/documentation/odata-version-3-0/json-verbose-format/#representingerrorsinaresponse
public class ErrorResponse
{
    [Required]
    [JsonPropertyName("error")]
    public Error Error { get; set; } = new Error();
}
