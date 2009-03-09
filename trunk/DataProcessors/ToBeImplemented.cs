using AppControlInterfaces.TextView;

namespace DataProcessors
{
    public class ToBeImplemented : ITextViewData
    {
        private readonly string _text;

        protected ToBeImplemented()
        {
            _text = "Not yet implemented";
        }

        protected ToBeImplemented(string text)
        {
            _text = text;
        }

        public string GetData()
        {
            return _text;
        }
    }

    [Launchable("Project Tracker")]
    public class ProjectTrackerControl : ToBeImplemented{public ProjectTrackerControl():base("This will track projects"){}}
    [Launchable("Keyboard Processor Test")]
    public class KeyboardProcessorTestControl : ToBeImplemented{}
    [Launchable("Command stack")]
    public class CommandStackControl : ToBeImplemented{}
    [Launchable("Files")]
    public class FilesControl : ToBeImplemented{}
    [Launchable("Audio")]
    public class AudioControl : ToBeImplemented{}
    [Launchable("Object Viewer")]
    public class ObjectsControl : ToBeImplemented{}
    [Launchable("Grapher")]
    public class GrapherControl : ToBeImplemented{}
    [Launchable("3D sketcher")]
    public class Sketch3dControl : ToBeImplemented{}
}
