using Navigator.Containers;

namespace Navigator.Repository
{
    public class RepositoryFactory
    {
        private readonly IContainer _container;
        private readonly IdentityLookup _identityLookup;

        public RepositoryFactory(IContainer container, IdentityLookup identityLookup)
        {
            _container = container;
            _identityLookup = identityLookup;
        }

        public Repository<T> Get<T>()
        {
            return new Repository<T>(_container, _identityLookup);
        }
    }
}