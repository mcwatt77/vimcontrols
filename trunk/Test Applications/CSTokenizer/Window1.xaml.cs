using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using KeyStringParser;
using Model;

namespace CSTokenizer
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = @"..\..\Window1.xaml.cs";
                var fileInfo = new FileInfo(fileName);
                var codeToParse = fileInfo.OpenText().ReadToEnd();
//            var codeToParse = "using sample; //spam!\r\nusing system;";

                var handler = new Handler();
                codeToParse.Do(handler.Process);
                handler.Flush();

                var tokens = new List<Token>();
                while (handler.Count() > 0)
                    tokens.Add(handler.Pop());

                var doc = new XDocument(new XElement("root"));
                tokens.Do(token => doc.Root.Add(new XElement("token", new XAttribute("data", token))));

                var parents = new Stack<object>();
                var codeListing = new CodeListing {Assembly = new Assembly()};
                parents.Push(codeListing);

                var currentTokenList = new List<Token>();
                var i = -1;
                foreach(var token in tokens)
                {
                    i++;
                    if (token.Characters == ";")
                    {
                        ProcessStatement(currentTokenList, parents.Peek());
                        currentTokenList.Clear();
                        continue;
                    }
                    if (token.Characters == "{")
                    {
                        parents.Push(ProcessParent(currentTokenList, parents.Peek()));
                        currentTokenList.Clear();
                        continue;
                    }
                    if (token.Characters == "}")
                    {
                        parents.Pop();
                        currentTokenList.Clear();
                        continue;
                    }

                    currentTokenList.Add(token);
                }

                MessageBox.Show(doc.ToString().Substring(0, 200));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\r\n\r\n" + exception.StackTrace);
            }

            Close();
        }

        private static IEnumerable<Template> _registeredTemplates = RegisterTemplates();

        private static IEnumerable<Template> RegisterTemplates()
        {
            yield return new UsingTemplate();
            yield return new NamespaceTemplate();
            yield return new ClassTemplate();
            yield return new MethodTemplate();
        }

        private static object ProcessParent(IEnumerable<Token> tokens, object peek)
        {
            var newObject = _registeredTemplates
                .Select(template => CheckTemplate(template, tokens, peek))
                .FirstOrDefault(newObj => newObj != null);
            return newObject ?? peek;
        }

        private static object CheckTemplate(Template template, IEnumerable<Token> tokens, object peek)
        {
            if (template.Matches(tokens.ToArray(), peek))
                return template.Process(tokens.ToArray(), peek);
            return null;
        }

        private static void ProcessStatement(IEnumerable<Token> tokens, object peek)
        {
            _registeredTemplates.Any(template => CheckTemplate(template, tokens, peek) != null);
        }
    }

    public class CharacterType
    {
        private readonly string _display;

        public CharacterType(string display)
        {
            _display = display;
        }

        public override string ToString()
        {
            return _display;
        }
    }
}