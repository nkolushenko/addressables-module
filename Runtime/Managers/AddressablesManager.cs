using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public static class AddressablesManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(string key) => AssetProviderCache<T>.Provider.LoadAsync(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(AssetReference reference) => AssetProviderCache<T>.Provider.LoadAsync(reference);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(string key) => AssetProviderCache<T>.Provider.Release(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(AssetReference reference) => AssetProviderCache<T>.Provider.Release(reference);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> InstantiateAsync<T>(string key)
        {
            if (AssetProviderCache<T>.Provider is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(key);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> InstantiateAsync<T>(AssetReference reference)
        {
            if (AssetProviderCache<T>.Provider is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(reference);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseInstance<T>(T instance)
        {
            if (AssetProviderCache<T>.Provider is IInstantiatingAssetProvider<T> instantiating)
            {
                instantiating.ReleaseInstance(instance);
            }
        }
    }
}