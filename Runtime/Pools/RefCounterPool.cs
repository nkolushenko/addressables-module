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
                return new RefCounter<T>(handle);
            }

            var item = Pool.Pop();
            item.Handle = handle;
            item.RefCount = 1;
            return item;
        }

        public static void Release(RefCounter<T> counter)
        {
            counter.Handle = default;
            counter.RefCount = 0;
            Pool.Push(counter);
        }
    }
}