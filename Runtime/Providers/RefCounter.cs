using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule
{
    public class RefCounter<T>
    {
        public AsyncOperationHandle<T> Handle;
        public int RefCount;

        public RefCounter(AsyncOperationHandle<T> handle)
        {
            Handle = handle;
            RefCount = 1;
        }
    }
}