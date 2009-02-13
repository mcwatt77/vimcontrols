using ActionDictionary;
using ActionDictionary.Interfaces;

namespace DataProcessors.ProjectTracker
{
    public class Controller : IMissing
    {
        private readonly ItemList _itemList = new ItemList();

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_itemList);
        }
    }
}
