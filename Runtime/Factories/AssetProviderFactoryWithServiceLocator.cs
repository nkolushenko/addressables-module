namespace Core.AddressablesModule
{
    public class AssetProviderFactoryWithServiceLocator : IAssetProviderFactory
    {
        private readonly ILogWrapper _logWrapper;

        public AssetProviderFactoryWithServiceLocator(ILogWrapper logWrapper)
        {
            _logWrapper = logWrapper;
        }

        public IAssetLoaderWithType<T> Create<T>()
        {
            return new DefaultAssetLoaderWithType<T>(_logWrapper);
        }
    }
}