using System;
using System.IO;
using NUnit.Framework;

namespace Navigator.Repository.PartitionedFile
{
    [TestFixture]
    public class PartitionedFileStreamTest
    {
        [Test]
        public void Test()
        {
            var fileStream = new FileStream("pfsTest", FileMode.Create);

            var headerAdder = new HeaderAdder(fileStream, 2);
            var partitionedFileStream = new PartitionedFileStream(fileStream, headerAdder.AddNewHeaderSection(), headerAdder.AddNewHeaderSection);

            Serializer.Serialize(partitionedFileStream, -1);

            partitionedFileStream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(-1, Serializer.DeserializeInt(partitionedFileStream));

            //TODO: Add a test where the header size is 4 or something, and add a large string, and try to recover it
        }

        private class HeaderAdder
        {
            private readonly FileStream _fileStream;
            private readonly int _bufferSize;

            public HeaderAdder(FileStream fileStream, int bufferSize)
            {
                _fileStream = fileStream;
                _bufferSize = bufferSize;
            }

            public PartitionedFileHeader AddNewHeaderSection()
            {
                var header = new PartitionedFileHeader(_fileStream.Length) { LengthOfCurrentBuffer = _bufferSize };
                _fileStream.Seek(0, SeekOrigin.End);
                header.Write(_fileStream);

                var buffer = new byte[_bufferSize];
                _fileStream.Write(buffer, 0, buffer.Length);

                _fileStream.Seek(PartitionedFileHeader.DataOffset, SeekOrigin.Begin);

                return header;
            }
        }
    }

    public class PartitionedFileStream : Stream
    {
        private readonly FileStream _fileStream;
        private PartitionedFileHeader _header;
        private readonly Func<PartitionedFileHeader> _getNewHeader;
        private long _length;
        private long _position;

        public PartitionedFileStream(FileStream fileStream, PartitionedFileHeader header, Func<PartitionedFileHeader> getNewHeader)
        {
            _fileStream = fileStream;
            _header = header;
            _getNewHeader = getNewHeader;

            UpdateLength();

            SeekFromBegin(0);

            _position = 0;
        }

        private long OffsetInCurrentHeader { get { return _fileStream.Position - (_header.OffsetOfHeader + PartitionedFileHeader.DataOffset); } }
        private long BytesRemainingInCurrentHeader { get { return _header.LengthOfCurrentBuffer - OffsetInCurrentHeader; } }

        private void UpdateLength()
        {
            long totalBufferLength = 0;
            var currentHeader = _header;
            while (currentHeader.OffsetOfPrevHeader != null)
            {
                _fileStream.Seek(currentHeader.OffsetOfPrevHeader.Value, SeekOrigin.Begin);
                currentHeader.Read(_fileStream);
                totalBufferLength += currentHeader.LengthOfCurrentBuffer;
            }
            totalBufferLength += _header.LengthOfCurrentBuffer;

            currentHeader = _header;
            while (currentHeader.OffsetOfNextHeader != null)
            {
                _fileStream.Seek(currentHeader.OffsetOfNextHeader.Value, SeekOrigin.Begin);
                currentHeader.Read(_fileStream);
                totalBufferLength += currentHeader.LengthOfCurrentBuffer;
            }

            SetLength(totalBufferLength);
        }

        public override void Flush()
        {
            _fileStream.Flush();
        }

        private long SeekFromEnd(long offset)
        {
            return SeekFromCurrent(offset + _length - Position);
        }

        private long SeekFromBegin(long offset)
        {
            return SeekFromCurrent(offset - Position);
        }

        private long SeekFromCurrent(long offset)
        {
            if (OffsetInCurrentHeader + offset < 0)
            {
                var newOffset = offset + OffsetInCurrentHeader;
                Position -= OffsetInCurrentHeader;
                MoveToPreviousHeader();
                _fileStream.Seek(
                    _header.LengthOfCurrentBuffer + _header.OffsetOfHeader + PartitionedFileHeader.DataOffset,
                    SeekOrigin.Begin);
                return SeekFromCurrent(newOffset);
            }
            if (offset > BytesRemainingInCurrentHeader)
            {
                var newOffset = offset - BytesRemainingInCurrentHeader;
                Position += BytesRemainingInCurrentHeader;
                MoveToNextHeader();
                _fileStream.Seek(_header.OffsetOfHeader + PartitionedFileHeader.DataOffset, SeekOrigin.Begin);
                return SeekFromCurrent(newOffset);
            }
            Position += offset;
            return _fileStream.Seek(offset, SeekOrigin.Current);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Current)
                return SeekFromCurrent(offset);
            if (origin == SeekOrigin.Begin)
                return SeekFromBegin(offset);
            return SeekFromEnd(offset);
        }

        private void MoveToNextHeader()
        {
            var oldPosition = _fileStream.Position;
            if (_header.OffsetOfNextHeader == null)
            {
                var nextHeader = _getNewHeader();
                _header.OffsetOfNextHeader = nextHeader.OffsetOfHeader;
                _fileStream.Seek(_header.OffsetOfHeader, SeekOrigin.Begin);
                _header.Write(_fileStream);

                nextHeader.OffsetOfPrevHeader = _header.OffsetOfHeader;
                _fileStream.Seek(nextHeader.OffsetOfHeader, SeekOrigin.Begin);
                nextHeader.Write(_fileStream);

                _header = nextHeader;
            }
            else
            {
                _fileStream.Seek(_header.OffsetOfNextHeader.Value, SeekOrigin.Begin);
                _header = new PartitionedFileHeader(_header.OffsetOfNextHeader.Value);
                _header.Read(_fileStream);
            }
            _fileStream.Seek(oldPosition, SeekOrigin.Begin);
        }

        private void MoveToPreviousHeader()
        {
            if (_header.OffsetOfPrevHeader == null)
                throw new InvalidOperationException("Already at the beginning of the Stream");

            var oldPosition = _fileStream.Position;
            _fileStream.Seek(_header.OffsetOfPrevHeader.Value, SeekOrigin.Begin);
            _header = new PartitionedFileHeader(_header.OffsetOfPrevHeader.Value);
            _header.Read(_fileStream);
            _fileStream.Seek(oldPosition, SeekOrigin.Begin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > BytesRemainingInCurrentHeader)
            {
                var bytesRemainingInCurrentHeader = BytesRemainingInCurrentHeader;
                _fileStream.Read(buffer, offset, (int) bytesRemainingInCurrentHeader);

                Position += bytesRemainingInCurrentHeader;

                MoveToNextHeader();

                _fileStream.Seek(_header.OffsetOfHeader + PartitionedFileHeader.DataOffset, SeekOrigin.Begin);

                return Read(buffer, offset + (int) bytesRemainingInCurrentHeader, count - (int) bytesRemainingInCurrentHeader)
                       + (int) bytesRemainingInCurrentHeader;
            }

            Position += count;
            return _fileStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > BytesRemainingInCurrentHeader)
            {
                var bytesRemainingInCurrentHeader = BytesRemainingInCurrentHeader;
                _fileStream.Write(buffer, offset, (int) bytesRemainingInCurrentHeader);

                Position += bytesRemainingInCurrentHeader;

                MoveToNextHeader();

                _fileStream.Seek(_header.OffsetOfHeader + PartitionedFileHeader.DataOffset, SeekOrigin.Begin);

                Write(buffer, offset + (int) bytesRemainingInCurrentHeader, count - (int) bytesRemainingInCurrentHeader);
            }

            Position += count;
            _fileStream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override long Position { get { return _position; } set { _position = value; } }
    }
}