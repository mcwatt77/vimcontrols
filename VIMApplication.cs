using System.Linq;
using System.Windows.Input;
using VIMControls.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls
{
    public class CreateBrowseElement : ICreateBrowseElement
    {
        public string DisplayName { get; set;}
        public IEditor Create { get; set; }
        public IBrowser Browse { get; set; }
    }

    public interface IContainer
    {
        T Get<T>(params object[] @params);
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

    public class VIMApplication : IApplication
    {
        private KeyCommandGenerator _keyGen;
        private IFactory<IView> _viewFactory;

        public void Initialize(IContainer container)
        {
            _keyGen = new KeyCommandGenerator();
            var elementFactory = container.Get<IFactory<IBrowseElement>>();
            var list = container.Get<ILinkedList<IBrowseElement>>();
            var stackNode = list.AddFirst(elementFactory.Create("Command Stack"));
            var names = new [] {"Notes", "Objects", "Movies", "Audio", "Files"};
            names
                .Reverse()
                .Select(s => elementFactory.Create(s))
                .Do(elem => list.AddBefore(stackNode, elem));

            CurrentView = container.Get<IBrowser>(list);

            _viewFactory = container.Get<IFactory<IView>>();
        }

        public IView CurrentView { get; set;}

        public void ProcessKey(Key key)
        {
            _keyGen.ProcessKey(key).Do(command => command.Invoke(this));
        }

        public void ProcessKeyString(string keyString)
        {
            _keyGen.ProcessKeyString(keyString).Do(command => command.Invoke(this));
        }

        public void SetView<TView>(TView item)
        {
            CurrentView = _viewFactory.Create(item);
        }

        public void SetMode(KeyInputMode mode)
        {
            _keyGen.SetMode(mode);
        }

        public TView FindView<TView>()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessCommand(ICommand command)
        {
        }

        public void ProcessMissingCommand(ICommand command)
        {
            command.Invoke(CurrentView);
        }
    }
}
