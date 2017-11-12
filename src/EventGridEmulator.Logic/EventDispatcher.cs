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
            IEnumerable<SubscriptionConfiguration> matchingSubscriptions = null;

            if (string.IsNullOrEmpty(type))
            {
                matchingSubscriptions = _subscriptions.Where(s => s.EventTypes == null);
            }
            else
            {
                matchingSubscriptions = _subscriptions.Where(s => s.EventTypes != null && s.EventTypes.Contains(type));
            }

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
    }
}
