using System;
using System.Windows;
using VIMControls.Controls;

namespace VIMControls.Contracts
{

    public interface IVIMMultiLineTextDisplay
    {
        double GetRequiredHeight(int numLines);
    }

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

        public static UIElement From(IVIMControl control)
        {
            var wrapper = control.GetUIElement() as UIElementWrapper;
            if (wrapper == null) throw new Exception("Control was not a WPF supported control type");

            return wrapper.UiElement;
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

    public interface IVIMCommandController : IVIMControl
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

    public interface IVIMListMotionController
    {
        void MoveVertically(int i);
        void NextLine();
    }

    public interface IVIMMotionController : IVIMListMotionController
    {
        void MoveHorizontally(int i);
        void EndOfLine();
        void BeginningOfLine();
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
        Type ControllerType { get; }
    }

    public interface IVIMAllControllers : IVIMContainer,
                                          IVIMPositionController,
                                          IVIMCommandController,
                                          IVIMActionController,
                                          IVIMPersistable,
                                          IListController
    {}

    public interface IVIMControlContainer : IVIMAllControllers
    {
        CommandMode Mode { get; set; }
    }
}