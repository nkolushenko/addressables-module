using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AddressablesModule
{
    //TODO optimize - move to DI or ServiceLocator
    public class AssetProviderResolver : IAssetProviderResolver
    {
        private readonly GameObjectInstantiatingAssetProvider _sGameObjectInstantiatingAssetProvider = new();

        private readonly Dictionary<Type, object> _providers = new();

        public IAssetProviderWithType<T> Get<T>()
        {
            if (typeof(T) == typeof(GameObject))
            {
                return (IAssetProviderWithType<T>)_sGameObjectInstantiatingAssetProvider;
            }

            if (_providers.TryGetValue(typeof(T), out var result))
            {
                return (IAssetProviderWithType<T>)result;
            }

            return new DefaultAssetProvider<T>();
        }

        public GameObjectInstantiatingAssetProvider GetSpecificProvider()
        {
            return _sGameObjectInstantiatingAssetProvider;
        }
    }
}