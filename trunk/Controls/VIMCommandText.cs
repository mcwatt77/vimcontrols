﻿using VIMControls.Contracts;
using VIMControls.Controls;

namespace VIMControls.Controls
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
            var mapper = ServiceLocator.FindService<IVIMStringCommandMapper>()();
            var msg = mapper.MapCommand(TextLine.Substring(1));
            var msgSvc = ServiceLocator.FindService<IVIMMessageService>()();
            msgSvc.SendMessage(msg);
        }

        public void CommandBackspace()
        {
            TextLine = TextLine.Remove(TextLine.Length - 1);
        }
    }
}