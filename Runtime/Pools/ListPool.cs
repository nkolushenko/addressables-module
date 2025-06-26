using System.Collections.Generic;

namespace Core.AddressablesModule.Pool
{
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> Pool = new(10);

        public static List<T> Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new List<T>();
        }

        public static void Release(List<T> list)
        {
            list.Clear();
            Pool.Push(list);
        }
    }
}