namespace VIMControls.Contracts
{
    public interface IVIMMessageService
    {
        void SendMessage(IVIMAction msg);
        void SendMessage(IVIMAction msg, params object[] @params);
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

        public void SendMessage(IVIMAction msg, params object[] @params)
        {
            msg.Invoke(_container);
        }
    }
}