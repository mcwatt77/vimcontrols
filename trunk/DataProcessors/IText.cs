using ActionDictionary.Interfaces;

namespace DataProcessors
{
    public interface IText : ITextInput
    {
        string GetText();
        void InsertText(int index);
        void RemoveText(int index, int count);
        string GetLinesInBox(int top, int left, int height, int width);
    }
}