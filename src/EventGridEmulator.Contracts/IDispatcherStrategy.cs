using System.Threading.Tasks;

namespace EventGridEmulator.Contracts
{
    public interface IDispatcherStrategy
    {
        Task DispatchEventAsync(string endpointUrl, EventGridEvent ev);
    }
}
