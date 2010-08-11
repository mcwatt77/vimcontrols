using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Navigator.Containers;
using NUnit.Framework;

namespace Navigator.Repository
{
    [TestFixture]
    public class RepositoryTest
    {
        [Test]
        public void Test()
        {
            //TODO: I'll end up doing some sort of data binding I think...
            //Where I can specify how objects are retrieved and populated for the repository,
            //then the UI elements can just get stuff from repositories
            var container = GetNewContainer();

            var personRepository = container.Get<Repository<IPerson>>();

            var newPerson = personRepository.New();
            newPerson.DateOfBirth = new DateTime(1980, 6, 4);
            newPerson.Name.FirstName = "Joe";
            newPerson.Name.LastName = "Jackson";
            personRepository.Save();

            //Doing a GetNewContainer() here flushes the cache and forces it to regrab the info from disk
            container = GetNewContainer();
            personRepository = container.Get<Repository<IPerson>>();
            newPerson = personRepository.Find(person => person.Name.LastName == "Jackson").Single();

            Assert.AreEqual(newPerson.Name.FirstName, "Joe");
        }

        private static Container GetNewContainer()
        {
            var container = new Container();
            container.Register(typeof(RepositoryFactory), typeof(RepositoryFactory), ContainerRegisterType.Singleton);
            container.RegisterGeneric<RepositoryFactory>(typeof (Repository<>),
                                                         (factory, type) =>
                                                             {
                                                                 var method = factory
                                                                     .GetType()
                                                                     .GetMethod("Get")
                                                                     .MakeGenericMethod(type.GetGenericArguments().Single());
                                                                 return () => method.Invoke(factory, new object[] {});
                                                             });
            return container;
        }
    }

    public abstract class Metadata<TType>
    {
        public abstract void Register(IMetadataRegistry<TType> metadataRegistry);
    }

    public class MetadataField
    {
        private MetadataField()
        {}

        public static MetadataField FromExpression(LambdaExpression expression)
        {
            var body = expression.Body;
            if (body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) body;
                body = unaryExpression.Operand;
            }

            var memberExpression = (MemberExpression)body;
            var propertyInfo = (PropertyInfo) memberExpression.Member;
            return new MetadataField {Property = propertyInfo};
        }

        public PropertyInfo Property { get; private set; }
        public Delegate DefaultValue { get; set; }

        public void SetValue(object @object, object value)
        {
            Property.SetValue(@object, value, null);
        }
    }

    public class MetadataIndex
    {
        public MetadataIndex(params MetadataField[] metadataFields)
        {
            Fields = metadataFields.ToArray();
        }

        public IEnumerable<MetadataField> Fields { get; private set; }
    }

    public class MetadataInfo<TType> : IMetadataRegistry<TType>
    { 
        private readonly List<MetadataIndex> _indexes = new List<MetadataIndex>();
        public IEnumerable<MetadataIndex> Indexes { get { return _indexes; } }

        public void SetupIndex(MetadataIndexType metadataIndexType, params Expression<Func<TType, object>>[] indexExpressions)
        {
            var fields = indexExpressions
                .Select(expression => MetadataField.FromExpression(expression))
                .ToArray();

            _indexes.Add(new MetadataIndex(fields));
        }

        private readonly List<MetadataField> _fields = new List<MetadataField>();
        public IEnumerable<MetadataField> Fields { get { return _fields; } }

        public void SetupColumn<TResult>(Expression<Func<TType, TResult>> fieldExpression, Action<FieldInfo<TResult>> fnSetupFieldInfo)
        {
            var fieldInfo = new FieldInfo<TResult>();
            fnSetupFieldInfo(fieldInfo);

            var field = MetadataField.FromExpression(fieldExpression);
            field.DefaultValue = fieldInfo.DefaultValue;

            _fields.Add(field);
        }
    }

    public enum MetadataIndexType
    {
        Unique, Nonunique
    }

    public interface IMetadataRegistry<TType>
    {
        void SetupIndex(MetadataIndexType metadataIndexType, params Expression<Func<TType, object>>[] indexExpressions);

        void SetupColumn<TResult>(Expression<Func<TType, TResult>> fieldExpression,
                                  Action<FieldInfo<TResult>> fnSetupFieldInfo);
    }

    //TODO: Make this an interface with an implementation that sets the underlying values of FieldInfo and adds it to a collection
    public class FieldInfo<TType>
    {
        public Func<TType> DefaultValue { get; set; }
    }

    public class PersonMetadata : Metadata<IPerson>
    {
        private readonly Repository<IPersonName> _personNameRepository;

        public PersonMetadata(Repository<IPersonName> personNameRepository)
        {
            _personNameRepository = personNameRepository;
        }

        public override void Register(IMetadataRegistry<IPerson> metadataRegistry)
        {
            metadataRegistry.SetupIndex(MetadataIndexType.Unique, person => person.Name, person => person.DateOfBirth, person => person.Location);

            metadataRegistry.SetupColumn(person => person.Name, field => field.DefaultValue = () => _personNameRepository.New());
        }
    }

    public interface IPerson
    {
        IPersonName Name { get; set; }
        ILocation Location { get; set; }
        DateTime DateOfBirth { get; set; }
        DateTime DateOfDeath { get; set; }
        ICollection<IImage> Images { get; }
        ICollection<ILink> Links { get; }
    }

    public interface ILocation
    {
        string Name { get; set; }
    }

    public interface ILink
    {}

    public interface IImage
    {}

    public interface IPersonName
    {
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
    }
}