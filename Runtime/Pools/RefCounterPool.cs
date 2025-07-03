using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule.Pool
{
    public static class RefCounterPool<T>
    {
        private static readonly Stack<RefCounter<T>> Pool = new(16);

        public static RefCounter<T> Get(AsyncOperationHandle<T> handle)
        {
            if (Pool.Count <= 0)
            {
                var refCounter = new RefCounter<T>();
                refCounter.Rent(handle);
                return refCounter;
            }

            var item = Pool.Pop();
            item.Rent(handle);
            return item;
        }

        public static void Release(RefCounter<T> counter)
        {
            counter.Release();
            Pool.Push(counter);
        }
    }
}