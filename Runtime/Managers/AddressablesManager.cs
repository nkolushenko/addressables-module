using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public static class AddressablesManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken) =>
            AssetProviderCache<T>.Provider.LoadAsync(key, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken) =>
            AssetProviderCache<T>.Provider.LoadAsync(reference, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(string key) => AssetProviderCache<T>.Provider.Release(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(AssetReference reference) => AssetProviderCache<T>.Provider.Release(reference);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> InstantiateAsync<T>(string key, CancellationToken cancellationToken)
        {
            if (AssetProviderCache<T>.Provider is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(key, cancellationToken);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<T> InstantiateAsync<T>(AssetReference reference, CancellationToken cancellationToken)
        {
            if (AssetProviderCache<T>.Provider is IInstantiatingAssetProvider<T> instantiating)
            {
                return instantiating.InstantiateAsync(reference, cancellationToken);
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