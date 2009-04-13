using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeMessaging;
using UITemplateViewer.DynamicPath;

namespace UITemplateViewer.DynamicPath
{
    public class PathObjectFactory
    {
        private readonly Func<IParentNode, INode, object> _fnNodeBuilder;
        private readonly INode _attribute;
        private readonly Type _propertyType;

        public PathObjectFactory(Func<IParentNode, INode, object> fnNodeBuilder, INode attribute, Type propertyType)
        {
            _fnNodeBuilder = fnNodeBuilder;
            _attribute = attribute;
            _propertyType = propertyType;
        }

        public Func<INode, object> CreateFactory()
        {
            var get = _attribute.Get<IAccessor<string>>();
            var path = Decoder.FromPath(get.Value);
            return dataNode =>
                       {
                           if (path.Data != null)
                           {
                               var fnData = path.Data.Compile();
                               var dataResult = fnData.DynamicInvoke(dataNode);
                               if (dataResult is IEndNode)
                               {
                                   var endNode = (IEndNode) dataResult;
                                   var accessor = endNode.Get<IAccessor<string>>();
                                   if (path.Data.ToString() == "a => a.Attribute(\"desc\")")
                                   {
                                       return accessor.Value;
                                       //                                   var data = dataResult.Get<IAccessor<string>>();
                                       var parent = (IParentNode) dataNode;
                                       var val = parent.Attribute("descr");
                                       int debug = 0;
                                   }
                               }
                           }
                           if (path.Local == null) return null;

                           var fnLocal = path.Local.Compile();
                           var di = fnLocal.DynamicInvoke(_attribute.Parent);
                           if (!typeof(IEnumerable<IParentNode>).IsAssignableFrom(di.GetType()))
                           {
                               //TODO: Implement this.  This is [:selector/@rowSelector]{@body}
                               return null;
                           }
                           var result = (IEnumerable<IParentNode>)fnLocal.DynamicInvoke(_attribute.Parent);
                           //TODO: instead of this it should actually be for each dataNode as well

                           IEnumerable<object> finalResult;
                           if (path.Data == null)
                               finalResult = result.Select(templateNode => _fnNodeBuilder(templateNode, dataNode));
                           else
                           {
                               //TODO: if _propertyType does not have a nested IEnumerable and result.Count() > 1, Fail!

                               var fnData = path.Data.Compile();
                               var dataResult = fnData.DynamicInvoke(dataNode);
                               var eResult = ((IEnumerable) dataResult).Cast<IParentNode>();
                               finalResult = eResult.Select(e => _fnNodeBuilder(result.First(), e));
//                               finalResult = result.Select(templateNode => _fnNodeBuilder(templateNode, dataNode));
                           }
                           finalResult = finalResult.ToList();

                           if (!_propertyType.IsAssignableFrom(finalResult.GetType()))
                           {
                               if (typeof(IEnumerable).IsAssignableFrom(_propertyType))
                               {
                                   //I can use supercast
                                   var enumCastType = _propertyType.GetGenericArguments().First();
                                   var castMethod = typeof(Enumerable).GetMethod("Cast");
                                   castMethod = castMethod.MakeGenericMethod(enumCastType);
                                   var newFinalResult = castMethod.Invoke(null, new[] { finalResult });
                                   return newFinalResult;
                               }
                               var fields = finalResult.Cast<UITemplateViewer.WPF.IField>();
                           }
//                           fnNodeBuilder((IParentNode)result, dataNode);

                           //now create a new object

                           return result;
                       };
        }
    }
}