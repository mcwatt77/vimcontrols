using System.Windows.Input;

namespace VIMControls.Interfaces.Framework
{
    public interface IApplication : ICommandable
    {
        void Initialize(IContainer container);
        IView CurrentView { get; set; }
        KeyInputMode Mode { get; }
        [KeyMap(KeyInputMode.Normal, "/", KeyInputMode.Search)]
        void SetMode(KeyInputMode mode);
        TView FindView<TView>();
        void ProcessCommand(ICommand command);
        void ProcessKey(Key key);
        void ProcessKeyString(string keyString);
    }
}