using System;
using System.Collections.Generic;

namespace Core.AddressablesModule
{
    public class AssetProviderResolver : IAssetProviderResolver
    {
        private readonly IAssetProviderFactory _factory;
        private readonly Dictionary<Type, IAssetLoader> _cache = new();

        public AssetProviderResolver(IAssetProviderFactory factory)
        {
            _factory = factory;
        }

        public IAssetLoaderWithType<T> Get<T>()
        {
            var type = typeof(T);

            if (_cache.TryGetValue(type, out var loader))
            {
                return (IAssetLoaderWithType<T>)loader;
            }

            var created = _factory.Create<T>();
            _cache[type] = created;

            return created;
        }

        public IReadOnlyCollection<IAssetLoader> GetAllLoaders() => _cache.Values;
    }
}