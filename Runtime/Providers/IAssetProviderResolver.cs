namespace Core.AddressablesModule
{
    public interface IAssetProviderResolver
    {
        IAssetProviderWithType<T> Get<T>();
        GameObjectInstantiatingAssetProvider GetSpecificProvider();
    }
}