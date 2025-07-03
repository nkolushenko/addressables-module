using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.AddressablesModule.Services
{
    public interface ISpriteAtlasService
    {
        bool TryGetSprite(string atlasName, string spriteName, out Sprite sprite);
        UniTask<Sprite> GetSprite(string atlasName, string spriteName, CancellationToken cancellationToken);
        void ReleaseAtlas(string atlasName);
        void ReleaseAll();
    }
}