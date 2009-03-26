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

            var storageNode = XmlNode.Parse("<root><rowSelector/><textOutput/></root>");
            rootNode.Register<IParentNode>(new AggregateNode(xmlNode, storageNode));

            Func<IParentNode, IFieldAccessor<string>> fnGetDesc = node => node.Attribute("desc").Get<IFieldAccessor<string>>();
            Func<IParentNode, IFieldAccessor<string>> fnGetText = node => node.Attribute("body").Get<IFieldAccessor<string>>();
            Func<IEndNode, IFieldAccessor<string>> fnGetNewText = endNode => endNode.Parent.Attribute("body").Get<IFieldAccessor<string>>();
            Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes = node => node.Nodes("note");

            var contextDictionary = new Dictionary<string, object>();

            EntityList entityList;
            var rows = BuildNoteList(rootNode, fnGetNotes, fnGetDesc, container, contextDictionary, out entityList);
            var textDisplay = BuildTextDisplay(rootNode, container);
            var _entityListController = BuildEntitySelector(rootNode, contextDictionary, textDisplay, fnGetNewText, fnGetText);
            Initialize((IUIInitialize)textDisplay, entityList, rows);


            //TODO:  3. Enforce the constraint that new objects go through a factory where the INode is known at creation time, and a proxy is returned
            //This will eliminate a lot of the difficulty I've been having figuring out what to do when I new objects

            //TODO:  $$ Overall this is pretty awesome, but it can't respond to changes to the IFieldAccessor<string>, only to the column list of the IEntityRow
            return _entityListController;
        }

        private static void Initialize(IUIInitialize textDisplay, EntityList entityList, IEnumerable rows)
        {
//TODO:  I think maybe this should be a message that gets sent to the RootNode
            //And when that happens I won't need to cast text display and pass it in from above.  Or pass in any other stuff either.


            //Definitely a single line call
            entityList.Initialize();
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Parent = entityList);
            rows.OfType<IUIInitialize>().Do(ctrl => ctrl.Initialize());
            textDisplay.Initialize();
        }

        private static EntityListController BuildEntitySelector(RootNode rootNode, IDictionary<string, object> contextDictionary, IFieldAccessor<string> textDisplay, Func<IEndNode, IFieldAccessor<string>> fnGetNewText, Func<IParentNode, IFieldAccessor<string>> fnGetText)
        {
            //TODO:  I shouldn't have to pass in textDisplay, I can get it from the provider


            //TODO: entitySelector element spawns the EntitySelector class, and passes it to the EntityListController
            //It also creates an adaptor of type IFieldAccessor<IEntityRow> that lives *BETWEEN* EntitySelector and EntityListController,
                //which fires off events to each of the children underneath the entitySelector element
            //It would be nice if something in the framework would conver the [noteList] to EntityList (not the interface)
            //for the EntitySelector to do it's work.
            //Also note: there seems to be a difference between my EntitySelector class, and some class that would do the work just described


            //TODO:  Can I replace this dictionary and casting step with calls to Register<> and Get<>?  I think so.
            var entityListCast = (IEntityList)contextDictionary["noteList"];
            //TODO: This should  happen automatically through a register call... (see the rowSelector.Register below)
            var entitySelector = new EntitySelectorWrapper(new EntitySelector())
                                     {
                                         Rows = entityListCast.Rows,
                                         SelectedRow = entityListCast.Rows.First()
                                     };

      //bug: It's right here yo!
            var textOutput = rootNode.Nodes("textOutput").First().Get<IFieldAccessor<string>>();
            var rowSelector = rootNode.Nodes("rowSelector").First();
            rowSelector.Register<IEntitySelector>(entitySelector);
            var wrappedSelector = rowSelector.Get<IEntitySelector>();
            var nodeMessage = new NodeMessage
                                  {
                                      NodePredicate = node => node.GetType() == typeof (XmlNode),
                                      Target = textOutput,
                                      MessagePredicate = (message => message.Method.Name == "set_SelectedRow"),
                                      TargetDelegate = (Func<IEntityRow, Action<IFieldAccessor<string>>>)
                                                       (row => accessor => accessor.Value = fnGetText(row.Context).Value)
                                                       //typically this delegate will be of the form...
                                                       //Execute xpath from delegate on context, and what should be set at that node is the Provider
                                                       //TODO: 1. I'm registering the selector and the rows, but not the text target?
                                                       //This problem might have been avoided if I were thinking in terms of using the factory...
                                  };
            rootNode.InstallHook(nodeMessage);

            //TODO:  2. Get the TargetDelegate above to work and then remove this EntityListInterceptor.  Pass the wrappedSelector in directly
            var interceptor = new EntityListInterceptor(wrappedSelector, textDisplay, fnGetNewText);
            var _entityListController = new EntityListController {EntityList = interceptor};
            interceptor.SelectedRow = interceptor.Rows.First();
            return _entityListController;
        }

        private static IFieldAccessor<string> BuildTextDisplay(IParentNode rootNode, IContainer container)
        {
            var textOutput = rootNode.Nodes("textOutput").First();
            textOutput.Register<IFieldAccessor<string>>(new TextDisplay {Parent = container});
            return textOutput.Get<IFieldAccessor<string>>();
        }

        private static IEntityRow BuildEntityRow(IParentNode node, Func<IParentNode, IFieldAccessor<string>> fnGetDesc)
        {
            IEntityRow row = new EntityRow {Columns = new[] {fnGetDesc(node)}, Context = node};
            node.Register(row);
            return node.Get<IEntityRow>();
        }

        private static List<IEntityRow> BuildNoteList(IParentNode rootNode, Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes, Func<IParentNode, IFieldAccessor<string>> fnGetDesc, IContainer container, IDictionary<string, object> contextDictionary, out EntityList entityList)
        {
            var notesContext = fnGetNotes(rootNode);
            var rows = notesContext.Select(node => BuildEntityRow(node, fnGetDesc)).ToList();
            entityList = new EntityList {ID = "noteList", Parent = container, Rows = rows};
            contextDictionary["noteList"] = entityList;
            return rows;
        }
    }
}