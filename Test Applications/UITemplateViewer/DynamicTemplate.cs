using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Interceptor;
using NodeMessaging;
using UITemplateViewer.Controllers;
using UITemplateViewer.DynamicPath;
using UITemplateViewer.Element;
using UITemplateViewer.WPF;
using Utility.Core;

namespace UITemplateViewer
{
    public class DynamicTemplate
    {
        private readonly Dictionary<string, IParentNode> _nodeLookups = new Dictionary<string, IParentNode>();
        private XPathDecoder _xpath;

        private object ProcessTemplateElem(IParentNode dataNode, IParentNode templateNode)
        {
            //create an object based on the template node, and insert it in the template node

            var nodeTypes = typeof (DynamicTemplate)
                .Assembly
                .GetTypes()
                .Where(type => type.Name.ToLower() == templateNode.Name.ToLower());

            if (nodeTypes.Count() != 1) throw new Exception("Found " + nodeTypes.Count() + " of type " + templateNode.Name);

            var newType = nodeTypes.Single();
            var newObj = newType.GetConstructor(Type.EmptyTypes).Invoke(new object[] {});

            templateNode.Attributes().Do(attr => ProcessAttribute(dataNode, templateNode, attr, newObj));

            //TODO: now do child elements of the templateNode
            //each element in the template will have data registered to it,
            //even if the parent never explicitly calls it.
            //A later optimization step could cull this out

            return newObj;
        }

        private void ProcessAttribute(IParentNode data, IParentNode parent, INode node, object newObj)
        {
            var propToSet = newObj.GetType().GetProperties().SingleOrDefault(prop => prop.Name.ToLower() == node.Name.ToLower());
            var get = node.Get<IAccessor<string>>();

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

            var path = Decoder.FromPath(get.Value);
            //if both local and data are set, this means that each of the children in Local gets each instance of Data
            if (path.Local != null && path.Data != null)
            {
                //TODO: Get all of this expression processing into a separate class.
                //Might even wrap some of the functionality in a localized class
                var fnData = path.Data.Compile();
                var dataRows = fnData.DynamicInvoke(data);

                var fnLocal = path.Local.Compile();
                var localRows = ((IEnumerable)fnLocal.DynamicInvoke(parent)).Cast<IParentNode>();
                var localRow = localRows.Single();

                var isEnumerable = typeof (System.Collections.IEnumerable).IsAssignableFrom(dataRows.GetType());
                var propIsEnumerable = typeof (System.Collections.IEnumerable).IsAssignableFrom(propToSet.PropertyType);
                if (propIsEnumerable && isEnumerable)
                {
                    //I should cast here... cause I can
                    var innerType = propToSet.PropertyType.GetGenericArguments().First();
                    var e = ((System.Collections.IEnumerable) dataRows).Cast<IParentNode>();
                    foreach (var row in e)
                    {
                        ProcessTemplateElem(row, localRow);
                        //call back to ProcessTemplateElem?
                        //ultimately need to create an object of type innerType, with the context of row
                        int debug2 = 0;
                    }
                }
                if (!isEnumerable)
                {
                }

            }
            if (path.Local != null)
            {
                var fnLocal = path.Local.Compile();
                var localRows = ((IEnumerable)fnLocal.DynamicInvoke(parent)).Cast<IParentNode>();

                var dataRows = localRows.Select(localRow => ProcessTemplateElem(data, localRow)).ToList();

                var e = dataRows.Cast<IUIInitialize>().ToList();
                node.Register(e);

                var child = node.Get<IEnumerable<IUIInitialize>>();
                propToSet.SetValue(newObj, child, null);

                int debug = 0;
                //now set this to propToSet
            }
            return;
        }

        private object CreateObject(Type type, INode node)
        {
            return null;
        }

        //TODO: 2. $$$ Replace this method with the automatic template reading above
        private static IUIEntityRow BuildEntityRow(IParentNode node, Func<IParentNode, IAccessor<string>> fnGetDesc)
        {
            IEntityRow row = new EntityRow {Columns = new[] {fnGetDesc(node)}, Context = node};
            node.Register(row);
            return node.Get<IUIEntityRow>();
        }

        private static void BuildNoteList(IParentNode rootNode, Func<IParentNode, IEnumerable<IParentNode>> fnGetNotes, Func<IParentNode, IAccessor<string>> fnGetDesc, IContainer container, out EntityList entityList)
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

            rootNode.Register<IParentNodeImplementor>(new AggregateNode(xmlNode, storageNode, templateNode));

            var dataRoot = rootNode.Nodes("data").First();
            var dynamicData = rootNode.Nodes("dynamicData").First();
            var uiRoot = rootNode.Nodes().Skip(2).First();
            //bug: here it is...  this needs to be implemented.  Will want to switch EndNodeWrapper to derive from NodeBase to fix
//            ProcessTemplateElem(dataRoot, uiRoot);


//            var fnGetDesc = _xpath.GetPathFunc<IAccessor<string>>("@descr");
//            var fnGetText = _xpath.GetPathFunc<IAccessor<string>>("@body");
            Func<IParentNode, IAccessor<string>> fnGetDesc = node => node.Attribute("desc").Get<IAccessor<string>>();
            Func<IParentNode, IAccessor<string>> fnGetText = node => node.Attribute("body").Get<IAccessor<string>>();
            var fnGetNotes = _xpath.GetPathFunc<IEnumerable<IParentNode>>("//note");


            EntityList entityList;
            BuildNoteList(dataRoot, fnGetNotes, fnGetDesc, container, out entityList);
            var textDisplay = BuildTextDisplay(dynamicData, container);
            var _entityListController = BuildEntitySelector(dataRoot, fnGetText, rootNode, dynamicData);
            Initialize((IUIInitialize)textDisplay, entityList);


            //TODO:  3. Enforce the constraint that new objects go through a factory where the INode is known at creation time, and a proxy is returned
            //This will eliminate a lot of the difficulty I've been having figuring out what to do when I new objects

            //TODO:  $$ Overall this is pretty awesome, but it can't respond to changes to the IAccessor<string>, only to the column list of the IEntityRow
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

        private static EntityListController BuildEntitySelector(INode rootNode, Func<IParentNode, IAccessor<string>> fnGetText, RootNode actualRootNode, IParentNode dynamicData)
        {
            var entityList = rootNode.Get<IEntityList<IUIEntityRow>>();
            var selector = new EntitySelector {Rows = entityList.Rows, SelectedRow = entityList.Rows.First()};
            var rowSelector = dynamicData.Nodes("rowSelector").First();
            rowSelector.Register(selector);

            var textOutput = dynamicData.Nodes("textOutput").First().Get<IAccessor<string>>();
            var nodeMessage = new NodeMessage
                                  {
                                      Target = textOutput,
                                      MessagePredicate = (message => message.Method.Name == "set_SelectedRow"),
                                      TargetDelegate = (Func<IInvocation, Action<IAccessor<string>>>)
                                                       (row => accessor => accessor.Value = fnGetText(((IEntityRow)row.Arguments.First()).Context).Value)
                                                       //TODO: 1. This seems a little complicated...
                                                       //Somebody, or something in the xml needs to clue the framework in to how to do this wiring.
                                  };
            actualRootNode.InstallHook(nodeMessage);

            var _entityListController = new EntityListController {EntityList = rowSelector.Get<IEntitySelector>()};
            _entityListController.Beginning();

            return _entityListController;
        }

        private static IAccessor<string> BuildTextDisplay(IParentNode rootNode, IContainer container)
        {
            var textOutput = rootNode.Nodes("textOutput").First();
            textOutput.Register<IAccessor<string>>(new TextDisplay {Parent = container});
            return textOutput.Get<IAccessor<string>>();
        }
    }
}