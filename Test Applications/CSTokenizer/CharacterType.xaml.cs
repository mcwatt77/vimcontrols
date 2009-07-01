namespace CSTokenizer
{
    public class CharacterType
    {
        private readonly string _display;

        public CharacterType(string display)
        {
            _display = display;
        }

        public override string ToString()
        {
            return _display;
        }
    }
}