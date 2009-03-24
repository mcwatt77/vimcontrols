using System;
using System.Collections.Generic;
using System.Linq;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer
{
    public class EntityListInterceptor : IEntitySelector
    {
        private readonly IEntitySelector _inner;
        private readonly IFieldAccessor<string> _provider;
        private readonly Func<IEndNode, IFieldAccessor<string>> _fnGetNewText;

        public EntityListInterceptor(IEntitySelector inner, IFieldAccessor<string> provider, Func<IEndNode, IFieldAccessor<string>> fnGetNewText)
        {
            _inner = inner;
            _provider = provider;
            _fnGetNewText = fnGetNewText;
        }

        public IEnumerable<IEntityRow> Rows { get { return _inner.Rows; } set { _inner.Rows = value; } }

        public IEntityRow SelectedRow
        {
            get { return _inner.SelectedRow; }
            set
            {
                _inner.SelectedRow = value;
                var val = value.Columns.First();
                var endNode = val as IEndNode;
                _provider.Value = _fnGetNewText(endNode).Value;
            }
        }
    }
}