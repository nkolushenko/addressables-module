using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AddressablesModule
{
    //TODO optimize 
    public static class AssetProviderRegistry
    {
        private static readonly GameObjectAssetProvider s_gameObjectAssetProvider = new();

        private static readonly Dictionary<Type, object> Providers = new();

        public static void Register<T>(IAssetProviderWithType<T> provider)
        {
            Providers[typeof(T)] = provider;
        }

        public static GameObjectAssetProvider GameObjectAssetProvider => s_gameObjectAssetProvider;

        public static IAssetProviderWithType<T> Get<T>()
        {
            if (typeof(T) == typeof(GameObject))
            {
                return (IAssetProviderWithType<T>)GameObjectAssetProvider;
            }

            if (Providers.TryGetValue(typeof(T), out var result))
            {
                return (IAssetProviderWithType<T>)result;
            }

            return new DefaultAssetProvider<T>();
        }

        public static T GetProvider<T>() where T : class, IAssetProvider
        {
            if (Providers.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
            }

            return (T)(typeof(T) == typeof(GameObject)
                ? (IAssetProviderWithType<T>)new GameObjectAssetProvider()
                : new DefaultAssetProvider<T>());
        }
    }
}