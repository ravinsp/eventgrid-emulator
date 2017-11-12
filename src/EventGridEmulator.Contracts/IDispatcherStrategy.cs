using System.Threading.Tasks;

namespace EventGridEmulator.Contracts
{
    public interface IDispatcherStrategy
    {
        Task DispatchEventAsync(EventGridEvent ev);
    }
}
