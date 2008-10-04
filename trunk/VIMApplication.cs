using System.Linq;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using VIMControls.Interfaces.Input;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls
{
    public class VIMApplication : IApplication
    {
        private IFactory<IView> _viewFactory;
        private IKeyCommandGenerator _keyGen;

        public void Initialize(IContainer container)
        {
            _keyGen = container.Get<IKeyCommandGenerator>();
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

        public void SetView<TView>(TView item)
        {
            CurrentView = _viewFactory.Create(item);
        }

        public IKeyCommandGenerator KeyGen
        {
            get { return _keyGen; }
        }

        public void SetMode(KeyInputMode mode)
        {
            //todo: there are two different KeyMode concepts going on...
            //one for the keymode as it changes to generate the interface calls...
            //and another that is important to provide UI queues.
            //but I still need to be able to generate interface calls ahead of time without
            //changing *the* mode
            _keyGen.SetMode(mode);
        }

        public void SetToggle(KeyToggle toggle)
        {
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
