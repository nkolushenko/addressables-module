using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IAddressablesService
    {
        UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken);

        UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken);

        void Release<T>(string key);

        void Release<T>(AssetReference reference);

        UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool
            trackHandle = false, CancellationToken cancellationToken = default);

        UniTask<GameObject> InstantiateAsync<T>(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = false, CancellationToken cancellationToken = default);

        void ReleaseInstance(GameObject instance);

        void ClearAllLoadedAssetsByKey<T>();
        void ClearAllLoadedAssetsByRef<T>();
        void ClearAllLoadedInstantiatedAssets();
        void ForceClearAll();
    }
}