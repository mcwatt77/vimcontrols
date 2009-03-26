using System;

namespace NodeMessaging
{
    //TODO: turn IEntitySelector.set_SelectedRow(IEntityRow) into IFieldAccessor<string>.set_Value(fnGetBody(IEntityRow as INode))
    public class MessageAdaptor<TSource, TValue>
    {
        public MessageAdaptor(Func<TSource, TValue> f)
        {}
    }
}