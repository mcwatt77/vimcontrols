using System.Collections.Generic;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace Tests
{
    public class TestFactorySetup
    {
        public MockRepository Repository { get; private set; }
        private IStackPanel stackPanel;

        public void Initialize()
        {
            Repository = new MockRepository();
            var textFactory = Repository.DynamicMock<ITextFactory>();
            var stackPanelFactory = Repository.DynamicMock<IStackPanelFactory>();
            var text = Repository.DynamicMock<IText>();
            var textCursor = Repository.DynamicMock<IVIMTextCursor>();
            stackPanel = Repository.DynamicMultiMock<IStackPanel>();
            var panelChildren = new List<IUIElement>();

            Services.Register<ITextFactory>(textFactory);
            Services.Register<IStackPanelFactory>(stackPanelFactory);
            Services.Register<IVIMTextCursor>(textCursor);

            textFactory.Expect(factory => factory.Create()).Return(text).Repeat.Any();
            stackPanelFactory.Expect(factory => factory.Create()).Return(stackPanel);
            stackPanel.Expect(a => a.Children).Return(panelChildren).Repeat.Any();
            textCursor.Expect(a => a.TextPosition).Return(new VIMTextDataPosition{Column = 0, Line = 0});
        }
    }
}
