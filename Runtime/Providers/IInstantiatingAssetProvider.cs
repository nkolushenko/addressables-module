using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IInstantiatingAssetProvider<T> : IAssetProviderWithType<T>
    {
        UniTask<T> InstantiateAsync(string key, CancellationToken cancellationToken);
        UniTask<T> InstantiateAsync(AssetReference reference, CancellationToken cancellationToken);
        void ReleaseInstance(T instance);
    }
}