using ActionDictionary;

namespace DataProcessors
{
    public class MessagePipe
    {
        private readonly object _mainMessageRecipient;

        public MessagePipe(object mainMessageRecipient)
        {
            _mainMessageRecipient = mainMessageRecipient;
        }

        public void SendMessage(Message msg)
        {
            msg.Invoke(_mainMessageRecipient);
        }
    }
}
