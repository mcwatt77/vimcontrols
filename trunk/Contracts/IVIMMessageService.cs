using System;

namespace VIMControls.Contracts
{
    public interface IVIMMessageService
    {
        void SendMessage(IVIMAction msg);
        void SendMessage(IVIMAction msg, params object[] @params);
    }

    public class VIMMessageService : IVIMMessageService
    {
        private readonly IVIMController _controller;

        public static void SendMessage<TContract>(Action<TContract> fn)
        {
            var action = new VIMAction(fn);
            var msgSvc = Services.Locate<IVIMMessageService>()();
            msgSvc.SendMessage(action);
        }

        public VIMMessageService(IVIMController controller)
        {
            _controller = controller;
        }

        public void SendMessage(IVIMAction msg)
        {
            msg.Invoke(_controller);
        }

        public void SendMessage(IVIMAction msg, params object[] @params)
        {
            msg.Invoke(_controller);
        }
    }
}