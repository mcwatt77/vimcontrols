using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Navigator.Repository.PartitionedFile
{
    [TestFixture]
    public class PartitionedFileTest
    {
        [Test]
        public void Test()
        {
            var fileInfo = new FileInfo("partition_test.dat");
            if (fileInfo.Exists) fileInfo.Delete();

            PersistInfo(fileInfo);
            ReloadInfo(fileInfo);
        }

        private static void ReloadInfo(FileSystemInfo fileInfo)
        {
            var partitionedFile = PartitionedFile.FromPath(fileInfo.FullName);

            ProcessFile(partitionedFile, DeserializeAndAssert);
        }

        private static void PersistInfo(FileSystemInfo fileInfo)
        {
            var partitionedFile = GetPartitionedFile(fileInfo);

            ProcessFile(partitionedFile, Serialize);
        }

        private static void DeserializeAndAssert(Stream stream, object @object)
        {
            var serializeMethod = typeof (Serializer)
                .GetMethods()
                .Where(method => method.GetParameters().Count() == 1)
                .Single(method => method.ReturnType == @object.GetType());

            var result = serializeMethod.Invoke(null, new[] {stream});
            Assert.AreEqual(@object, result);
        }

        private static void Serialize(Stream stream, object @object)
        {
            var serializeMethod = typeof (Serializer)
                .GetMethods()
                .Where(method => method.GetParameters().Count() == 2)
                .Single(method => method.GetParameters().ElementAt(1).ParameterType == @object.GetType());

            serializeMethod.Invoke(null, new[] {stream, @object});
        }

        private static void ProcessFile(PartitionedFile partitionedFile, Action<Stream, object> processStream)
        {
            var theDataSection = partitionedFile.Sections.ElementAt(2);

            var table1Section = theDataSection.Sections.ElementAt(0);
            var table1Stream = table1Section.GetStream();
            processStream(table1Stream, "Table 1 Test");

            var table2Section = theDataSection.Sections.ElementAt(1);
            var table2Stream = table2Section.GetStream();
            processStream(table2Stream, 2);

            var sectionIndexSection = partitionedFile.Sections.ElementAt(1);
            var sectionIndexStream = sectionIndexSection.GetStream();
            processStream(sectionIndexStream, 2);
            processStream(sectionIndexStream, table1Section.Offset);
            processStream(sectionIndexStream, typeof(string));
            processStream(sectionIndexStream, table2Section.Offset);
            processStream(sectionIndexStream, typeof(int));
        }

        private static PartitionedFile GetPartitionedFile(FileSystemInfo fileInfo)
        {
            var partitionedFile = PartitionedFile.FromPath(fileInfo.FullName);

            var typeDictionarySection = new PartitionedFileSection();
            partitionedFile.Sections.Add(typeDictionarySection);

            partitionedFile.Sections.Add(new PartitionedFileSection());

            var dataSection = new PartitionedFileSection();
            partitionedFile.Sections.Add(dataSection);

            dataSection.Sections.Add(new PartitionedFileSection());
            dataSection.Sections.Add(new PartitionedFileSection());
            return partitionedFile;
        }
    }

    public class PartitionedFile
    {
        public static PartitionedFile FromPath(string fileName)
        {
            return new PartitionedFile(fileName);
        }

        private PartitionedFile(string fileName)
        {
            Sections = new List<PartitionedFileSection>();
            BufferSize = 4096;

            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {}
        }

        public short BufferSize { get; set; }
        public ICollection<PartitionedFileSection> Sections { get; private set; }
    }

    public class PartitionedFileSectionCollection : ICollection<PartitionedFileSection>
    {
        private readonly Stream _stream;
        private readonly int _bufferSize;

        public PartitionedFileSectionCollection(Stream stream, int bufferSize)
        {
            _stream = stream;
            _bufferSize = bufferSize;

            if (_stream.Length != 0)
            {
                var numberOfSections = Serializer.DeserializeInt(_stream);
                for (var i = 0; i < numberOfSections;)
                {
                    var lengthOfBuffer = Serializer.DeserializeInt(_stream);
                }
            }
            else Serializer.Serialize(_stream, 0);
        }

        private readonly List<PartitionedFileSection> _sections = new List<PartitionedFileSection>();

        public IEnumerator<PartitionedFileSection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(PartitionedFileSection item)
        {
            Serializer.Serialize(_stream, _bufferSize);
            _sections.Add(item);
        }

        public void Clear()
        {
            _sections.Clear();
        }

        public bool Contains(PartitionedFileSection item)
        {
            return _sections.Contains(item);
        }

        public void CopyTo(PartitionedFileSection[] array, int arrayIndex)
        {
            _sections.CopyTo(array, arrayIndex);
        }

        public bool Remove(PartitionedFileSection item)
        {
            return _sections.Remove(item);
        }

        public int Count
        {
            get { return _sections.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }

    public class PartitionedFileSection
    {
        public PartitionedFileSection()
        {
            Sections = new List<PartitionedFileSection>();
        }

        public long Offset { get; set; }
        public Stream GetStream()
        {
            return null;
        }
        public ICollection<PartitionedFileSection> Sections { get; set; }
    }
}