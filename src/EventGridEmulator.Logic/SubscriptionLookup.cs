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
                    _logger.LogInfo($"No matching subscriptions found for event {ev.Id} (Type: {ev.EventType})");

                foreach (var de in eventsToDispatch)
                    _eventQueue.Enqueue(de);
            }
        }

        private IEnumerable<SubscriptionConfiguration> GetMatchingSubscriptions(EventGridEvent ev, IEnumerable<SubscriptionConfiguration> subscriptions)
        {
            return subscriptions.Where(s =>
                (string.IsNullOrEmpty(ev.EventType) || s.EventTypes == null || s.EventTypes.Contains(ev.EventType)) && //Match event type
                (string.IsNullOrEmpty(s.SubjectBeginsWith) || (!string.IsNullOrEmpty(ev.Subject) && ev.Subject.StartsWith(s.SubjectBeginsWith))) && //Match subject prefix
                (string.IsNullOrEmpty(s.SubjectEndsWith) || (!string.IsNullOrEmpty(ev.Subject) && ev.Subject.EndsWith(s.SubjectEndsWith))) //Match subject suffix
                );
        }
    }
}
