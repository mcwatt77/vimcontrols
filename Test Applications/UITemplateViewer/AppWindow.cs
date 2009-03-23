using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ActionDictionary;
using ActionDictionary.Interfaces;
using NodeMessaging;
using UITemplateViewer.Controllers;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;
using Utility.Core;

namespace UITemplateViewer
{
    public class AppWindow : Window, IWindow, IMissing, IError, IContainer
    {
        private static readonly MessageDictionary _mDict = new MessageDictionary();
        private readonly EntityListController _entityListController;

        public AppWindow()
        {
            Content = new StackPanel();

            var rootNode = new RootNode();
            var xmlNode = XmlNode.Parse("<root><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></root>");
            rootNode.Register<IParentNode>(xmlNode);

            var data = rootNode.Nodes("note").Select(note => note.Attribute("desc").Get<IStringProvider>());
            var rows = data.Select(row => (IEntityRow)new EntityRow {Columns = new[] {row}}).ToList();

            var entityList = new EntityList {Parent = this, Rows = rows, SelectedRow = rows.First()};
            entityList.Initialize();
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Parent = entityList);
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Initialize());

            var textDisplay = new TextDisplay {Parent = this};
            textDisplay.Initialize();

            Func<IEndNode, IStringProvider> fnGetNewText = endNode => endNode.Parent.Attribute("body").Get<IStringProvider>();
            var interceptor = new EntityListInterceptor(entityList, textDisplay, fnGetNewText);
            _entityListController = new EntityListController {EntityList = interceptor};

            interceptor.SelectedRow = interceptor.Rows.First();

            //TODO:  $$ Overall this is pretty awesome, but it can't respond to changes to the IStringProvider, only to the column list of the IEntityRow
        }

        public void Maximize()
        {
            throw new System.NotImplementedException();
        }

        public void Navigate(Type type)
        {
            throw new System.NotImplementedException();
        }

        public void Quit()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_entityListController);
        }

        public void Report(Exception e)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
//            if (!ProcessKeyState(e.KeyboardDevice, e.Key, Key.LeftCtrl, Key.RightCtrl)) return;

            var messages = _mDict.ProcessKey(e.Key);
            messages.Do(msg => msg.Invoke(this, false).Do(m => m.Invoke(this)));
        }

        public void AddChild(UIElement element)
        {
            ((StackPanel) Content).Children.Add(element);
        }
    }
}