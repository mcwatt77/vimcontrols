using System;
using System.Reflection;
using VIMControls.Contracts;

namespace VIMControls
{
    public class VIMAction : IVIMAction
    {
        private readonly object _vimAction;
        private readonly MethodInfo _method;

        public Type ControllerType { get; private set; }

        internal VIMAction() : this(null)
        {
        }

        internal VIMAction(object vimAction)
        {
            _vimAction = vimAction;
            if (_vimAction != null)
            {
                var typeOfAction = _vimAction.GetType();
                var typesInActionParameters = typeOfAction.GetGenericArguments();
                ControllerType = typesInActionParameters[0];
            }
            else
            {
                ControllerType = typeof (IVIMController);
                _vimAction = (Action<IVIMController>) (c => c.MissingMapping());
            }
            _method = _vimAction.GetType().GetMethod("Invoke");
        }

        public void Invoke(IVIMController controller)
        {
            if (ControllerType.IsAssignableFrom(controller.GetType()))
                _method.Invoke(_vimAction, new object[] {controller});
            else
                controller.MissingModeAction(this);
        }
    }
}