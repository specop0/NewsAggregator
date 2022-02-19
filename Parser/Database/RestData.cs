namespace Parser.Database
{
    using Newtonsoft.Json;
    public class RestData<T>
    {
        [JsonProperty("$data")]
        public T? Data { get; set; }
    }
}