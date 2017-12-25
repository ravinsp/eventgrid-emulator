using EventGridEmulator.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventGridEmulator.Logic
{
    public class EventDispatcher
    {
        private readonly IEnumerable<SubscriptionConfiguration> _subscriptions;
        private readonly DispatcherStrategyFactory _dispatcherStrategyFactory;
        private readonly ILogger _logger;

        public EventDispatcher(IEnumerable<SubscriptionConfiguration> subscriptions,
            DispatcherStrategyFactory dispatcherStrategyFactory,
            ILogger logger)
        {
            _subscriptions = subscriptions;
            _dispatcherStrategyFactory = dispatcherStrategyFactory;
            _logger = logger;
        }

        public async Task DispatchEventAsync(EventGridEvent ev)
        {
            var type = ev.EventType;
            IEnumerable<SubscriptionConfiguration> matchingSubscriptions = GetMatchingSubscriptions(ev, _subscriptions);

            if (matchingSubscriptions.Count() == 0)
            {
                _logger.LogInfo($"No matching subscriptions for Event Id: {ev.Id}, Type: {ev.EventType}");
            }

            foreach(var subscription in matchingSubscriptions)
            {
                var dispatchStrategy = _dispatcherStrategyFactory.GetStrategy(subscription.DispatchStrategy);
                await dispatchStrategy.DispatchEventAsync(subscription.EndpointUrl, ev);
            }
        }

        public IEnumerable<SubscriptionConfiguration> GetMatchingSubscriptions(EventGridEvent ev, IEnumerable<SubscriptionConfiguration> subscriptions)
        {
            return subscriptions.Where(s =>
                (string.IsNullOrEmpty(ev.EventType) || s.EventTypes == null || s.EventTypes.Contains(ev.EventType)) && //Match event type
                (string.IsNullOrEmpty(s.SubjectBeginsWith) || (!string.IsNullOrEmpty(ev.Subject) && ev.Subject.StartsWith(s.SubjectBeginsWith))) && //Match subject prefix
                (string.IsNullOrEmpty(s.SubjectEndsWith) || (!string.IsNullOrEmpty(ev.Subject) && ev.Subject.EndsWith(s.SubjectEndsWith))) //Match subject suffix
                );
        }
    }
}
