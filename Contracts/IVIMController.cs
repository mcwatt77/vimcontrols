using System.Windows;

namespace VIMControls.Controls
{
    public interface IVIMControl
    {
        IUIElement GetUIElement();
    }

    public interface IUIElement
    {
    }

    public class UIElementWrapper : IUIElement
    {
        public UIElement UiElement { get; set; }

        public UIElementWrapper(UIElement uiElement)
        {
            UiElement = uiElement;
        }
    }

    public interface IVIMNavigable<T>
    {
        void Navigate(T obj);
    }

    public interface IVIMController
    {
        void ResetInput();
        void MissingModeAction(IVIMAction action);
        void MissingMapping();
    }

    public interface IVIMCommandController
    {
        void EnterCommandMode();
        void InfoCharacter(char c);
        void CommandCharacter(char c);
        void Execute();
        void CommandBackspace();
    }

    public interface IVIMContainer : IVIMController, IVIMNavigable<string>, IVIMNavigable<object>
    {
        void StatusLine(string status);
    }

    public interface IVIMMotionController
    {
        void MoveVertically(int i);
        void MoveHorizontally(int i);
        void EndOfLine();
        void BeginningOfLine();
        void NextLine();
    }

    public interface IVIMCharacterController
    {
        void Output(char c);
        void NewLine();
        void Backspace();
    }

    public interface IVIMActionController
    {
        void DeleteAtCursor();
        void EnterInsertMode(CharacterInsertMode mode);
        void EnterCommandMode();
        void EnterNormalMode();
        void InsertLine(LineInsertMode mode);
    }

    public enum CharacterInsertMode
    {
        Before, After
    }

    public enum LineInsertMode
    {
        Above, Below
    }

    public interface IVIMPositionController
    {
        void Move(GridLength horz, GridLength vert);
        void TogglePositionIndicator();
    }

    public interface IVIMPersistable
    {
        void Save();
        void Delete();
    }

    public interface IVIMSystemUICommands : IVIMController
    {
        void Maximize();
        void Save();
        void About();
    }

    public interface IVIMAction
    {
        void Invoke(IVIMController controller);
    }

    public interface IVIMAllControllers : IVIMContainer,
                                          IVIMCharacterController,
                                          IVIMMotionController,
                                          IVIMPositionController,
                                          IVIMCommandController,
                                          IVIMActionController,
                                          IVIMPersistable
    {}

    public interface IVIMControlContainer : IVIMAllControllers
    {
        CommandMode Mode { get; set; }
    }
}