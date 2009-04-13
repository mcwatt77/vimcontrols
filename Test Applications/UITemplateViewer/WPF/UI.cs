using System.Collections.Generic;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer.WPF
{
    public class UI : IUIInitialize
    {
        public IEnumerable<IUIInitialize> Children { get; set; }

        public void Initialize()
        {
            Children.Do(child =>
                            {
                                child.Parent = Parent;
                                child.Initialize();
                            });
        }

        public IContainer Parent { get; set; }
    }
}
