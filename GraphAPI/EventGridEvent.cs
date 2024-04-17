using Newtonsoft.Json;

namespace GraphAPI
{
    public class EventGridEvent
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("EventType")]
        public string EventType { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("EventTime")]
        public DateTimeOffset EventTime { get; set; }

        [JsonProperty("Data")]
        public dynamic Data { get; set; }

        [JsonProperty("DataVersion")]
        public string DataVersion { get; set; }

        [JsonProperty("metadataVersion")]
        public long MetadataVersion { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }
    }
}
