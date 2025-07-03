using System.Collections.Generic;

namespace Core.AddressablesModule
{
    public interface IAssetProviderResolver
    {
        IAssetLoaderWithType<T> Get<T>();
        IReadOnlyCollection<IAssetLoader> GetAllLoaders();
    }
}