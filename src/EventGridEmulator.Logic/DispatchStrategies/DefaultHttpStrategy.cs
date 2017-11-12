using EventGridEmulator.Contracts;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventGridEmulator.Logic.DispatchStrategies
{
    public class DefaultHttpStrategy : IDispatcherStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        private const string JsonMimeType = "application/json";


        public DefaultHttpStrategy(ILogger logger)
        {
            _logger = logger;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("aeg-event-type", "Notification");
        }

        public async Task DispatchEventAsync(string endpointUrl, EventGridEvent ev)
        {
            _logger.LogInfo($"{Environment.NewLine}Dispatching event (Id: {ev.Id}) to '{endpointUrl}' using '{nameof(DefaultHttpStrategy)}'");

            var json = JsonConvert.SerializeObject(new EventGridEvent[] { ev });
            using (var content = new StringContent(json, Encoding.UTF8, JsonMimeType))
            {
                try
                {
                    // Send request
                    var result = await _httpClient.PostAsync(endpointUrl, content);

                    if (!result.IsSuccessStatusCode)
                    {
                        _logger.LogError(result.ToString());
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
