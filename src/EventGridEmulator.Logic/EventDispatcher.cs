using EventGridEmulator.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventGridEmulator.Logic
{
    public class EventDispatcher
    {
        private readonly IEnumerable<SubscriptionConfiguration> _subscriptions;
        private readonly Dictionary<string, DispatchStrategyConfiguration> _strategyLookups;
        private readonly ILogger _logger;

        public EventDispatcher(
            IEnumerable<SubscriptionConfiguration> subscriptions,
            Dictionary<string, DispatchStrategyConfiguration> strategyLookups,
            ILogger logger)
        {
            _subscriptions = subscriptions;
            _strategyLookups = strategyLookups;
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

            foreach(var subscription in matchingSubscriptions)
            {
                var dispatchStrategy = CreateDispatchStratgey(subscription.DispatchStrategy);
                await dispatchStrategy.DispatchEventAsync(ev);
            }
        }

        private IDispatcherStrategy CreateDispatchStratgey(string name)
        {
            if (_strategyLookups.ContainsKey(name))
            {
                var strategyDef = _strategyLookups[name];
                var strategy = (IDispatcherStrategy)Activator.CreateInstance(strategyDef.Assembly, strategyDef.Type).Unwrap();

                return strategy;
            }

            _logger.LogError($"Invalid dispatch strategy name '{name}'.");
            return null;
        }
    }
}
