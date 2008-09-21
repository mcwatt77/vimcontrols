using System;
using System.Collections.Generic;
using System.Linq;
using VIMControls.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;

namespace VIMControls
{
    public class LinkedListBrowser : IBrowser, ISearchable
    {
        private readonly IApplication _app;
        private readonly LinkedList<IBrowseElement> _list;
        private string _search = String.Empty;
        private LinkedListNode<IBrowseElement> _currentNode;

        public LinkedListBrowser(IApplication app, LinkedList<IBrowseElement> list)
        {
            _app = app;
            _list = list;
        }

        public IEnumerable<IBrowseElement> Elements
        {
            get { return _list.Select(elem => elem); }
        }

        public void AddSearchCharacter(char c)
        {
            _search += c;
        }

        public void FinalizeSearch()
        {
        }

        public void ProcessMissingCommand(ICommand command)
        {
        }

        public void MoveRight(int i)
        {
        }

        public void Navigate()
        {
            _app.SetView(String.Empty);
        }
    }
}