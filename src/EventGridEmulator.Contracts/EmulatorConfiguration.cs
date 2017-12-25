using System.Collections.Generic;

namespace EventGridEmulator.Contracts
{
    public class EmulatorConfiguration
    {
        public int Port { get; set; }
        public IEnumerable<TopicConfiguration> Topics { get; set; }
        public IEnumerable<DispatchStrategyConfiguration> DispatchStrategies { get; set; }
    }

    public class TopicConfiguration
    {
        public string Name { get; set; }
        public IEnumerable<SubscriptionConfiguration> Subscriptions { get; set; }
    }

    public class SubscriptionConfiguration
    {
        public string Name { get; set; }
        public IEnumerable<string> EventTypes { get; set; }
        public string SubjectBeginsWith { get; set; }
        public string SubjectEndsWith { get; set; }
        public string EndpointUrl { get; set; }
        public string DispatchStrategy { get; set; }
    }

    public class DispatchStrategyConfiguration
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
