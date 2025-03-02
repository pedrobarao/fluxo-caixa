namespace FC.Cache;

public class RedisCacheOptions
{
    public string? ConnectionString { get; set; }
    public string? InstanceName { get; set; }
    public TimeSpan DefaultExpirationTime { get; set; } = TimeSpan.FromMinutes(30);
}