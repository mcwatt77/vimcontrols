using System;
using System.Linq;
using Navigator.Containers;
using Navigator.UI;
using Navigator.UI.Attributes;
using Utility.Core;
using VIControls.Commands.Interfaces;

namespace Navigator
{
    //TODO: Do something with this class
    public class TypeProfile
    {
        private readonly object _object;

        public TypeProfile(object @object)
        {
            _object = @object;
        }

        public bool ImplementsInterface(Type type)
        {
            if (type.IsGenericType)
            {
                return _object
                    .GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(t => t.GetGenericTypeDefinition() == type);
            }
            return type.IsAssignableFrom(_object.GetType());
        }

        public bool ImplementsAllInterfaces(params Type[] types)
        {
            return types.All(ImplementsInterface);
        }

        public bool HasTypeInConstructor(Type type)
        {
            return _object
                .GetType()
                .ChainWithSelf(o => o.BaseType)
                .Select(o =>
                        o.GetConstructors()
                            .Select(constructor => constructor
                                                       .GetParameters()
                                                       .Any(parameter => type.IsAssignableFrom(parameter.ParameterType)))
                            .Any(b => b))
                .Any(b => b);
        }
    }

    public class UIElementFactory : IUIElementFactory
    {
        private readonly IContainer _container;

        public UIElementFactory(IContainer container)
        {
            _container = container;
        }

        //TODO: Complete the conversion to using TypeProfile instead of directoy IsAssignableFrom calls
        public IUIElement GetUIElement(object modelElement)
        {
            var typeProfile = new TypeProfile(modelElement);

            if (typeProfile.ImplementsAllInterfaces(typeof(IHasRows<>), typeof(ISummaryString)))
            {
                var typeofRowList = modelElement
                    .GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Single(t => t.GetGenericTypeDefinition() == typeof (IHasRows<>));

                var tableToMake = typeof (TableViewier<>)
                    .MakeGenericType(typeofRowList.GetGenericArguments().Single());

                var summary = (ISummaryString) modelElement;

                return (IUIElement) Activator.CreateInstance(tableToMake, summary.Summary, modelElement);
            }
            if (typeProfile.ImplementsAllInterfaces(typeof(IHasUrl), typeof(ISummaryString)))
            {
                var summaryString = (ISummaryString) modelElement;
                var url = (IHasUrl) modelElement;
                return new StringWithUrlNavigation(summaryString.Summary, url.Url);
            }
            if (typeProfile.ImplementsAllInterfaces(typeof(IHasXml)))
            {
                var xml = (IHasXml) modelElement;
                return new HtmlToWPF(xml.Xml);
            }
            if (typeof(IModelChildren).IsAssignableFrom(modelElement.GetType())
                && typeof(ISummaryString).IsAssignableFrom(modelElement.GetType()))
            {
                var summaryString = (ISummaryString)modelElement;
                return new StringWithChildrenElement(summaryString.Summary, (IUIChildren)modelElement);
            }
            if (typeof(IDescriptionString).IsAssignableFrom(modelElement.GetType())
                && typeof(ISummaryString).IsAssignableFrom(modelElement.GetType())
                && typeof(IMessageable).IsAssignableFrom(modelElement.GetType().BaseType))
            {
                var summaryString = (ISummaryString) modelElement;
                var descriptionString = (IDescriptionString) modelElement;
                var insertMode = _container.Get<IHasInsertMode>();
                return new UpdateableStringSummaryElement(insertMode, () => summaryString.Summary, () => descriptionString.Description);
            }
            if (typeof(IDescriptionString).IsAssignableFrom(modelElement.GetType())
                && typeof(ISummaryString).IsAssignableFrom(modelElement.GetType()))
            {
                var summaryString = (ISummaryString) modelElement;
                var descriptionString = (IDescriptionString) modelElement;
                return new StringSummaryElement(summaryString.Summary, descriptionString.Description);
            }
            if (typeof(IFileViewer).IsAssignableFrom(modelElement.GetType()))
            {
                return new FileElement(((IFileViewer)modelElement).File);
            }
            throw new InvalidOperationException("Couldn't find an IUIElement to represent the IModelElement");
        }
    }
}