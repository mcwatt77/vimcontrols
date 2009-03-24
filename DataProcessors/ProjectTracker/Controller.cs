using ActionDictionary;
using ActionDictionary.Interfaces;

namespace DataProcessors.ProjectTracker
{
    public class Controller : IMissing
    {
        private readonly ItemList _itemList = new ItemList();

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_itemList);
        }
    }
}
