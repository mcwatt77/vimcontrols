using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Interceptor;
using NodeMessaging;
using UITemplateViewer.Controllers;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;
using Utility.Core;

namespace UITemplateViewer
{
    public class DynamicTemplate
    {
        private readonly Dictionary<string, IParentNode> _nodeLookups = new Dictionary<string, IParentNode>();
        private XPathDecoder _xpath;

        private void ProcessTemplateElem(IParentNode dataNode, IParentNode templateNode)
        {
            var elem = templateNode.Nodes().First();

            //find a class that matches the elem name
            var types = typeof (DynamicTemplate)
                .Assembly
                .GetTypes()
                .Where(type => type.Name.ToLower() == elem.Name.ToLower());

            if (types.Count() != 1) throw new Exception("Found " + types.Count() + " of type " + elem.Name);

            var newType = types.Single();
            var newObj = newType.GetConstructor(Type.EmptyTypes).Invoke(new object[] {});

            elem.Attributes().Do(attr => ProcessAttribute(dataNode, elem, attr, newObj));
        }

        private void ProcessAttribute(IParentNode data, IParentNode parent, INode node, object newObj)
        {
            var propToSet = newObj.GetType().GetProperties().SingleOrDefault(prop => prop.Name.ToLower() == node.Name.ToLower());
            var get = node.Get<IFieldAccessor<string>>();

            var dynamicPath = DynamicPathDecoder.FromPath(get.Value);
            if (node.Name == "id")
            {
                _nodeLookups[get.Value] = parent;
                return;
            }
            if (get.Value[0] != '[' && get.Value[0] != '{')
            {
                if (propToSet != null)
                {
                    propToSet.SetValue(newObj, get.Value, null);
                }
                return;
            }

            var path = get.Value.Substring(1, get.Value.Length - 2);
            var method = _xpath.GetType().GetMethods().Single(lmethod => lmethod.Name == "GetPathFunc" && !lmethod.IsGenericMethod);
            return;
            var result = (Delegate)method.Invoke(_xpath, new object[] {path});
            var invokeResult = result.DynamicInvoke(data);

            if (typeof(IEnumerable).IsAssignableFrom(invokeResult.GetType()))
            {
                var nodes = ((IEnumerable) invokeResult).Cast<INode>();
                //try to pack this into the propToSet

                int debug2 = 0;
            }

            //NOTE:  This is the current design path... but as soon as I pass in data...
            //I tie the whole system up in dynamic runtime execution.
            //Would be much nicer if I could return delegates to operate later
            //NOTE:  I might be able to an inversion type thing, where instead of passing in data, I pass in a delegate to data

            //what I have here is the result of fnGetNotes
            //TODO: 1. Now I need to build EntityRows out of them
//            _xpath.GetPathFunc<>()

            //The value of invokeResult is whatever gets returned by xpath, which should either be IEnum<Node> or Node...
            //Should I always make it IEnum?
            int debug = 0;
        }

        //TODO: 2. $$$ Replace this method with the automatic template reading above
        private static IUIEntityRow BuildEntityRow(IParentNode node, Func<IParentNode, IFieldAccessor<string>> fnGetDesc)
        {
            IEntityRow row = new EntityRow {Columns = new[] {fnGetDesc(node)}, Context = node};
            node.Register(row);
            return node.Get<IUIEntityRow>();
        }

        private static void BuildNoteList(IParentNode rootNode, Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes, Func<IParentNode, IFieldAccessor<string>> fnGetDesc, IContainer container, out EntityList entityList)
        {
            var notesContext = fnGetNotes(rootNode);
            var rows = notesContext.Take(2).Select(node => BuildEntityRow(node, fnGetDesc)).ToList();
            entityList = new EntityList {Parent = container, Rows = rows};
            rootNode.Register(entityList);
        }

        public object InitializeController(IContainer container)
        {
            _xpath = new XPathDecoder();
            var rootNode = new RootNode();

            var template = XDocument.Load(@"..\..\templates\noteviewer.xml");
            if (template.Root == null) throw new Exception("Invalid xml document");

            var xmlNode = XmlNode.Parse("<data><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></data>");
            //TODO: This needs to be generated dynamically somehow
            var storageNode = XmlNode.Parse("<dynamicData><rowSelector/><textOutput/></dynamicData>");
            var templateNode = XmlNode.Parse(template.ToString());

            rootNode.Register<IParentNode>(new AggregateNode(xmlNode, storageNode, templateNode));

            var dataRoot = rootNode.Nodes("data").First();
            var dynamicData = rootNode.Nodes("dynamicData").First();
            var uiRoot = rootNode.Nodes().Skip(2).First();
            template.Root.Elements().Do(elem => ProcessTemplateElem(dataRoot, uiRoot));


//            var fnGetDesc = _xpath.GetPathFunc<IFieldAccessor<string>>("@descr");
//            var fnGetText = _xpath.GetPathFunc<IFieldAccessor<string>>("@body");
            Func<IParentNode, IFieldAccessor<string>> fnGetDesc = node => node.Attribute("desc").Get<IFieldAccessor<string>>();
            Func<IParentNode, IFieldAccessor<string>> fnGetText = node => node.Attribute("body").Get<IFieldAccessor<string>>();
            var fnGetNotes = _xpath.GetPathFunc<IEnumerable<IParentNode>>("//note");


            EntityList entityList;
            BuildNoteList(dataRoot, fnGetNotes, fnGetDesc, container, out entityList);
            var textDisplay = BuildTextDisplay(dynamicData, container);
            var _entityListController = BuildEntitySelector(dataRoot, fnGetText, rootNode, dynamicData);
            Initialize((IUIInitialize)textDisplay, entityList);


            //TODO:  3. Enforce the constraint that new objects go through a factory where the INode is known at creation time, and a proxy is returned
            //This will eliminate a lot of the difficulty I've been having figuring out what to do when I new objects

            //TODO:  $$ Overall this is pretty awesome, but it can't respond to changes to the IFieldAccessor<string>, only to the column list of the IEntityRow
            return _entityListController;
        }

        private static void Initialize(IUIInitialize textDisplay, IUIInitialize entityList)
        {
//TODO:  I think maybe this should be a message that gets sent to the RootNode
            //And when that happens I won't need to cast text display and pass it in from above.  Or pass in any other stuff either.

/*            var msg = Message.Create<IUIInitialize>(ui => ui.Initialize());
            rootNode.Send(msg);*/

            //should send the message to all top level controls

            //Definitely a single line call
            entityList.Initialize();
            textDisplay.Initialize();
        }

        private static EntityListController BuildEntitySelector(INode rootNode, Func<IParentNode, IFieldAccessor<string>> fnGetText, RootNode actualRootNode, IParentNode dynamicData)
        {
            var entityList = rootNode.Get<IEntityList<IUIEntityRow>>();
            var selector = new EntitySelector {Rows = entityList.Rows, SelectedRow = entityList.Rows.First()};
            var rowSelector = dynamicData.Nodes("rowSelector").First();
            rowSelector.Register(selector);

            var textOutput = dynamicData.Nodes("textOutput").First().Get<IFieldAccessor<string>>();
            var nodeMessage = new NodeMessage
                                  {
                                      NodePredicate = node => node.GetType() == typeof (XmlNode),
                                      Target = textOutput,
                                      MessagePredicate = (message => message.Method.Name == "set_SelectedRow"),
                                      TargetDelegate = (Func<IInvocation, Action<IFieldAccessor<string>>>)
                                                       (row => accessor => accessor.Value = fnGetText(((IEntityRow)row.Arguments.First()).Context).Value)
                                                       //TODO: 1. This seems a little complicated...
                                                       //Somebody, or something in the xml needs to clue the framework in to how to do this wiring.
                                  };
            actualRootNode.InstallHook(nodeMessage);

            var _entityListController = new EntityListController {EntityList = rowSelector.Get<IEntitySelector>()};
            _entityListController.Beginning();

            return _entityListController;
        }

        private static IFieldAccessor<string> BuildTextDisplay(IParentNode rootNode, IContainer container)
        {
            var textOutput = rootNode.Nodes("textOutput").First();
            textOutput.Register<IFieldAccessor<string>>(new TextDisplay {Parent = container});
            return textOutput.Get<IFieldAccessor<string>>();
        }
    }
}