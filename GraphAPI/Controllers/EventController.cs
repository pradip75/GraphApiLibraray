using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GraphAPI.Controllers
{
    public class EventController : Controller
    {
        public IEventGridPublisher _publisher;
        public EventController(IEventGridPublisher publisher)
        {
            _publisher = publisher;
        }
        [HttpPost]
        [Route("api/event/trigger")]
        public async Task EventTrigger()
        {
            var topicEndpoint = "https://customer-event-notification.westeurope-1.eventgrid.azure.net/api/events";
            var topicSasKey = "yApzbfJdMkDF8dcsyVYHKxSZbTTZE1FAUrcJSTXtVLA=";

            var message = new EventMetadata()
            {
                Message = "Hello event has been trigger. Make sure the event subscription is working."
            };

            await _publisher.PublishMessage(topicEndpoint, topicSasKey, message);
        }
    }
}
