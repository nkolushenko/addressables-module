using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IInstantiatingAssetProvider : IAssetProviderWithType<GameObject>
    {
        UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool
            trackHandle = false, CancellationToken cancellationToken = default);

        UniTask<GameObject> InstantiateAsync(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool
                trackHandle = false, CancellationToken cancellationToken = default);

        void ReleaseInstance(GameObject instance);
    }
}