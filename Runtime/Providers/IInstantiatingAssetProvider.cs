using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public interface IInstantiatingAssetProvider<T> : IAssetProviderWithType<T>
    {
        UniTask<T> InstantiateAsync(string key);
        UniTask<T> InstantiateAsync(AssetReference reference);
        void ReleaseInstance(T instance);
    }
}