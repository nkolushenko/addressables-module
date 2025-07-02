using UnityEngine;

namespace Core.AddressablesModule
{
    public class UnityLogWrapper : ILogWrapper
    {
        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
    }
}