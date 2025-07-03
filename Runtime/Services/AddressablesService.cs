using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public class AddressablesService : IAddressablesService
    {
        private readonly IAssetProviderResolver _resolver;
        private readonly GameObjectInstantiatingAssetProvider _instantiatingAssetProvider;

        public AddressablesService(IAssetProviderResolver resolver, GameObjectInstantiatingAssetProvider instantiatingAssetProvider)
        {
            _resolver = resolver;
            _instantiatingAssetProvider = instantiatingAssetProvider;
        }

        public UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken) =>
            _resolver.Get<T>().LoadAsync(key, cancellationToken);

        public UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken) =>
            _resolver.Get<T>().LoadAsync(reference, cancellationToken);

        public void Release<T>(string key) => _resolver.Get<T>().Release(key);

        public void Release<T>(AssetReference reference) => _resolver.Get<T>().Release(reference);

        public UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool
            trackHandle = false, CancellationToken cancellationToken = default)
        {
            return _instantiatingAssetProvider.InstantiateAsync(key, parent, instantiateInWorldSpace,
                trackHandle, cancellationToken);
        }

        public UniTask<GameObject> InstantiateAsync<T>(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = false, CancellationToken cancellationToken = default)
        {
            return _instantiatingAssetProvider.InstantiateAsync(reference, parent, instantiateInWorldSpace, trackHandle, cancellationToken);
        }

        public void ReleaseInstance(GameObject instance)
        {
            _instantiatingAssetProvider.ReleaseInstance(instance);
        }

        public void ClearAllLoadedAssetsByKey<T>()
        {
            _resolver.Get<T>().ClearAllLoadedAssetsByKey();
        }

        public void ClearAllLoadedAssetsByRef<T>()
        {
            _resolver.Get<T>().ClearAllLoadedAssetsByRef();
        }

        public void ClearAllLoadedInstantiatedAssets()
        {
            _instantiatingAssetProvider.ClearAll();
        }

        public void ForceClearAll()
        {
            var loaders = _resolver.GetAllLoaders();
            foreach (var loader in loaders)
            {
                loader.ClearAll();
            }

            _instantiatingAssetProvider.ClearAll();
        }
    }
}