namespace GraphAPI
{
    public class CustomEvent
    {
        public string Id { get; }

        public string EventType { get; set; }

        public string Subject { get; set; }

        public string EventTime { get; }

        public string Data { get; set; }
        public string DataVersion { get; set; }

        public CustomEvent()
        {
            Id = Guid.NewGuid().ToString();
            DateTime localTime = DateTime.Now;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            EventTime = localTimeAndOffset.ToString("o");
        }
    }
}
