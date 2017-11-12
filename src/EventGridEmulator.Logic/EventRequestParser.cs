using EventGridEmulator.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventGridEmulator.Logic
{
    public class EventRequestParser
    {
        private readonly Dictionary<string, TopicConfiguration> _topicsLookup;
        private readonly ILogger _logger;

        public EventRequestParser(IEnumerable<TopicConfiguration> topicConfigurations, ILogger logger)
        {
            _topicsLookup = topicConfigurations.ToDictionary(t => t.Name);
            _logger = logger;
        }

        public TopicConfiguration RetrieveTopicConfigurationFromUrl(string rawUrl)
        {
            var topicName = ExtractTopicNameFromUrl(rawUrl);

            if (!string.IsNullOrEmpty(topicName))
            {
                if (_topicsLookup.ContainsKey(topicName))
                {
                    _logger.LogInfo($"Request received for topic '{topicName}'");
                    return _topicsLookup[topicName];
                }
                else
                {
                    _logger.LogError($"Topic '{topicName}' not found in configuration.");
                }
            }
            else
            {
                _logger.LogError($"Invalid request url format: '{rawUrl}'");
            }

            return null;
        }

        public async Task<IEnumerable<EventGridEvent>> ReadEventsFromStreamAsync(Stream stream, Encoding encoding)
        {
            using (var sr = new StreamReader(stream, encoding))
            {
                var body = await sr.ReadToEndAsync();

                var events = JsonConvert.DeserializeObject<EventGridEvent[]>(body);
                return events;
            }
        }

        private string ExtractTopicNameFromUrl(string rawUrl)
        {
            var components = rawUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (components.Length == 1)
                return components[0];

            return null;
        }
    }
}
