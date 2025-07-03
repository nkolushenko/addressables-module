using Core.AddressablesModule;

namespace addressables_module.Runtime.Infrastructure
{
// TODO: Replace with DI container and remove this
    public class ServiceLocator
    {
        private static ServiceLocator _instance;

        private static readonly object s_lock = new();

        private ServiceLocator()
        {
        }

        public IAddressablesService AddressablesService { get; private set; }
        public ISpriteAtlasService SpriteAtlasService { get; private set; }

        public static ServiceLocator Instance
        {
            get
            {
                lock (s_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServiceLocator();
                        _instance.BuildDependencies();
                    }
                }

                return _instance;
            }
        }

        private void BuildDependencies()
        {
            ILogWrapper logWrapper = new UnityLogWrapper();

            GameObjectInstantiatingAssetProvider gameObjectsAssetProvider = new GameObjectInstantiatingAssetProvider(logWrapper);
            IAssetProviderFactory assetProviderFactory = new AssetProviderFactoryWithServiceLocator(logWrapper);
            
            var addressablesResolver = new AssetProviderResolver(assetProviderFactory);

            AddressablesService = new AddressablesService(addressablesResolver, gameObjectsAssetProvider);
            SpriteAtlasService = new SpriteAtlasService(AddressablesService, logWrapper);
        }
    }
}