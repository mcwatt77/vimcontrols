using System.Windows.Input;

namespace AcceptanceTests.Interfaces
{
    public interface IApplication
    {
        void Initialize();
        IView CurrentView { get; set; }
        void ProcessKey(Key key);
        void ProcessKeyString(string keyString);
        InputMode Mode { get; }
        TView FindView<TView>();
    }
}