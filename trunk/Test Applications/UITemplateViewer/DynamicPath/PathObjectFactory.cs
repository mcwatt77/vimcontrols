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
        private readonly Func<INode, INode, Type, object> _fnNodeBuilder;
        private readonly INode _attribute;
        private readonly Type _propertyType;

        public PathObjectFactory(Func<INode, INode, Type, object> fnNodeBuilder, INode attribute, Type propertyType)
        {
            _fnNodeBuilder = fnNodeBuilder;
            _attribute = attribute;
            _propertyType = propertyType;
        }

        public Func<INode, object> CreateFactory()
        {
            //if local point to data, then local retrieves the underlying template node
            //standard local behavior instantiates the object as a singleton from the template
            //If data pass the actual data, if virtualizable, wrap it (there might a flag to have it only wrap if it needs to)

            var get = _attribute.Get<IAccessor<string>>();
            var path = Decoder.FromPath(get.Value);
            return dataNode =>
                       {
                           if (get.Value == "[@rows][1]")
                           {
                               var firstExpr = path.Expressions.First();
                               var firstResult = firstExpr.Invoke(_attribute.Parent);
                               var objResult = _fnNodeBuilder((INode)firstResult, dataNode, typeof(object));
//                               throw new Exception("Debug from here.  For some reason what it's doing here is causing infinite recursion.");
                               return null;
                           }
                           //TODO: I think this will be needed to support certain orderings of properties
/*                            var existingObject = _attribute
                                .GetType()
                                .GetMethod("Get")
                                .MakeGenericMethod(_propertyType)
                                .Invoke(_attribute, new object[] {});*/

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
                                           return _fnNodeBuilder((INode)di, dataNode, _propertyType);
                                       }
                                       var newObj = _fnNodeBuilder((IParentNode)di, dataNode, typeof(object));
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
                               finalResult = result.Select(templateNode => _fnNodeBuilder(templateNode, dataNode, typeof(object)));
                           else
                           {
                               //TODO: if _propertyType does not have a nested IEnumerable and result.Count() > 1, Fail!

                               var fnData = path.Data.Compile();
                               var dataResult = fnData.DynamicInvoke(dataNode);
                               var eResult = ((IEnumerable) dataResult).Cast<IParentNode>();
                               finalResult = eResult.Select(e => _fnNodeBuilder(result.First(), e, typeof(object)));
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