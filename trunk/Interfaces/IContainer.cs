using System.Linq;
using VIMControls.Interfaces;

namespace VIMControls
{
    public interface IContainer
    {
        T Get<T>(params object[] @params);
    }
    public class CreateBrowseElement : ICreateBrowseElement
    {
        public string DisplayName { get; set;}
        public IEditor Create { get; set; }
        public IBrowser Browse { get; set; }
    }

    public interface IFactory<T>
    {
        T Create(params object[] @params);
    }

    public class Factory<T> : IFactory<T>
    {
        public T Create(params object[] @params)
        {
            var type = typeof (T).GetImplementations().First();
            return (T)type.GetConstructors().First().Invoke(@params);
        }
    }
}