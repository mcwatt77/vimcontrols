using VIMControls.Controls;

namespace VIMControls
{
    public class VIMCommandText : VIMTextControl, IVIMCommandController
    {
        public void EnterCommandMode()
        {
            _textData[0].Text = ":";
        }

        public void InfoCharacter(char c)
        {
            _textData[0].Text = c.ToString();
        }

        public void CommandCharacter(char c)
        {
            _textData[0].Text += c;
        }

        public void Execute()
        {
            var result = ServiceLocator.FindService<ServiceResult>(_textData[0].Text.Substring(1))();
        }
    }
}
