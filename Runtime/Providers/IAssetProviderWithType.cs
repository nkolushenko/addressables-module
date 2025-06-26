using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IAssetProviderWithType<T>
    {
        UniTask<T> LoadAsync(string key, CancellationToken cancellationToken);
        UniTask<T> LoadAsync(AssetReference reference, CancellationToken cancellationToken);

        void Release(string key);
        void Release(AssetReference reference);
    }
}