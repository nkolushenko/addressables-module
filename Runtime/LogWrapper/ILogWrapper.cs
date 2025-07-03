namespace Core.AddressablesModule
{
    public interface ILogWrapper
    {
        void LogWarning(string message);
        void LogError(string message);
    }
}