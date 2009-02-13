using System.Windows;
using System.Windows.Controls;
using ActionDictionary.Interfaces;

namespace AppViewer
{
    public class UnimplementedControl : IAppControl
    {
        public UIElement GetControl()
        {
            return new TextBlock {Text = "Not yet implemented"};
        }
    }

    //TODO: Make a single base class that uses children that are capable of rendering their own screen space??
    //as long as those classes are still testable...
    public class ProjectTrackerControl : UnimplementedControl{}
    public class KeyboardProcessorTestControl : UnimplementedControl{}
    public class CommandStackControl : UnimplementedControl{}
    public class FilesControl : UnimplementedControl{}
    public class AudioControl : UnimplementedControl{}
    public class MoviesControl : UnimplementedControl{}
    public class ObjectsControl : UnimplementedControl{}
    public class NotesControl : UnimplementedControl{}
    public class GrapherControl : UnimplementedControl{}
    public class Sketch3dControl : UnimplementedControl{}
}