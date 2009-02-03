using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Contracts;
using VIMControls.Controls;
using VIMControls.Controls.Misc;
using VIMControls.Controls.StackProcessor;
using VIMControls.Controls.StackProcessor.Graphing;
using VIMControls.Controls.VIMForms;

namespace VIMControls.Controls.Container
{
    public class VIMControlContainer : Grid, IVIMControlContainer
    {
        public string Uri { get; set; }

        private CommandMode _mode = CommandMode.Normal;
        public CommandMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                VIMMessageService.SendMessage<IVIMSystemUICommands>(c => c.UpdateTitle());
            }
        }

        public void Focus(IVIMController controller)
        {
            if (controller is IVIMExpressionProcessor)
            {
                IntializeRPN();
            }
        }

        private IListController _listController;
        private IVIMMotionController _motionController;
        private IVIMPositionController _positionController;
        private IVIMCharacterController _characterController;
        private readonly Dictionary<string, UIElement> _savedViewers = new Dictionary<string, UIElement>();
        private readonly Dictionary<string, IVIMMotionController> _savedMotionControllers = new Dictionary<string, IVIMMotionController>();
        private string _currentViewer;
        private IVIMCommandController _commandController;
        private VIMTextControl _statusLine;
        private readonly VIMCommandText _commandText = new VIMCommandText();
        private readonly Dictionary<Type, IVIMController> _handlers = new Dictionary<Type, IVIMController>();

        public VIMControlContainer()
        {
            _commandController = _commandText;
        }

        public VIMControlContainer(string uri) : this()
        {
            Uri = uri;
            Navigate(uri);
        }

        private void InitializeForm(UIElement elem, IVIMControl ctrl)
        {
            SaveCurrentViewer();

            Children.Add(elem);

            _currentViewer = "form";
            _characterController = ctrl as IVIMCharacterController;

            _savedViewers[_currentViewer] = Children[0];

            _handlers[typeof (IVIMForm)] = (IVIMForm)ctrl;
        }

        private void InitializeRPNGrapher()
        {
            SaveCurrentViewer();

            var mainGrid = new Grid {ShowGridLines = true};
            Children.Add(mainGrid);
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(1, GridUnitType.Star)});
            mainGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(115, GridUnitType.Pixel)});

            var graphicDisplay = new VIMTextControl();
            var gdUiElement = UIElementWrapper.From(graphicDisplay);
            mainGrid.Children.Add(gdUiElement);
            SetRow(gdUiElement, 0);

            var rpnCommand = new VIMRPNController();
            var rpnUiElement = UIElementWrapper.From(rpnCommand);
            _commandController = rpnCommand;
            mainGrid.Children.Add(rpnUiElement);
            SetRow(rpnUiElement, 1);

            _currentViewer = "graph";

            _commandController.EnterCommandMode();
                
            _savedViewers[_currentViewer] = Children[0];

            Mode = CommandMode.Command;
        }

        private MediaElement _audio;
        private void InitializeMediaViewer(string uri)
        {
            Mode = CommandMode.Navigation;
            var fileInfo = new FileInfo(uri);
            if (fileInfo.Extension == ".mp3")
            {
                _audio = new MediaElement() {Source = new Uri(uri)};
                _audio.LoadedBehavior = MediaState.Manual;
                _audio.Play();
                return;
            }

            SaveCurrentViewer();
            var media = new VIMMediaControl(this) {Source = new Uri(uri)};

            Children.Add(media);
            _currentViewer = "media";
            _motionController = media;
            _positionController = media;

            _savedViewers[_currentViewer] = Children[0];
        }

        private void SaveCurrentViewer()
        {
            if (Children.Count != 1) return;

            _savedViewers[_currentViewer] = Children[0];
            _savedMotionControllers[_currentViewer] = _motionController;
            Children.RemoveAt(0);
        }

        private void InitializeMRU()
        {
            InitializeListNav<VIMMRUControl>("mru");
        }

        private enum SplitType
        {
            DynamicFirst, DynamicSecond, DynamicBoth
        }

        private void InitializeListNav<T>(string type) where T : VIMListBrowser
        {
            SaveCurrentViewer();

            var statusBarGrid = SetupSplit(Children, Orientation.Vertical, SplitType.DynamicFirst, 1, 23);

            AddCommandText(statusBarGrid);
            var treeInfoGrid = SetupSplit(statusBarGrid.Children, Orientation.Horizontal, SplitType.DynamicBoth, 0.75);
            SetRow(treeInfoGrid, 0);

            var lineInfoGrid = SetupSplit(treeInfoGrid.Children, Orientation.Vertical, SplitType.DynamicSecond, 23, 1);
            SetColumn(lineInfoGrid, 0);

            var lineInfo = new VIMTextControl();
            var liUiElement = UIElementWrapper.From(lineInfo);
            _statusLine = lineInfo;
            lineInfoGrid.Children.Add(liUiElement);
            SetRow(liUiElement, 0);

            var newObj = typeof (T).GetConstructor(new []{typeof(IVIMContainer)}).Invoke(new object[] {this});
            var directoryBrowser = (T)newObj;
            var dbUiElement = UIElementWrapper.From(directoryBrowser);

            var canvas = DecorateWithCanvas(dbUiElement);
            lineInfoGrid.Children.Add(canvas);
            SetRow(canvas, 1);
            _motionController = directoryBrowser;

            AddCursor(canvas as Canvas, directoryBrowser);

            var treeInfo = new VIMTextControl();
            var tiUiElement = UIElementWrapper.From(treeInfo);
            treeInfoGrid.Children.Add(tiUiElement);
            SetColumn(tiUiElement, 1);

            _currentViewer = type;

            _savedViewers[_currentViewer] = Children[0];

            _listController = directoryBrowser;
        }

        private void AddCommandText(Panel statusBarGrid)
        {
            var ctUiElement = (StackPanel)UIElementWrapper.From(_commandText);
            if (ctUiElement.Parent != null)
                ((Grid)ctUiElement.Parent).Children.Remove(ctUiElement);
            statusBarGrid.Children.Add(ctUiElement);
            _commandController = _commandText;
            _handlers[typeof (IVIMCommandController)] = _commandText;
            SetRow(ctUiElement, 1);
        }

        private void AddCursor<T>(Panel canvas, T directoryBrowser)
        {
            var cursor = Services.Locate<IVIMListCursor>(directoryBrowser)();
            _motionController = new ListMotionWrapper(cursor);
            var elem = (UIElementWrapper)cursor.GetUIElement();
            canvas.Children.Add(elem.UiElement);
        }

        private static Grid SetupSplit(UIElementCollection elements, Orientation o, SplitType splitType, double val1)
        {
            return SetupSplit(elements, o, splitType, val1, null);
        }

        private static Grid SetupSplit(UIElementCollection elements, Orientation o, SplitType splitType, double val1, double? val2)
        {
            var grid = new Grid {ShowGridLines = true};
            elements.Add(grid);
            var grid1 = new GridLength(val1,
                                       splitType == SplitType.DynamicFirst || splitType == SplitType.DynamicBoth
                                           ? GridUnitType.Star
                                           : GridUnitType.Pixel);
            var grid2 = new GridLength(val2 ?? 1.0 - val1,
                                       splitType == SplitType.DynamicSecond || splitType == SplitType.DynamicBoth
                                           ? GridUnitType.Star
                                           : GridUnitType.Pixel);

            if (o == Orientation.Vertical)
            {
                grid.RowDefinitions.Add(new RowDefinition {Height = grid1});
                grid.RowDefinitions.Add(new RowDefinition {Height = grid2});
            }
            else
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = grid1});
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = grid2});
            }
            return grid;
        }

        private static FrameworkElement DecorateWithCanvas(UIElement elem)
        {
            var canvas = new FillableCanvas();
            canvas.Children.Add(elem);
            Canvas.SetLeft(elem, 0);
            Canvas.SetTop(elem, 0);
            return canvas;
        }

        public void StatusLine(string status)
        {
            _statusLine.Value = status;
        }

        public void Output(char c)
        {
            if (_characterController != null) _characterController.Output(c);
        }

        public void NewLine()
        {
            if (_characterController != null) _characterController.NewLine();
        }

        public void Backspace()
        {
            if (_characterController != null) _characterController.Backspace();
        }

        public void MoveVertically(int i)
        {
            if (_motionController != null) _motionController.MoveVertically(i);
        }

        public void MoveHorizontally(int i)
        {
            if (_motionController != null) _motionController.MoveHorizontally(i);
        }

        public void EndOfLine()
        {
        }

        public void BeginningOfLine()
        {
        }

        public void NextLine()
        {
            _motionController.NextLine();
        }

        public void ResetInput()
        {
            Mode = CommandMode.Normal;
        }

        public void OldResetInput()
        {
            if (_currentViewer == "file") return;
            
/*            var media = _motionController as VIMMediaControl;
            if (media != null) media.Close();*/

            Children.RemoveAt(0);
            _currentViewer = "file";
            if (!_savedViewers.ContainsKey(_currentViewer))
            {
                InitializeListNav<VIMDirectoryControl>("file");
                SaveCurrentViewer();
            }
            Children.Add(_savedViewers[_currentViewer]);
            _motionController = _savedMotionControllers[_currentViewer];
            _positionController = null;

            Mode = CommandMode.Normal;
        }

        public void MissingModeAction(IVIMAction action)
        {
            if (_handlers.ContainsKey(action.ControllerType))
            {
                action.Invoke(_handlers[action.ControllerType]);
            }
        }

        public void MissingMapping()
        {
        }

        public void Move(GridLength horz, GridLength vert)
        {
            if (_positionController != null)
            {
                _positionController.Move(horz, vert);
            }
        }

        public void TogglePositionIndicator()
        {
            if (_positionController != null)
            {
                _positionController.TogglePositionIndicator();
            }
        }

        public void InfoCharacter(char c)
        {
            _commandController.InfoCharacter(c);
        }

        public void CommandCharacter(char c)
        {
            _commandController.CommandCharacter(c);
        }

        public void Execute()
        {
            _commandController.Execute();
            Mode = CommandMode.Normal;
        }

        public void CommandBackspace()
        {
            _commandController.CommandBackspace();
        }

        public void DeleteAtCursor()
        {
        }

        public void EnterInsertMode(CharacterInsertMode mode)
        {
            Mode = CommandMode.Insert;
        }

        public void InvalidCommand(string cmd)
        {
            _commandController.InvalidCommand(cmd);
        }

        public void EnterCommandMode()
        {
            if (!_handlers.ContainsKey(typeof(IVIMCommandController)))
            {
                _handlers[typeof (IVIMCommandController)] = _commandText;
            }

            var ctUiElement = (StackPanel)UIElementWrapper.From(_commandText);
            if (!ctUiElement.IsLoaded)
                InsertCommandTextInCurrentView();

            _commandText.Text = ":";
            Mode = CommandMode.Command;
        }

        private void InsertCommandTextInCurrentView()
        {
            var ctUiElement = (StackPanel) UIElementWrapper.From(_commandText);
            if (ctUiElement.Parent != null)
            {
                var obj = (Grid)ctUiElement.Parent;
                obj.Children.Remove(ctUiElement);
            }

            var oldChild = Children[0];
            Children.Remove(oldChild);

            var grid = SetupSplit(Children, Orientation.Vertical, SplitType.DynamicFirst, 1, 23);
            grid.Children.Add(oldChild);
            SetRow(oldChild, 0);

            grid.Children.Add(ctUiElement);
            SetRow(ctUiElement, 1);
        }

        public void EnterNormalMode()
        {
            if (Mode == CommandMode.Insert)
                Mode = CommandMode.Normal;
        }

        public void InsertLine(LineInsertMode mode)
        {
        }

        public void Save()
        {
            if (_currentViewer != "form") return;

            var persist = _savedViewers[_currentViewer] as IVIMPersistable;
            if (persist == null) return;
            persist.Save();
        }

        public void Delete()
        {
            if (_currentViewer != "form") return;

            var persist = _savedViewers[_currentViewer] as IVIMPersistable;
            if (persist == null) return;
            persist.Delete();
        }

        public void Navigate(object obj)
        {
            var navType = typeof (IVIMNavigable<>).MakeGenericType(obj.GetType());
            var findServiceGen = typeof (Services).GetMethod("Locate", new[] {typeof (object[])});
            var findService = findServiceGen.MakeGenericMethod(navType);
            var fnNav = findService.Invoke(null, new object[] {new object[] {this}});
            var methodType = typeof (Func<>).MakeGenericType(navType);
            var methodMethodInfo = methodType.GetMethod("Invoke", Type.EmptyTypes);
            var ctrl = (IVIMControl)methodMethodInfo.Invoke(fnNav, new object[]{});
            var elem = ((UIElementWrapper)ctrl.GetUIElement());

            InitializeForm(elem.UiElement, ctrl);

            var navigate = navType.GetMethod("Navigate", new [] {obj.GetType()});
            navigate.Invoke(ctrl, new [] {obj});
        }

        public void Navigate(string uri)
        {
            if (uri == ".")
            {
                OldResetInput();
                return;
            }
            switch (uri.ToLower())
            {
                case "rpn":
                    IntializeRPN();
                    break;
                case "computer":
                    InitializeListNav<VIMDirectoryControl>("file");
                    break;
                case "graph":
                    InitializeRPNGrapher();
                    break;
                case "form":
                    var ctrl = new VIMFormControl(this);
                    InitializeForm(ctrl, ctrl);
                    break;
                case "mru":
                    InitializeMRU();
                    break;
                default:
//                    if (_currentViewer == "file")
                        InitializeMediaViewer(uri);
                    break;
            }
        }

        private void IntializeRPN()
        {
            SaveCurrentViewer();

            var stackPanel = new StackPanel();

            var stackInputController = Services.Locate<IStackInputController>()();
            var stackCtrl = (FrameworkElement)UIElementWrapper.From(stackInputController);
            stackCtrl.Height = 25;
            _handlers[typeof (IStackInputController)] = stackInputController;

            var expressionProcessor = Services.Locate<IVIMExpressionProcessor>()();
            var exprCtrl = (FrameworkElement)UIElementWrapper.From(expressionProcessor);
            exprCtrl.Height = expressionProcessor.GetRequiredHeight(4);
            _handlers[typeof(IVIMExpressionProcessor)] = expressionProcessor;

            var fancyDisplayStack = Services.Locate<IFancyDisplayStack>()();
            var graphPanel = Services.Locate<IVIMGraphPanel>()();
            _handlers[typeof (IVIMGraphPanel)] = graphPanel;
            var graphCtrl = (FrameworkElement) UIElementWrapper.From(graphPanel);

            var splitGrid = SetupSplit(Children, Orientation.Vertical, SplitType.DynamicSecond,
                                       exprCtrl.Height + stackCtrl.Height, 1);

//            Children.Add(stackPanel);
            splitGrid.Children.Add(stackPanel);
            SetRow(stackPanel, 0);

            stackPanel.Children.Add(stackCtrl);
            stackPanel.Children.Add(exprCtrl);

            splitGrid.Children.Add(graphCtrl);
            SetRow(graphCtrl, 1);

            _currentViewer = "rpn";

            _characterController = stackInputController;

            Mode = CommandMode.StackInsert;

//            stackPanel.Children.Add(stackInputController.GetUIElement());

            //stackinput should take one line
            //expressionprocessor should take 4 lines
            //fancydisplaystack should take 4 lines
            //graph panel should take remaining space
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }

        public void Select(int index)
        {
            if (_currentViewer == "file")
            {
                _listController.Select(index);
            }
        }

        public string Text
        {
            get { return String.Empty; }
            set
            {
                SaveCurrentViewer();

                var text = new VIMTextControl();
                var elem = UIElementWrapper.From(text);

                var canvas = (Canvas)DecorateWithCanvas(elem);
                Children.Add(canvas);

                _currentViewer = "edit";
                _characterController = text;

                _savedViewers[_currentViewer] = Children[0];

                _handlers[typeof (ITextInputProvider)] = text;

                text.Text = value;

                Mode = CommandMode.Insert;

                var cursor = Services.Locate<IVIMTextCursor>(text)();
                _handlers[typeof (IVIMTextCursor)] = cursor;
                _motionController = cursor;
                var cElem = (UIElementWrapper)cursor.GetUIElement();
                canvas.Children.Add(cElem.UiElement);
            }
        }
    }
}