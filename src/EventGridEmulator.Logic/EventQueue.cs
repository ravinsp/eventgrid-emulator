using EventGridEmulator.Contracts;
using System.Collections.Concurrent;

namespace EventGridEmulator.Logic
{
    public class EventQueue : IEventQueue
    {
        private BlockingCollection<DispatchedEvent> eventQueue = new BlockingCollection<DispatchedEvent>();

        public void Enqueue(DispatchedEvent ev)
        {
            eventQueue.Add(ev);
        }

        public DispatchedEvent Dequeue()
        {
            return eventQueue.Take();
        }
    }
}
