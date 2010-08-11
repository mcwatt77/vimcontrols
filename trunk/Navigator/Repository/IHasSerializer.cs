using System.IO;

namespace Navigator.Repository
{
    public interface IHasSerializer<TType>
    {
        void Serialize(Stream stream);
        TType Deserialize(Stream stream);
    }
}