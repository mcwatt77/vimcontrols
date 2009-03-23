using System;
using System.Collections.Generic;
using System.Linq;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer
{
    public class EntityListInterceptor : IEntityList
    {
        private readonly IEntityList _inner;
        private readonly IStringProvider _provider;
        private readonly Func<IEndNode, IStringProvider> _fnGetNewText;

        public EntityListInterceptor(IEntityList inner, IStringProvider provider, Func<IEndNode, IStringProvider> fnGetNewText)
        {
            _inner = inner;
            _provider = provider;
            _fnGetNewText = fnGetNewText;
        }

        public string DisplayName { get { return _inner.DisplayName; } set { _inner.DisplayName = value; } }
        public IEnumerable<IEntityRow> Rows { get { return _inner.Rows; } set { _inner.Rows = value; } }

        public IEntityRow SelectedRow
        {
            get { return _inner.SelectedRow; }
            set
            {
                _inner.SelectedRow = value;
                var val = value.Columns.First();
                var endNode = val as IEndNode;
                _provider.Text = _fnGetNewText(endNode).Text;
            }
        }
    }
}