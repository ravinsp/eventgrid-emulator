using EventGridEmulator.Contracts;
using EventGridEmulator.Logic;
using System.Threading.Tasks;

namespace EventGridEmulator
{
    /// <summary>
    /// Sends the events to their destination as they get queued.
    /// </summary>
    public class EventDispatcher
    {
        private readonly EmulatorConfiguration _config;
        private readonly IEventQueue _eventQueue;
        private readonly ILogger _logger;
        private readonly DispatcherStrategyFactory _dispatcherStrategyFactory;

        public EventDispatcher(EmulatorConfiguration config, IEventQueue eventQueue, ILogger logger)
        {
            _config = config;
            _eventQueue = eventQueue;
            _logger = logger;
            _dispatcherStrategyFactory = new DispatcherStrategyFactory(config.DispatchStrategies, logger);
        }

        public async Task StartListening()
        {
            while (true)
            {
                var dispatchedEvent = _eventQueue.Dequeue();
                var strategy = _dispatcherStrategyFactory.GetStrategy(dispatchedEvent.DispatcherStrategy);
                await strategy.DispatchEventAsync(dispatchedEvent.EndpointUrl, dispatchedEvent.Payload);
            }
        }
    }
}
