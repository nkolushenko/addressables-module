using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Core.AddressablesModule.Services
{
    public class AtlasEntry
    {
        private readonly Dictionary<string, Sprite> _spriteCache = new();

        public SpriteAtlas Atlas { get; private set; }
        public int RefCount { get; private set; }

        public void IncrementRef()
        {
            RefCount++;
        }

        public void DecrementRef()
        {
            RefCount--;
        }

        public void Rent(SpriteAtlas atlas)
        {
            Atlas = atlas;
            IncrementRef();
        }

        public void Release()
        {
            Atlas = null;
            RefCount = 0;
            _spriteCache.Clear();
        }

        public bool TryGetSprite(string spriteName, out Sprite sprite)
        {
            if (Atlas is null)
            {
                sprite = null;
                return false;
            }

            if (_spriteCache.TryGetValue(spriteName, out sprite))
            {
                return true;
            }

            sprite = Atlas.GetSprite(spriteName);

            if (sprite == null)
            {
                return false;
            }

            _spriteCache[spriteName] = sprite;
            return true;
        }
    }
}