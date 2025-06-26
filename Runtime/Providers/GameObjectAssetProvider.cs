using System.Collections.Generic;
using Core.AddressablesModule.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AddressablesModule
{
    public class GameObjectAssetProvider : IInstantiatingAssetProvider<GameObject>
    {
        private readonly Dictionary<string, RefCounter<GameObject>> _prefabHandlesByKey = new();
        private readonly Dictionary<AssetReference, RefCounter<GameObject>> _prefabHandlesByRef = new();

        private readonly Dictionary<string, List<GameObject>> _instancesByKey = new();
        private readonly Dictionary<AssetReference, List<GameObject>> _instancesByRef = new();
//unsafe code
        public async UniTask<GameObject> LoadAsync(string key)
        {
            if (_prefabHandlesByKey.TryGetValue(key, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return null;
            }

            var newCounter = RefCounterPool<GameObject>.Get(handle);
            _prefabHandlesByKey[key] = newCounter;

            return handle.Result;
        }
//unsafe code
        public async UniTask<GameObject> LoadAsync(AssetReference reference)
        {
            if (_prefabHandlesByRef.TryGetValue(reference, out var counter))
            {
                counter.RefCount++;
                return counter.Handle.Result;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(reference);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return null;
            }

            var newCounter = RefCounterPool<GameObject>.Get(handle);
            _prefabHandlesByRef[reference] = newCounter;

            return handle.Result;
        }

        public async UniTask<GameObject> InstantiateAsync(string key)
        {
            var handle = Addressables.InstantiateAsync(key);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return null;
            }

            if (!_instancesByKey.TryGetValue(key, out var list))
            {
                list = ListPool<GameObject>.Get();
                _instancesByKey[key] = list;
            }

            list.Add(handle.Result);
            return handle.Result;
        }

        public async UniTask<GameObject> InstantiateAsync(AssetReference reference)
        {
            var handle = Addressables.InstantiateAsync(reference);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                return null;
            }

            if (!_instancesByRef.TryGetValue(reference, out var list))
            {
                list = ListPool<GameObject>.Get();
                _instancesByRef[reference] = list;
            }

            list.Add(handle.Result);
            return handle.Result;
        }
//Need optimization
        public void Release(string key)
        {
            if (_prefabHandlesByKey.TryGetValue(key, out var counter))
            {
                counter.RefCount--;
                if (counter.RefCount <= 0)
                {
                    Addressables.Release(counter.Handle);
                    _prefabHandlesByKey.Remove(key);
                    RefCounterPool<GameObject>.Release(counter);
                }
            }

            if (_instancesByKey.Remove(key, out var list))
            {
                foreach (var go in list)
                {
                    if (go != null)
                    {
                        Addressables.ReleaseInstance(go);
                    }
                }

                ListPool<GameObject>.Release(list);
            }
        }
//Need optimization
        public void Release(AssetReference reference)
        {
            if (_prefabHandlesByRef.TryGetValue(reference, out var counter))
            {
                counter.RefCount--;
                if (counter.RefCount <= 0)
                {
                    Addressables.Release(counter.Handle);
                    _prefabHandlesByRef.Remove(reference);
                    RefCounterPool<GameObject>.Release(counter);
                }
            }

            if (_instancesByRef.Remove(reference, out var list))
            {
                foreach (var go in list)
                {
                    if (go != null)
                    {
                        Addressables.ReleaseInstance(go);
                    }
                }

                ListPool<GameObject>.Release(list);
            }
        }
//Need optimization
        public void ReleaseInstance(GameObject instance)
        {
            if (instance != null)
            {
                Addressables.ReleaseInstance(instance);
            }
        }
    }
}