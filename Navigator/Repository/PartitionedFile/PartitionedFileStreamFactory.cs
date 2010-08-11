using System.Collections.Generic;
using System.IO;

namespace Navigator.Repository.PartitionedFile
{
    //TODO: Instead of working from a file stream, have it work from a Stream
    public class PartitionedFileStreamFactory
    {
        private readonly FileStream _fileStream;
        private PartitionedFileHeader _header;

        private long _currentHeaderOffset;
        private long _newStreamOffset;
        private long _dataStart;

        public PartitionedFileStreamFactory(string fileName)
        {
            _fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            if (_fileStream.Length == 0)
            {
                _header = new PartitionedFileHeader(0);
                _dataStart = _newStreamOffset = PartitionedFileHeader.DataOffset;
                _header.Write(_fileStream);
            }
        }

        private PartitionedFileHeader AddNewHeaderSection()
        {
            const int bufferSize = 4096;
            var header = new PartitionedFileHeader(_fileStream.Length) {LengthOfCurrentBuffer = bufferSize};
            _fileStream.Seek(0, SeekOrigin.End);
            header.Write(_fileStream);

            var buffer = new byte[bufferSize];
            _fileStream.Write(buffer, 0, buffer.Length);

            return header;
        }

        public IEnumerable<PartitionedFileStream> GetStreams()
        {
            _fileStream.Seek(0, SeekOrigin.Begin);
            var header = new PartitionedFileHeader(0);
            header.Read(_fileStream);

            var currentIndex = PartitionedFileHeader.DataOffset;
            var dataStart = currentIndex;
            var allOffsets = new List<long>();
            while (currentIndex + sizeof(long) < dataStart + header.LengthOfCurrentBuffer)
            {
                var partitionedFileStreamOffset = Serializer.DeserializeLong(_fileStream);
                allOffsets.Add(partitionedFileStreamOffset);
            }

            var allStreams = new List<PartitionedFileStream>();
            foreach (var offset in allOffsets)
            {
                header = new PartitionedFileHeader(offset);
                _fileStream.Seek(offset, SeekOrigin.Begin);
                header.Read(_fileStream);

                allStreams.Add(new PartitionedFileStream(_fileStream, header, AddNewHeaderSection));
            }

            return allStreams;
        }

        public PartitionedFileStream GetNewStream()
        {
            var bufferOverrun = (_newStreamOffset + sizeof (long)) - (_dataStart + _header.LengthOfCurrentBuffer);
            if (bufferOverrun > 0)
            {
                _header.OffsetOfNextHeader = _fileStream.Length;
                _fileStream.Seek(_currentHeaderOffset, SeekOrigin.Begin);
                _header.Write(_fileStream);
                _currentHeaderOffset = _header.OffsetOfHeader;

                _header = AddNewHeaderSection();

                _header.OffsetOfPrevHeader = _currentHeaderOffset;
                _fileStream.Seek(_header.OffsetOfHeader, SeekOrigin.Begin);
                _header.Write(_fileStream);
            }

            _fileStream.Seek(_newStreamOffset, SeekOrigin.Begin);
            var newHeader = AddNewHeaderSection();
            Serializer.Serialize(_fileStream, _fileStream.Length);
            _newStreamOffset += sizeof (long);
            
            return new PartitionedFileStream(_fileStream, newHeader, AddNewHeaderSection);
        }
    }
}