using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule
{
    public class RefCounter<T>
    {
        public AsyncOperationHandle<T> Handle { get; private set; }
        public int RefCount { get; private set; }

        public void Rent(AsyncOperationHandle<T> handle)
        {
            Handle = handle;
            RefCount = 1;
        }

        public void Release()
        {
            Handle = default;
            RefCount = 0;
        }

        public void IncrementRef()
        {
            RefCount++;
        }

        public void DecrementRef()
        {
            RefCount--;
        }
    }
}