﻿using System.Collections.Generic;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class UI : IUIInitialize
    {
        public IEnumerable<IUIInitialize> Children { get; set; }

        public void Initialize()
        {
        }

        public IContainer Parent
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
