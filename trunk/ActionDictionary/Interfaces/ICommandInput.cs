namespace ActionDictionary.Interfaces
{
    public interface ICommandInput
    {
        void CommandCharacter(char c);
        void CommandBackspace();
        void CommandEnter();
    }
}
