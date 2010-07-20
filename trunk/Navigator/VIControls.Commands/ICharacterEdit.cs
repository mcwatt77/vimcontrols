namespace VIControls.Commands
{
    public interface ICharacterEdit
    {
        void Output(char c);
        void NewLine();
        void Backspace();
        void SetCursor(int row, int column);
    }
}
