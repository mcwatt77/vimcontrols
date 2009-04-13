using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using NodeMessaging;
using UITemplateViewer.DynamicPath;

namespace UITemplateViewer.DynamicPath
{
    public class PathObjectFactory
    {
        //TODO:  This should be more like a factory type implementation, where sometimes it's new,
            //sometimes it retrieves existing instances, but it needs to maintain unique identity of requests
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
                                   }
                               }
                           }
                           if (path.Local == null) return null;

                           if (path.Local.ToString() == "a => a.Root.NodeById(\"controller\")")
                           {
                               int debug = 0;
                           }

                           var fnLocal = path.Local.Compile();
                           var di = fnLocal.DynamicInvoke(_attribute.Parent);
                           if (!typeof(IEnumerable<IParentNode>).IsAssignableFrom(di.GetType()))
                           {
                               if (typeof(INode).IsAssignableFrom(di.GetType()))
                               {
                                   //take di, and call _fnNodeBuilder
                                   if (path.Data == null)
                                   {
                                       if (typeof(IEndNode).IsAssignableFrom(di.GetType()))
                                       {
                                           int debug = 0;
                                           return null;
                                       }
                                       var newObj = _fnNodeBuilder((IParentNode)di, dataNode);
                                       if (!_propertyType.IsAssignableFrom(newObj.GetType()))
                                       {
                                           var proxy = new ProxyGenerator();
                                           return proxy.CreateInterfaceProxyWithoutTarget(_propertyType, new SuperCast(newObj));
                                       }
                                       return newObj;
                                   }
                               }
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