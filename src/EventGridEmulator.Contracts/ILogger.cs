namespace EventGridEmulator.Contracts
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogHeading(string heading);
    }
}
