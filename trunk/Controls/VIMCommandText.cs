using VIMControls.Controls;

namespace VIMControls
{
    public class VIMCommandText : VIMTextControl, IVIMCommandController
    {
        private string TextLine
        {
            get
            {
                return _textData[0].Text;
            }
            set
            {
                _textData[0].Text = value;
            }
        }
        public void EnterCommandMode()
        {
            TextLine = ":";
        }

        public void InfoCharacter(char c)
        {
            TextLine = c.ToString();
        }

        public void CommandCharacter(char c)
        {
            TextLine += c;
        }

        public void Execute()
        {
//            var result = ServiceLocator.FindService<ServiceResult>(TextLine.Substring(1))();
        }

        public void CommandBackspace()
        {
            TextLine = TextLine.Remove(TextLine.Length - 1);
        }
    }
}
