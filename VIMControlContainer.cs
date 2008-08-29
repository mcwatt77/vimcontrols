using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Controls;
using VIMControls.Controls.VIMForms;

namespace VIMControls
{
    public class VIMControlContainer : Grid, IVIMControlContainer, IVIMNavigable<object>
    {
        public string Uri { get; set; }

        public CommandMode Mode { get; set; }

        private IVIMMotionController _motionController;
        private IVIMPositionController _positionController;
        private IVIMCharacterController _characterController;
        private readonly Dictionary<string, UIElement> _savedViewers = new Dictionary<string, UIElement>();
        private readonly Dictionary<string, IVIMMotionController> _savedMotionControllers = new Dictionary<string, IVIMMotionController>();
        private string _currentViewer;
        private IVIMCommandController _commandController;
        private VIMTextControl _statusLine;
        private readonly VIMCommandText _commandText = new VIMCommandText();

        public VIMControlContainer()
        {
//            var db = ServiceLocator.FindService<IDirectoryBrowser>(this)();

            Mode = CommandMode.Normal;
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
        }

        private void InitializeRPNGrapher()
        {
            SaveCurrentViewer();

            var mainGrid = new Grid {ShowGridLines = true};
            Children.Add(mainGrid);
            mainGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(1, GridUnitType.Star)});
            mainGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(115, GridUnitType.Pixel)});

            var graphicDisplay = new VIMTextControl();
            mainGrid.Children.Add(graphicDisplay);
            SetRow(graphicDisplay, 0);

            var rpnCommand = new VIMRPNController();
            _commandController = rpnCommand;
            mainGrid.Children.Add(rpnCommand);
            SetRow(rpnCommand, 1);

            _currentViewer = "graph";

            _commandController.EnterCommandMode();
                
            _savedViewers[_currentViewer] = Children[0];

            Mode = CommandMode.Command;
        }

        private void InitializeMediaViewer(string uri)
        {
            Mode = CommandMode.Navigation;

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

        private void InitializeListNav<T>(string type) where T : VIMListBrowser
        {
            SaveCurrentViewer();

            var statusBarGrid = new Grid {ShowGridLines = true};
            Children.Add(statusBarGrid);
            statusBarGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            statusBarGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(23, GridUnitType.Pixel)});

            var treeInfoGrid = new Grid {ShowGridLines = true};
            statusBarGrid.Children.Add(treeInfoGrid);
            SetRow(treeInfoGrid, 0);
            treeInfoGrid.ColumnDefinitions.Add(new ColumnDefinition
                                                   {Width = new GridLength(0.75, GridUnitType.Star)});
            treeInfoGrid.ColumnDefinitions.Add(new ColumnDefinition
                                                   {Width = new GridLength(0.25, GridUnitType.Star)});

            var lineInfoGrid = new Grid {ShowGridLines = true};
            treeInfoGrid.Children.Add(lineInfoGrid);
            SetColumn(lineInfoGrid, 0);
            lineInfoGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(23, GridUnitType.Pixel)});
            lineInfoGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

            var lineInfo = new VIMTextControl();
            _statusLine = lineInfo;
            lineInfoGrid.Children.Add(lineInfo);
            SetRow(lineInfo, 0);

            var newObj = typeof (T).GetConstructor(new []{typeof(IVIMContainer)}).Invoke(new object[] {this});
            var directoryBrowser = newObj as T;
//            var directoryBrowser = new T(this);
            lineInfoGrid.Children.Add(directoryBrowser);
            SetRow(directoryBrowser, 1);
            _motionController = directoryBrowser;

            var treeInfo = new VIMTextControl();
            treeInfoGrid.Children.Add(treeInfo);
            SetColumn(treeInfo, 1);

            if (_commandText.Parent != null)
                (_commandText.Parent as Grid).Children.Remove(_commandText);
            statusBarGrid.Children.Add(_commandText);
            _commandController = _commandText;
            SetRow(_commandText, 1);

            _currentViewer = type;
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
            if (_currentViewer == "file") return;

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

        public void EnterCommandMode()
        {
            Mode = CommandMode.Command;
            _commandController.EnterCommandMode();
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

        //todo: Refactor!
        public void Navigate(object obj)
        {
            var navType = typeof (IVIMNavigable<>).MakeGenericType(obj.GetType());
            var findServiceGen = typeof (ServiceLocator).GetMethod("FindService", new[] {typeof (object[])});
            var findService = findServiceGen.MakeGenericMethod(navType);
            var fnNav = findService.Invoke(null, new object[] {new object[] {this}});
            var methodType = typeof (Func<>).MakeGenericType(navType);
            var methodMethodInfo = methodType.GetMethod("Invoke", Type.EmptyTypes);
            var ctrl = methodMethodInfo.Invoke(fnNav, new object[]{}) as IVIMControl;
            var elem = ctrl.GetUIElement() as UIElementWrapper;

            InitializeForm(elem.UiElement, ctrl);

            var navigate = navType.GetMethod("Navigate", new [] {obj.GetType()});
            navigate.Invoke(ctrl, new [] {obj});
        }

        public void Navigate(string uri)
        {
            switch (uri.ToLower())
            {
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
                    if (_currentViewer == "file")
                        InitializeMediaViewer(uri);
                    break;
            }
        }
    }
}