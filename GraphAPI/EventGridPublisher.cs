using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace GraphAPI

{   /// <summary>
    /// Azure service to drop event data to Event Grid
    /// </summary>
    public class EventGridPublisher : IEventGridPublisher
    {

        public EventGridPublisher()
        {
        }

        public async Task<string> PublishMessage(string topicEndpoint, string key, object eventData, string eventType = "")
        {
            using (HttpClient _httpClient = new HttpClient())
            {
                var events = new List<EventGridEvent>();

                if (string.IsNullOrEmpty(eventType))
                {
                    eventType = "custom-event";
                }

                var customEvent = new EventGridEvent
                {
                    EventType = eventType,
                    Subject = "/customtopic/message",
                    Data = JsonConvert.SerializeObject(eventData),
                    DataVersion = "1.0"
                };

                events.Add(customEvent);

                var headers = new Dictionary<string, string>
                {
                    { "aeg-sas-key",key}
                };
                var body = JsonConvert.SerializeObject(events);

                var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, topicEndpoint);

                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }

                var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
                requestMessage.Content = httpContent;

                var response = await _httpClient.SendAsync(requestMessage);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}
