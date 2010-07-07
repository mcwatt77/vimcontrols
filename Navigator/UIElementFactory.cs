using System;
using Navigator.Containers;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    public class UIElementFactory : IUIElementFactory
    {
        private readonly IContainer _container;
        //TODO: Eliminate this by using a container to inject the appropriate overriden value?
        public IUIElementFactory FactoryForChildren { get; set; }

        public UIElementFactory(IContainer container)
        {
            _container = container;
        }

        public IUIElement GetUIElement(object modelElement)
        {
            if (typeof(IHasUrl).IsAssignableFrom(modelElement.GetType())
                && typeof(ISummaryString).IsAssignableFrom(modelElement.GetType()))
            {
                var summaryString = (ISummaryString) modelElement;
                var url = (IHasUrl) modelElement;
                return new StringWithUrlNavigation(summaryString.Summary, url.Url);
            }
            if (typeof(IModelChildren).IsAssignableFrom(modelElement.GetType())
                && typeof(ISummaryString).IsAssignableFrom(modelElement.GetType()))
            {
                var summaryString = (ISummaryString)modelElement;
                return new StringWithChildrenElement(summaryString.Summary, (IModelChildren)modelElement, FactoryForChildren ?? this);
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