using VIMControls.Interfaces.Framework;

namespace VIMControls.Input
{
    public interface IMovable
    {
        [KeyMapNormal("l", 1)]
        void MoveRight(int i);
    }
}