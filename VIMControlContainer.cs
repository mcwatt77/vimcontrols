using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Controls;

namespace VIMControls
{
    public class VIMControlContainer : Grid, IVIMControlContainer
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
            var db = ServiceLocator.FindService<IDirectoryBrowser>(this)();

            Mode = CommandMode.Normal;
            _commandController = _commandText;
        }

        public VIMControlContainer(string uri) : this()
        {
            Uri = uri;
            Navigate(uri);
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
                    InitializeForm();
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

        private void InitializeForm()
        {
            SaveCurrentViewer();

            var mainForm = new VIMFormControl(this);
            Children.Add(mainForm);

            _currentViewer = "form";
            _characterController = mainForm;

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

        private void InitializeListNav<T>(string type) where T : VIMDirectoryControlBase
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
            _characterController.Output(c);
        }

        public void NewLine()
        {
            _characterController.NewLine();
        }

        public void Backspace()
        {
            _characterController.Backspace();
        }

        public void MoveVertically(int i)
        {
            _motionController.MoveVertically(i);
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
    }
}