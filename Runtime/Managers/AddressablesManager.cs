using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public class AddressablesManager
    {
        private readonly IAssetProviderResolver _resolver;

        public AddressablesManager(IAssetProviderResolver resolver)
        {
            _resolver = resolver;
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
            var provider = _resolver.GetSpecificProvider();

            return provider.InstantiateAsync(key, parent, instantiateInWorldSpace,
                trackHandle, cancellationToken);
        }

        public UniTask<GameObject> InstantiateAsync<T>(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = false, CancellationToken cancellationToken = default)
        {
            var provider = _resolver.GetSpecificProvider();

            return provider.InstantiateAsync(reference, parent, instantiateInWorldSpace, trackHandle, cancellationToken);
        }

        public void ReleaseInstance(GameObject instance)
        {
            var provider = _resolver.GetSpecificProvider();

            provider.ReleaseInstance(instance);
        }
    }
}