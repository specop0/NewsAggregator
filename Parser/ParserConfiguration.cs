namespace Parser
{
    public class ParserConfiguration
    {
        public string PathBase { get; set; } = string.Empty;
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
    }

    public class DatabaseConfiguration
    {
        public int Port { get; set; }
        public string Authorization { get; set; } = string.Empty;
    }

}