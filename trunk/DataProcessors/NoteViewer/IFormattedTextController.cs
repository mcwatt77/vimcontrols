using System.Collections.Generic;
using System.Windows.Media;

namespace DataProcessors.NoteViewer
{
    public interface IFormattedTextController
    {
        int FirstRow { get; set; }
        double Height { get; set; }
        IEnumerable<FormattedText> RowsByHeight();
    }
}