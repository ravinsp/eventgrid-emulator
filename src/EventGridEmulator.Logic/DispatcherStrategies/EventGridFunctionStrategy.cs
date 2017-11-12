using EventGridEmulator.Contracts;
using System;
using System.Threading.Tasks;

namespace EventGridEmulator.Logic.PublisherStrategies
{
    public class EventGridFunctionStrategy : IDispatcherStrategy
    {
        public Task DispatchEventAsync(EventGridEvent ev)
        {
            Console.WriteLine("EventGridFunctionStrategy: Dispatching event");
            return Task.CompletedTask;
        }
    }
}
