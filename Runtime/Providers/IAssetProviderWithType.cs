using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IAssetProviderWithType<T>
    {
        UniTask<T> LoadAsync(string key);
        UniTask<T> LoadAsync(AssetReference reference);
        
        void Release(string key);
        void Release(AssetReference reference);
    }
}