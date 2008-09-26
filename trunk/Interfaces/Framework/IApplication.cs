namespace VIMControls.Interfaces.Framework
{
    public interface IApplication : ICommandable
    {
        void Initialize(IContainer container);
        IView CurrentView { get; set; }
        TView FindView<TView>();
        void ProcessCommand(ICommand command);
        void SetView<TView>(TView item);
        IKeyCommandGenerator KeyGen { get; }
        [KeyMapSearch("<cr>", KeyInputMode.Normal)]
        void SetMode(KeyInputMode mode);
    }
}