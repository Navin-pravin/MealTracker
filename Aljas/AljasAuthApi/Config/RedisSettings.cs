namespace AljasAuthApi.Config
{
    public class RedisSettings
    {
        public required string ConnectionString { get; set; }
        public required string StreamKey { get; set; }
    }
}
