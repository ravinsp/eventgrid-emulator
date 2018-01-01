namespace EventGridEmulator.Contracts
{
    public interface IEventQueue
    {
        void Enqueue(DispatchedEvent ev);
        DispatchedEvent Dequeue();
    }
}
