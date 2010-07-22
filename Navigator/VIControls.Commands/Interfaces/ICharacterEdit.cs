namespace VIControls.Commands.Interfaces
{
    public interface ICharacterEdit
    {
        void Output(char c);
        void NewLine();
        void Backspace();
        void SetCursor(int row, int column);
    }
}