using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public static class AddressablesManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken) =>
            AssetProviderRegistry.Get<T>().LoadAsync(key, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken) =>
            AssetProviderRegistry.Get<T>().LoadAsync(reference, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(string key) => AssetProviderRegistry.Get<T>().Release(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(AssetReference reference) => AssetProviderRegistry.Get<T>().Release(reference);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool
            trackHandle = false, CancellationToken cancellationToken = default)
        {
            var provider = AssetProviderRegistry.GameObjectAssetProvider;

            return provider.InstantiateAsync(key, parent, instantiateInWorldSpace,
                trackHandle, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<GameObject> InstantiateAsync<T>(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool trackHandle = false, CancellationToken cancellationToken = default)
        {
            var provider = AssetProviderRegistry.GameObjectAssetProvider;

            return provider.InstantiateAsync(reference, parent, instantiateInWorldSpace, trackHandle, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseInstance(GameObject instance)
        {
            var provider = AssetProviderRegistry.GameObjectAssetProvider;

            provider.ReleaseInstance(instance);
        }
    }
}