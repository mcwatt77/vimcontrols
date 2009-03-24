using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeMessaging;
using UITemplateViewer.Controllers;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;
using Utility.Core;

namespace UITemplateViewer
{
    public class DynamicTemplate
    {
        public object InitializeController(IContainer container)
        {
            var xmlNode = XmlNode.Parse("<root><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></root>");
            var rootNode = new RootNode();
            rootNode.Register<IParentNode>(xmlNode);

            var storageNode = XmlNode.Parse("<root><selectedRow/></root>");
            rootNode.Register<IParentNode>(new AggregateNode(xmlNode, storageNode));

            Func<IParentNode, IFieldAccessor<string>> fnGetDesc = node => node.Attribute("desc").Get<IFieldAccessor<string>>();
            Func<IEndNode, IFieldAccessor<string>> fnGetNewText = endNode => endNode.Parent.Attribute("body").Get<IFieldAccessor<string>>();
            Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes = node => node.Nodes("note");

            var contextDictionary = new Dictionary<string, object>();

            EntityList entityList;
            var rows = BuildNoteList(rootNode, fnGetNotes, fnGetDesc, container, contextDictionary, out entityList);
            var textDisplay = BuildTextDisplay(container);
            var _entityListController = BuildEntitySelector(rootNode, contextDictionary, textDisplay, fnGetNewText);
            Initialize(textDisplay, entityList, rows);

            //TODO:  $$ Overall this is pretty awesome, but it can't respond to changes to the IFieldAccessor<string>, only to the column list of the IEntityRow
            return _entityListController;
        }

        private static void Initialize(IUIInitialize textDisplay, EntityList entityList, IEnumerable rows)
        {
//TODO:  I think maybe this should be a message that gets sent to the RootNode
            //Definitely a single line call
            entityList.Initialize();
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Parent = entityList);
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Initialize());
            textDisplay.Initialize();
        }

        private static EntityListController BuildEntitySelector(RootNode rootNode, IDictionary<string, object> contextDictionary, IFieldAccessor<string> textDisplay, Func<IEndNode, IFieldAccessor<string>> fnGetNewText)
        {
            //TODO: entitySelector element spawns the EntitySelector class, and passes it to the EntityListController
            //It also creates an adaptor of type IFieldAccessor<IEntityRow> that lives *BETWEEN* EntitySelector and EntityListController,
                //which fires off events to each of the children underneath the entitySelector element
            //It would be nice if something in the framework would conver the [noteList] to EntityList (not the interface)
            //for the EntitySelector to do it's work.
            //Also note: there seems to be a difference between my EntitySelector class, and some class that would do the work just described


            var entityListCast = (IEntityList)contextDictionary["noteList"];
            var entitySelector = new EntitySelector {Rows = entityListCast.Rows, SelectedRow = entityListCast.Rows.First()};
            var accessor = new DelegateFieldAccessor<IEntityRow>(
                () => entitySelector.SelectedRow,
                row => { entitySelector.SelectedRow = row; });
            rootNode.Nodes("selectedRow").First().Register<IFieldAccessor<IEntityRow>>(accessor);
                //this IFieldAccessor should attach to the SelectedRow in the IEntityList
            rootNode.InstallHook(new NodeMessage());//This ultimately should be what fires off changes to textDisplay, not the interceptor below

            //TODO: rather than being attached to the description, the interceptor should be attached to IEntityRow.Context

            //TODO: IEntityRow needs to have an IParentNode packed inside it, rather than have it as a property

            //TODO: Replace this whole class with an interceptor call
            var interceptor = new EntityListInterceptor(entitySelector, textDisplay, fnGetNewText);
            var _entityListController = new EntityListController {EntityList = interceptor};
            interceptor.SelectedRow = interceptor.Rows.First();
            return _entityListController;
        }

        private static TextDisplay BuildTextDisplay(IContainer container)
        {
            return new TextDisplay {Parent = container};
        }

        private static IEntityRow BuildEntityRow(IParentNode node, Func<IParentNode, IFieldAccessor<string>> fnGetDesc)
        {
            IEntityRow row = new EntityRow {Columns = new[] {fnGetDesc(node)}, Context = node};
            //save row into the framework

//            var generator = new ProxyGenerator();
//            var proxy = generator.CreateInterfaceProxyWithTarget(typeof (IEntityRow), new[] {typeof (IParentNode)}, row, new FinalI
            //TODO: add some sort of framework support into NodeMessaging that either does this packing somewhere,
                //or allows me to do it here
            //also use that same call within the NodeMessaging component
            //and remove the reference to Castle here

            return row;
        }

        private static List<IEntityRow> BuildNoteList(IParentNode rootNode, Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes, Func<IParentNode, IFieldAccessor<string>> fnGetDesc, IContainer container, IDictionary<string, object> contextDictionary, out EntityList entityList)
        {
            var notesContext = fnGetNotes(rootNode);
            //TODO: I feel like I need to replace this with something that sets this into the Data
            var rows = notesContext.Select(node => BuildEntityRow(node, fnGetDesc)).ToList();
            entityList = new EntityList {ID = "noteList", Parent = container, Rows = rows};
            contextDictionary["noteList"] = entityList;
            return rows;
        }
    }
}