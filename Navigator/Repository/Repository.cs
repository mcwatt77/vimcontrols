using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Navigator.Containers;

namespace Navigator.Repository
{
    public class Repository<TType>
    {
        private readonly IContainer _container;
        private readonly IdentityLookup _identityLookup;

        private readonly List<TType> _objectsToSave = new List<TType>();

        public Repository(IContainer container, IdentityLookup identityLookup)
        {
            _container = container;
            _identityLookup = identityLookup;

            var implementor = FindImplementor() ?? BuildImplementor();

            _container.Register(typeof(TType), implementor, ContainerRegisterType.Instance);
        }

        private static Type FindImplementor()
        {
            if (!typeof(TType).IsAbstract && !typeof(TType).IsInterface)
                return typeof (TType);

            var singleImplementor = typeof (Repository<TType>)
                .Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .SingleOrDefault(type => typeof(TType).IsAssignableFrom(type));

            return singleImplementor;
        }

        public TType New()
        {
            var @new = _container.Get<TType>();
            _identityLookup.GetNewId(@new);
            _objectsToSave.Add(@new);

            var metadataType = typeof (Repository<>)
                .Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .SingleOrDefault(type => typeof (Metadata<TType>).IsAssignableFrom(type));

            if (metadataType == null) return @new;

            var metadata = (Metadata<TType>) _container.Get(metadataType);

            var metadataRegistry = _container.Get<MetadataInfo<TType>>();

            metadata.Register(metadataRegistry);

            foreach (var field in metadataRegistry.Fields.Where(field => field.DefaultValue != null))
                field.SetValue(@new, field.DefaultValue.DynamicInvoke());

            return @new;
        }

        public void Save()
        {
            var repositorySerializer = new RepositorySerializer(_identityLookup);
            repositorySerializer.SerializeObjects(_objectsToSave.Cast<object>());
        }

        public IEnumerable<TType> Find(Expression<Predicate<TType>> filter)
        {
            var repositorySerializer = new RepositorySerializer(_identityLookup);
            _objectsToSave.AddRange(repositorySerializer.DeserializeByType<TType>());

            return _objectsToSave.Where(o => filter.Compile()(o));
        }

        private static Type BuildImplementor()
        {
            return ImplementationBuilder.Implement(typeof(TType));
        }
    }
}