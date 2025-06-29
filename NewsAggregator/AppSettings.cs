namespace NewsAggregator;

public class AppSettings
{
    public string? PathBase { get; set; }

    public AppCorsSettings? Cors { get; set; }
}

public class AppCorsSettings
{
    public string[]? AllowedOrigins { get; set; }

}
