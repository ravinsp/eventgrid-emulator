using System;

namespace EventGridEmulator.Contracts
{
    public class EventGridEvent
    {
        public string subject { get; set; }
        public object data { get; set; }
        public string eventType { get; set; }
        public DateTime eventTime { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
        public string id { get; set; }

        public EventGridEvent()
        {
            dataVersion = string.Empty;
            metadataVersion = string.Empty;
        }
    }
}
