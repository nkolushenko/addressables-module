using System;
using System.Collections.Generic;
using System.Threading;
using Core.AddressablesModule.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Core.AddressablesModule.Services
{
    public class SpriteAtlasService : ISpriteAtlasService
    {
        private readonly IAddressablesService _addressablesService;
        private readonly ILogWrapper _logWrapper;

        private readonly Dictionary<string, AtlasEntry> _atlases = new(10);
        private readonly Dictionary<string, UniTaskCompletionSource<SpriteAtlas>> _pendingLoads = new();

        public SpriteAtlasService(IAddressablesService addressablesService, ILogWrapper logWrapper)
        {
            _addressablesService = addressablesService;
            _logWrapper = logWrapper;
        }

        public bool TryGetSprite(string atlasName, string spriteName, out Sprite sprite)
        {
            sprite = null;

            if (string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
            {
                return false;
            }

            return _atlases.TryGetValue(atlasName, out var entry) && entry.TryGetSprite(spriteName, out sprite);
        }

        public async UniTask<Sprite> GetSprite(string atlasName, string spriteName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            var entry = await GetOrLoadAtlas(atlasName, cancellationToken);

            return entry.TryGetSprite(spriteName, out var sprite) ? sprite : null;
        }

        private async UniTask<AtlasEntry> GetOrLoadAtlas(string atlasName, CancellationToken cancellationToken)
        {
            if (_atlases.TryGetValue(atlasName, out var existing))
            {
                existing.IncrementRef();
                return existing;
            }

            if (_pendingLoads.TryGetValue(atlasName, out var pending))
            {
                var atlas = await pending.Task.AttachExternalCancellation(cancellationToken);
                return FinalizeEntry(atlasName, atlas);
            }

            var tcs = new UniTaskCompletionSource<SpriteAtlas>();
            _pendingLoads[atlasName] = tcs;

            try
            {
                var atlas = await _addressablesService.LoadAsync<SpriteAtlas>(atlasName, cancellationToken);
                tcs.TrySetResult(atlas);
                return FinalizeEntry(atlasName, atlas);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
                _pendingLoads.Remove(atlasName);
                _logWrapper.LogError($"[SpriteAtlasService] Failed to load atlas '{atlasName}': {e}");
                throw;
            }
            finally
            {
                _pendingLoads.Remove(atlasName);
            }
        }

        private AtlasEntry FinalizeEntry(string atlasName, SpriteAtlas atlas)
        {
            var entry = AtlasEntryPool.Get(atlas);
            _atlases[atlasName] = entry;
            return entry;
        }

        public void ReleaseAtlas(string atlasName)
        {
            if (!_atlases.TryGetValue(atlasName, out var entry))
            {
                return;
            }

            entry.DecrementRef();

            if (entry.RefCount > 0)
            {
                return;
            }

            if (entry.Atlas != null)
            {
                _addressablesService.Release<SpriteAtlas>(atlasName);
            }

            _atlases.Remove(atlasName);
            AtlasEntryPool.Release(entry);
        }

        public void ReleaseAll()
        {
            foreach (var (atlasName, entry) in _atlases)
            {
                if (entry.Atlas != null)
                {
                    _addressablesService.Release<SpriteAtlas>(atlasName);
                }

                AtlasEntryPool.Release(entry);
            }

            _atlases.Clear();
            _pendingLoads.Clear();
        }
    }
}