﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Contracts;
using VIMControls.Controls;
using VIMControls.Controls.VIMForms;

namespace VIMControls.Controls
{
    public class VIMControlContainer : Grid, IVIMControlContainer
    {
        public string Uri { get; set; }

        public CommandMode Mode { get; set; }

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

        public VIMControlContainer()
        {
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
            _statusLine = lineInfo;
            lineInfoGrid.Children.Add(lineInfo);
            SetRow(lineInfo, 0);

            var newObj = typeof (T).GetConstructor(new []{typeof(IVIMContainer)}).Invoke(new object[] {this});
            var directoryBrowser = (T)newObj;

            var canvas = DecorateWithCanvas(directoryBrowser);
            lineInfoGrid.Children.Add(canvas);
            SetRow(canvas, 1);
            _motionController = directoryBrowser;

            AddCursor<T>(canvas as Canvas, directoryBrowser);

            var treeInfo = new VIMTextControl();
            treeInfoGrid.Children.Add(treeInfo);
            SetColumn(treeInfo, 1);

            _currentViewer = type;

            _savedViewers[_currentViewer] = Children[0];

            _listController = directoryBrowser;
        }

        private void AddCommandText(Grid statusBarGrid)
        {
            if (_commandText.Parent != null)
                (_commandText.Parent as Grid).Children.Remove(_commandText);
            statusBarGrid.Children.Add(_commandText);
            _commandController = _commandText;
            SetRow(_commandText, 1);
        }

        private void AddCursor<T>(Canvas canvas, T directoryBrowser)
        {
            var cursor = ServiceLocator.FindService<IVIMListCursor>(directoryBrowser)();
            _motionController = cursor;
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

        public void Delete()
        {
            if (_currentViewer != "form") return;

            var persist = _savedViewers[_currentViewer] as IVIMPersistable;
            if (persist == null) return;
            persist.Delete();
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
    }
}