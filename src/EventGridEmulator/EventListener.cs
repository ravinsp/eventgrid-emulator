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
        
        private readonly ILogger _logger;
        private readonly EventRequestParser _requestParser;
        private readonly SubscriptionLookup _subscriptionLookup;

        public EventListener(EmulatorConfiguration config, SubscriptionLookup subscriptionLookup, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _subscriptionLookup = subscriptionLookup;
            _requestParser = new EventRequestParser(config.Topics, logger);

        }

        public async Task StartListeningAsync()
        {
            var listener = new HttpListener();
            var listenerPrefix = $"http://localhost:{_config.Port}/";

            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            _logger.LogInfo($"Started listening on {listenerPrefix}");

            while (true)
            {
                _logger.LogInfo(Environment.NewLine + "Waiting for requests...");

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
                    response.StatusCode = 200; //OK
                    response.Close();
                }
                catch(Exception ex)
                {
                    _logger.LogError("Invalid request body format. Could not parse events.");
                    _logger.LogError(ex.ToString());
                }

                if (events != null && events.Count() > 0)
                {
                    foreach(var ev in events)
                    {
                        _logger.LogInfo($"Received event: {JsonConvert.SerializeObject(ev)}");
                    }

                    _subscriptionLookup.QueueEventsToDispatch(topicConfiguration, events);
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
    }
}
;