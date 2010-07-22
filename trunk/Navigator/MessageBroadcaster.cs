using VIControls.Commands.Interfaces;

namespace Navigator
{
    public class MessageBroadcaster
    {
        public void Update()
        {
            UpdateElement.Update();
        }

        public IUIElement UpdateElement { get; set; }
    }
}