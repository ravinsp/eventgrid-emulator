using EventGridEmulator.Contracts;
using EventGridEmulator.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EventGridEmulator
{
    public class EventListener
    {
        private readonly EmulatorConfiguration _config;
        private readonly Dictionary<string, DispatchStrategyConfiguration> _strategyLookups;
        private readonly ILogger _logger;
        private readonly EventRequestParser _requestParser;

        public EventListener(EmulatorConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _requestParser = new EventRequestParser(config.Topics, logger);

            _strategyLookups = config.DispatchStrategies.ToDictionary(s => s.Name);
        }

        public async Task StartListeningAsync()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add(string.Format("http://*:{0}/", _config.Port));
            listener.Start();
            _logger.LogInfo($"Started listening on port {_config.Port}...");

            while (true)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                _logger.LogHeading($"Received new request on {DateTime.Now}");
                await HandleRequestAsync(request, response);
            }
        }

        private async Task HandleRequestAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var topicConfiguration = _requestParser.RetrieveTopicConfigurationFromUrl(request.RawUrl);

            if (topicConfiguration != null)
            {
                IEnumerable<EventGridEvent> events = null;

                try
                {
                    events = await _requestParser.ReadEventsFromStreamAsync(request.InputStream, request.ContentEncoding);
                }
                catch
                {
                    _logger.LogError("Invalid reqest body format. Could not parse events.");
                }

                if (events != null && events.Count() > 0)
                {
                    await DispatchEventsAsync(topicConfiguration, events);

                    response.StatusCode = 200; //OK
                    response.Close();
                    return;
                }
                else
                {
                    _logger.LogError("No events could be identified from the request.");
                }
            }

            response.StatusCode = 400; //bad request
            response.Close();
        }

        private async Task DispatchEventsAsync(TopicConfiguration topicConfiguration, IEnumerable<EventGridEvent> events)
        {
            var dispatcher = new EventDispatcher(topicConfiguration.Subscriptions, _strategyLookups, _logger);

            foreach (var ev in events)
            {
                _logger.LogInfo($"Received event: {JsonConvert.SerializeObject(ev)}");
                await dispatcher.DispatchEventAsync(ev);
            }
        }
    }
}
