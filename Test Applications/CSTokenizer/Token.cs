using System.Collections.Generic;

namespace CSTokenizer
{
    public class Token
    {
        public CharacterType CharacterType { get; private set; }
        public string Characters { get; set; }

        public List<Token> Children = new List<Token>();

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