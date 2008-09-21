using System.Windows.Input;

namespace VIMControls.Interfaces.Framework
{
    public interface IApplication : ICommandable
    {
        void Initialize(IContainer container);
        IView CurrentView { get; set; }
        [KeyMapSearch("<cr>", KeyInputMode.Normal)]
        void SetMode(KeyInputMode mode);
        TView FindView<TView>();
        void ProcessCommand(ICommand command);
        void ProcessKey(Key key);
        void ProcessKeyString(string keyString);
        void SetView<TView>(TView item);
    }
}