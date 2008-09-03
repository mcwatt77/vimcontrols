using System;
using System.Collections.Generic;
using System.Windows;
using VIMControls.Controls;

namespace VIMControls.Contracts
{
    public interface ITextInputProvider : IVIMCharacterController, IVIMMotionController
    {
        string Text { get; set; }
    }

    public interface IVIMMultiLineTextDisplay
    {
        double GetRequiredHeight(int numLines);
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
        void InvalidCommand(string cmd);
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

    public interface IVIMCharacterController : IVIMController, IVIMControl
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
        [NumberMapper(NumberMapperType.KeyPad, typeof(PositionMapper))]
        void Move(GridLength horz, GridLength vert);
        void TogglePositionIndicator();
    }

    public class PositionMapper : INumberMapper
    {
        public Func<int, object[]> Map()
        {
            return i => new object[]{new GridLength(i * 0.1, GridUnitType.Star), default(GridLength)};
        }
    }

    public interface IVIMPersistable
    {
        void Save();
        void Delete();
    }

    public interface IVIMFormConstraint
    {
        bool Multiline { get; }
    }

    public interface IVIMForm : IVIMCharacterController, IVIMPersistable, IVIMNavigable<List<KeyValuePair<string, string>>>
    {
        void SetMode(IVIMFormConstraint constraint);
    }

    public interface IVIMSystemUICommands : IVIMController
    {
        void Maximize();
        void Save();
        void About();
        void UpdateTitle();
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