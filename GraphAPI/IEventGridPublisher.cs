namespace GraphAPI
{
    public interface IEventGridPublisher
    {
        Task<string> PublishMessage(string topicEndpoint, string key, object eventData, string eventType = "");

    }
}
