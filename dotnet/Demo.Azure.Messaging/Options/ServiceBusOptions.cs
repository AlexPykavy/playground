namespace Demo.Azure.Messaging.Options;

public class ServiceBusOptions
{
    public string? ConnectionString { get; init; }
    public string? Url { get; init; }
    public string? TopicName { get; init; }
}
