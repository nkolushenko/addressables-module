using UnityEngine;

namespace Core.AddressablesModule
{
    public static class AssetProviderCache<T>
    {
        public static readonly IAssetProviderWithType<T> Provider;

        static AssetProviderCache()
        {
            if (typeof(T) == typeof(GameObject))
            {
                Provider = (IAssetProviderWithType<T>)new GameObjectAssetProvider();
            }
            else
            {
                Provider = new DefaultAssetProvider<T>();
            }
        }
    }
}