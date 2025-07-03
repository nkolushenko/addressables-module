namespace Core.AddressablesModule
{
    public interface IAssetProviderFactory
    {
        IAssetLoaderWithType<T> Create<T>();
    }
}