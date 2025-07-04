using System.Collections.Generic;
using System.Threading;
using Core.AddressablesModule.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressablesModule
{
    public class GameObjectInstantiatingAssetProvider : IInstantiatingAssetProvider
    {
        private readonly ILogWrapper _logger;
        
        private readonly Dictionary<GameObject, RefCounter<GameObject>> _manualHandles = new();
        private readonly Dictionary<string, List<GameObject>> _instantiated = new();
        private readonly Dictionary<GameObject, string> _instanceToKey = new();

        public GameObjectInstantiatingAssetProvider(ILogWrapper logger)
        {
            _logger = logger;
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool
            trackHandle = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning($"[GameObjectAssetProvider] Failed to load asset by key: key is null or empty.");
                return default;
            }

            var handle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
            var go = await handle.WithCancellation(cancellationToken);

            if (go == null)
            {
                return null;
            }

            if (!trackHandle)
            {
                var counter = RefCounterPool<GameObject>.Get(handle);
                _manualHandles[go] = counter;
            }

            if (!_instantiated.TryGetValue(key, out var list))
            {
                list = ListPool<GameObject>.Get();

                _instantiated[key] = list;
            }

            list.Add(go);

            _instanceToKey[go] = key;

            return go;
        }

        public async UniTask<GameObject> InstantiateAsync(AssetReference reference, Transform parent = null,
            bool instantiateInWorldSpace = false, bool
                trackHandle = false, CancellationToken cancellationToken = default)
        {
            var key = reference.AssetGUID;
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning($"[GameObjectAssetProvider] Failed to load asset by AssetReference: key is null or empty.");
                return default;
            }


            var handle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
            var go = await handle.WithCancellation(cancellationToken);

            if (go == null)
            {
                return null;
            }

            if (!trackHandle)
            {
                var counter = RefCounterPool<GameObject>.Get(handle);
                _manualHandles[go] = counter;
            }

            if (!_instantiated.TryGetValue(key, out var list))
            {
                list = ListPool<GameObject>.Get();
                _instantiated[key] = list;
            }

            list.Add(go);
            _instanceToKey[go] = key;

            return go;
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null || !_instanceToKey.Remove(instance, out var key))
            {
                return;
            }

            if (_instantiated.TryGetValue(key, out var list))
            {
                int index = list.IndexOf(instance);
                if (index >= 0)
                {
                    int last = list.Count - 1;
                    list[index] = list[last];
                    list.RemoveAt(last);
                }

                if (list.Count == 0)
                {
                    _instantiated.Remove(key);
                    ListPool<GameObject>.Release(list);
                }
            }

            if (_manualHandles.TryGetValue(instance, out var counter))
            {
                counter.DecrementRef(); 

                if (counter.RefCount <= 0)
                {
                    Addressables.Release(counter.Handle);
                    RefCounterPool<GameObject>.Release(counter);
                    _manualHandles.Remove(instance);
                }
            }
            else
            {
                Addressables.ReleaseInstance(instance);
            }
        }

        public void ClearAll()
        {
            foreach (var info in _instantiated.Values)
            {
                for (int j = 0; j < info.Count; j++)
                {
                    var go = info[j];

                    if (go != null)
                    {
                        Addressables.ReleaseInstance(go);
                    }
                }

                ListPool<GameObject>.Release(info);
            }

            _instantiated.Clear();
            _instanceToKey.Clear();
        }
    }
}