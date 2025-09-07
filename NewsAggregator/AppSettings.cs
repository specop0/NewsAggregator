namespace NewsAggregator;

public class AppSettings
{
    public string? PathBase { get; set; }

    public AppCorsSettings? Cors { get; set; }

    public AppDatabaseSettings? Database { get; set; }

    public AppBrowserSettings? Browser { get; set; }

    public AuthenticationSettings? Authentication { get; set; }
}

public class AppCorsSettings
{
    public string[]? AllowedOrigins { get; set; }

}

public class AppDatabaseSettings
{
    public string? Url { get; set; }
}


public class AppBrowserSettings
{
    public string? Url { get; set; }
    public bool? UseIntegrated { get; set; }
}

public class AuthenticationSettings
{
    public string? Authority { get; set; }
    public string? Audience { get; set; }
}
