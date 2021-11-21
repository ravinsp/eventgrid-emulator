using EventGridEmulator.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace EventGridEmulator.Logic
{
    public class SubscriptionLookup
    {
        private readonly IEventQueue _eventQueue;
        private readonly ILogger _logger;

        public SubscriptionLookup(IEventQueue eventQueue, ILogger logger)
        {
            _eventQueue = eventQueue;
            _logger = logger;
        }

        public void QueueEventsToDispatch(TopicConfiguration topicConfiguration, IEnumerable<EventGridEvent> events)
        {
            foreach (var ev in events)
            {
                var eventsToDispatch = GetMatchingSubscriptions(ev, topicConfiguration.Subscriptions)
                   .Select(s => new DispatchedEvent
                   {
                       DispatcherStrategy = s.DispatchStrategy,
                       EndpointUrl = s.EndpointUrl,
                       Payload = ev
                   }).ToList();

                if (eventsToDispatch.Count == 0)
                    _logger.LogInfo($"No matching subscriptions found for event {ev.id} (Type: {ev.eventType})");

                foreach (var de in eventsToDispatch)
                    _eventQueue.Enqueue(de);
            }
        }

        private IEnumerable<SubscriptionConfiguration> GetMatchingSubscriptions(EventGridEvent ev, IEnumerable<SubscriptionConfiguration> subscriptions)
        {
            return subscriptions.Where(s =>
                (string.IsNullOrEmpty(ev.eventType) || s.EventTypes == null || s.EventTypes.Contains(ev.eventType)) && //Match event type
                (string.IsNullOrEmpty(s.SubjectBeginsWith) || (!string.IsNullOrEmpty(ev.subject) && ev.subject.StartsWith(s.SubjectBeginsWith))) && //Match subject prefix
                (string.IsNullOrEmpty(s.SubjectEndsWith) || (!string.IsNullOrEmpty(ev.subject) && ev.subject.EndsWith(s.SubjectEndsWith))) //Match subject suffix
                );
        }
    }
}
