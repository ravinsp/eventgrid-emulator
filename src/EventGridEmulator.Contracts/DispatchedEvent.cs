namespace EventGridEmulator.Contracts
{
    public class DispatchedEvent
    {
        public string EndpointUrl { get; set; }
        public EventGridEvent Payload { get; set; }
        public string DispatcherStrategy { get; set; }
    }
}
