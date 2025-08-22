namespace Demo.Azure.Messaging.Options;

public class EventHubOptions
{
    public string? ConnectionString { get; init; }
    public string? Url { get; init; }
    public string? HubName { get; init; }
}
