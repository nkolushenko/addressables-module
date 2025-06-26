using System.Collections.Generic;
using Core.AddressablesModule.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule
{
    public class DefaultAssetProvider<T> : IAssetProviderWithType<T>
    {
        private readonly Dictionary<string, RefCounter<T>> _keyHandles = new();
        private readonly Dictionary<AssetReference, RefCounter<T>> _refHandles = new();

        public async UniTask<T> LoadAsync(string key)
        {
            if (_keyHandles.TryGetValue(key, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return default;
            }

            _keyHandles[key] = RefCounterPool<T>.Get(handle);
            return handle.Result;
        }

        public async UniTask<T> LoadAsync(AssetReference reference)
        {
            if (_refHandles.TryGetValue(reference, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(reference);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return default;
            }

            _refHandles[reference] = RefCounterPool<T>.Get(handle);
            return handle.Result;
        }

        public void Release(string key)
        {
            if (!_keyHandles.TryGetValue(key, out var counter))
            {
                return;
            }

            counter.RefCount--;
            if (counter.RefCount > 0)
            {
                return;
            }

            Addressables.Release(counter.Handle);
            _keyHandles.Remove(key);
            RefCounterPool<T>.Release(counter);
        }

        public void Release(AssetReference reference)
        {
            if (!_refHandles.TryGetValue(reference, out var counter))
            {
                return;
            }

            counter.RefCount--;
            if (counter.RefCount > 0)
            {
                return;
            }

            Addressables.Release(counter.Handle);
            _refHandles.Remove(reference);
            RefCounterPool<T>.Release(counter);
        }
    }
}