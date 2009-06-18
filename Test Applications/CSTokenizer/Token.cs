namespace CSTokenizer
{
    public class Token
    {
        public CharacterType CharacterType { get; private set; }
        public string Characters { get; private set; }

        public Token(CharacterType characterType, string characters)
        {
            CharacterType = characterType;
            Characters = characters;
        }

        public override string ToString()
        {
            return CharacterType + ": " + Characters;
        }
    }
}