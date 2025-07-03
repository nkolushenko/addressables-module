using System.Collections.Generic;
using Core.AddressablesModule.Services;
using UnityEngine.U2D;

namespace Core.AddressablesModule.Pool
{
    public class AtlasEntryPool
    {
        private static readonly Stack<AtlasEntry> Pool = new(16);

        public static AtlasEntry Get(SpriteAtlas atlas)
        {
            if (Pool.Count <= 0)
            {
                var entry = new AtlasEntry();
                entry.Rent(atlas);
                return entry;
            }

            var item = Pool.Pop();
            item.Rent(atlas);
            return item;
        }

        public static void Release(AtlasEntry atlasEntry)
        {
            atlasEntry.Release();
            Pool.Push(atlasEntry);
        }
    }
}