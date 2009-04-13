using System.Collections.Generic;
using ActionDictionary;
using ActionDictionary.Interfaces;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer.WPF
{
    public class UI : IUIInitialize, IMissing
    {
        public IEnumerable<IUIInitialize> Children { get; set; }
        public object Controller { get; set; }

        public void Initialize()
        {
            Children.Do(child =>
                            {
                                child.Parent = Parent;
                                child.Initialize();
                            });
        }

        public IContainer Parent { get; set; }
        public object ProcessMissingCmd(Message msg)
        {
            return Controller != null ? msg.Invoke(Controller) : null;
        }
    }
}