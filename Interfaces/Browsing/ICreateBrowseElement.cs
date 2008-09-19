using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface ICreateBrowseElement : IBrowseElement
    {
        IEditor Create { get; }
        IBrowser Browse { get; }
    }
}