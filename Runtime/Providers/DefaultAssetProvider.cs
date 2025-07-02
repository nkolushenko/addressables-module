using System.Collections.Generic;
using System.Threading;
using Core.AddressablesModule.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule
{
    //TODO
    //1 add custom logger - package realization
    //2 TryGetLoaded - sync method ?
    //3 asset guid - test on build - reference key better?
    public class DefaultAssetProvider<T> : IAssetProviderWithType<T>
    {
        private readonly ILogWrapper _logger;
        private readonly Dictionary<string, RefCounter<T>> _keyHandles = new();
        private readonly Dictionary<string, RefCounter<T>> _refHandles = new();

        public DefaultAssetProvider(ILogWrapper logger)
        {
            _logger = logger;
        }

        public bool TryUseLoaded(string key, out T asset)
        {
            if (string.IsNullOrEmpty(key))
            {
                asset = default;
                return false;
            }

            if (_keyHandles.TryGetValue(key, out var counter) &&
                counter.Handle.IsValid() &&
                counter.Handle.Status == AsyncOperationStatus.Succeeded &&
                counter.Handle.Result != null)
            {
                counter.RefCount++;
                asset = counter.Handle.Result;
                return true;
            }

            asset = default;
            return false;
        }

        public bool TryUseLoaded(AssetReference reference, out T asset)
        {
            if (string.IsNullOrEmpty(reference.AssetGUID))
            {
                asset = default;
                return false;
            }

            var guid = reference.AssetGUID;

            if (_refHandles.TryGetValue(guid, out var counter) &&
                counter.Handle.IsValid() &&
                counter.Handle.Status == AsyncOperationStatus.Succeeded &&
                counter.Handle.Result != null)
            {
                counter.RefCount++;
                asset = counter.Handle.Result;
                return true;
            }

            asset = default;
            return false;
        }

        public async UniTask<T> LoadAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning($"[DefaultAssetProvider{typeof(T)}] Failed to load asset by AssetReference: key is null or empty.");
                return default;
            }

            if (_keyHandles.TryGetValue(key, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask(cancellationToken: cancellationToken);

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                Addressables.Release(handle);
                _logger.LogWarning($"[DefaultAssetProvider{typeof(T)}] Failed to load asset by key: {key}");
                return default;
            }

            var newCounter = RefCounterPool<T>.Get(handle);
            _keyHandles[key] = newCounter;

            return handle.Result;
        }

        public async UniTask<T> LoadAsync(AssetReference reference, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(reference.AssetGUID))
            {
                _logger.LogWarning(
                    $"[DefaultAssetProvider{typeof(T)}] Failed to load asset by AssetReference: AssetGUID is null or empty.");
                return default;
            }

            var guid = reference.AssetGUID;

            if (_refHandles.TryGetValue(guid, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(reference);
            await handle.ToUniTask(cancellationToken: cancellationToken);

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                Addressables.Release(handle);
                _logger.LogWarning($"[DefaultAssetProvider{typeof(T)}] Failed to load asset by AssetReference: {guid}");
                return default;
            }

            var newCounter = RefCounterPool<T>.Get(handle);
            _refHandles[guid] = newCounter;

            return handle.Result;
        }

        public void Release(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning($"[DefaultAssetProvider{typeof(T)}] Failed to release asset by key: key is null or empty.");
                return;
            }

            if (!_keyHandles.TryGetValue(key, out var counter))
            {
                _logger.LogWarning($"[DefaultAssetProvider{typeof(T)}] Failed to release asset by key: key {key} doesn't exist.");
                return;
            }

            counter.RefCount--;

            if (counter.RefCount != 0)
            {
                return;
            }

            Addressables.Release(counter.Handle);
            _keyHandles.Remove(key);
            RefCounterPool<T>.Release(counter);
        }

        public void Release(AssetReference reference)
        {
            var guid = reference.AssetGUID;
            if (string.IsNullOrEmpty(guid))
            {
                _logger.LogWarning(
                    $"[DefaultAssetProvider{typeof(T)}] Failed to release asset by AssetReference: AssetGUID is null or empty.");
                return;
            }

            if (!_refHandles.TryGetValue(guid, out var counter))
            {
                _logger.LogWarning(
                    $"[DefaultAssetProvider{typeof(T)}] Failed to release asset by AssetReference: key {guid} doesn't exist.");
                return;
            }

            counter.RefCount--;

            if (counter.RefCount != 0)
            {
                return;
            }

            Addressables.Release(counter.Handle);
            _refHandles.Remove(guid);
            RefCounterPool<T>.Release(counter);
        }

        public void ClearAll()
        {
            foreach (var counter in _keyHandles.Values)
            {
                Addressables.Release(counter.Handle);
                RefCounterPool<T>.Release(counter);
            }

            foreach (var counter in _refHandles.Values)
            {
                Addressables.Release(counter.Handle);
                RefCounterPool<T>.Release(counter);
            }

            _keyHandles.Clear();
            _refHandles.Clear();
        }
    }
}