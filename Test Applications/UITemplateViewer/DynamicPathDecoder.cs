namespace UITemplateViewer
{
    public class DynamicPathDecoder
    {
        public static DynamicPathDecoder FromPath(string path)
        {
            // [*]{/n ote}                   Make a copy of each child, for each /n ote, and execute that child from the context of that /n ote
            // [*]                           Put an IEnum of children into this attribute
            // {@descr}                      Get the descr attribute from the data
            // {/dyn::rowSelector}           Get access to the rowSeletor dynamicData
            // [:noteList/@rows]             Find the local element with noteList id, and return the value in it's rows property
            // [../@rowSelector]{@body}      For the rowSelector property on this object, get the body attribute from it

            //TODO: 1.  The above are the different pathes I'm using at the moment, and represent an overview of what I need to support
            return new DynamicPathDecoder();
        }
    }

    public class MapPathDecoder { }
    public class DataContextDecoder { }
    public class MixedDecoder { }
    public class LiteralDecoder { }
}