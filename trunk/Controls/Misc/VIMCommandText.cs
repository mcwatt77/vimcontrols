using VIMControls.Contracts;
using VIMControls.Controls;

namespace VIMControls.Controls.Misc
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
            var cmd = TextLine.Substring(1);
            TextLine = "";
            var mapper = Services.Locate<IVIMStringCommandMapper>()();
            var msg = mapper.MapCommand(cmd.Split(' ')[0]);
            var msgSvc = Services.Locate<IVIMMessageService>()();
            msgSvc.SendMessage(msg);
        }

        public void CommandBackspace()
        {
            TextLine = TextLine.Remove(TextLine.Length - 1);
        }
    }
}