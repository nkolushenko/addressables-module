using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AddressablesModule
{
    public static class AssetProviderRegistry
    {
        private static readonly Dictionary<Type, object> Providers = new();

        public static void Register<T>(IAssetProviderWithType<T> provider)
        {
            Providers[typeof(T)] = provider;
        }

        public static IAssetProviderWithType<T> Get<T>()
        {
            if (Providers.TryGetValue(typeof(T), out var result))
            {
                return (IAssetProviderWithType<T>)result;
            }

            return typeof(T) == typeof(GameObject)
                ? (IAssetProviderWithType<T>)new GameObjectAssetProvider()
                : new DefaultAssetProvider<T>();
        }
    }
}