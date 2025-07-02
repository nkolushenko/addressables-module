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
        public static UniTask<GameObject> InstantiateAsync(string key, CancellationToken cancellationToken)
        {
            var provider = AssetProviderRegistry.Get<GameObjectAssetProvider>();
            if (AssetProviderRegistry.Get<GameObjectAssetProvider>() is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(key, cancellationToken);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> InstantiateAsync<T>(AssetReference reference, CancellationToken cancellationToken)
        {
            if (AssetProviderRegistry.Get<T>() is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(reference, cancellationToken);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReleaseInstance<T>(T instance)
        {
            if (AssetProviderRegistry.Get<T>() is IInstantiatingAssetProvider<T> instantiating)
            {
                instantiating.ReleaseInstance(instance);
            }
        }
    }
}