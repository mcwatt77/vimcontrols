using VIMControls.Controls;

namespace VIMControls.Contracts
{
    public interface IVIMMessageService
    {
        void SendMessage(IVIMAction msg);
    }

    public class VIMMessageService : IVIMMessageService
    {
        private readonly IVIMControlContainer _container;

        public VIMMessageService(IVIMControlContainer container)
        {
            _container = container;
        }

        public void SendMessage(IVIMAction msg)
        {
            msg.Invoke(_container);
        }
    }
}