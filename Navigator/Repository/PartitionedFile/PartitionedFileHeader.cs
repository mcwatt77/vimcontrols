using System;
using System.IO;

namespace Navigator.Repository.PartitionedFile
{
    public class PartitionedFileHeader
    {
        private readonly long _offsetOfHeader;
        private long? _offsetOfNextHeader;
        public long? OffsetOfNextHeader
        {
            get { return _offsetOfNextHeader; }
            set
            {
                if (value < 0) throw new ArgumentException("OffsetOfNextHeader must be positive");
                _offsetOfNextHeader = value;
            }
        }

        private long? _offsetOfPrevHeader;
        public long? OffsetOfPrevHeader
        {
            get { return _offsetOfPrevHeader; }
            set
            {
                if (value < 0) throw new ArgumentException("OffsetOfPrevHeader must be positive");
                _offsetOfPrevHeader = value;
            }
        }

        public int LengthOfCurrentBuffer { get; set; }

        public PartitionedFileHeader(long offsetOfHeader)
        {
            _offsetOfHeader = offsetOfHeader;
        }

        public long OffsetOfHeader { get { return _offsetOfHeader; } }

        public static int DataOffset
        {
            get
            {
                return sizeof (long) + sizeof(long) + sizeof (int);
            }
        }

        public void Write(Stream stream)
        {
            Serializer.Serialize(stream, OffsetOfNextHeader  == null ? -1 : OffsetOfNextHeader.Value);
            Serializer.Serialize(stream, OffsetOfPrevHeader  == null ? -1 : OffsetOfPrevHeader.Value);
            Serializer.Serialize(stream, LengthOfCurrentBuffer);
        }

        public void Read(Stream stream)
        {
            var iVal = Serializer.DeserializeLong(stream);
            OffsetOfNextHeader = iVal == -1 ? null : (long?) iVal;

            iVal = Serializer.DeserializeLong(stream);
            OffsetOfPrevHeader = iVal == -1 ? null : (long?) iVal;

            LengthOfCurrentBuffer = Serializer.DeserializeInt(stream);
        }
    }
}