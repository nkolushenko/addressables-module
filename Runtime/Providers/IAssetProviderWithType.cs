using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IAssetProviderWithType<T>
    {
        public bool TryUseLoaded(string key, out T asset);
        public bool TryUseLoaded(AssetReference reference, out T asset);
        
        UniTask<T> LoadAsync(string key, CancellationToken cancellationToken);
        UniTask<T> LoadAsync(AssetReference reference, CancellationToken cancellationToken);

        void Release(string key);
        void Release(AssetReference reference);
        
        void ClearAll();
    }
}