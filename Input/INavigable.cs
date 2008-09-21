using VIMControls.Interfaces.Framework;

namespace VIMControls.Input
{
    public interface INavigable
    {
        [KeyMapNormal("<cr>")]
        void Navigate();
    }
}