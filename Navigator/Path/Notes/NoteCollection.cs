﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Navigator.Containers;
using Navigator.UI.Attributes;

namespace Navigator.Path.Notes
{
    public class NoteCollection : ISummaryString, IModelChildren
    {
        private readonly IContainer _container;
        private readonly XDocument _document;
        private readonly IEnumerable<object> _children;

        public NoteCollection(IContainer container)
        {
            _container = container;
#if DEBUG
            var file = new FileInfo(@"..\..\notes.xml");
#else
            var file = new FileInfo("notes.xml");
#endif

            _document = file.Exists
                            ? XDocument.Load(file.FullName)
                            : new XDocument(new XElement("notes"));

            _children = _document
                .Elements("notes")
                .Single()
                .Elements("note")
                .Select(noteElement => (object) _container.Get<NoteItem>(noteElement))
                .ToArray();
        }

        public string Summary
        {
            get { return "Notes"; }
        }

        public IEnumerable<object> Children
        {
            get
            {
                return _children;
            }
        }
    }
}