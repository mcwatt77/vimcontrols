using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Utility.Core
{
    public class PathParser
    {
        private readonly Dictionary<string, string> _parseDictionary;
        public PathParser(TextReader reader)
        {
            _parseDictionary = CreateParseDictionary(reader.ReadToEnd());
        }

        private static Dictionary<string, string> CreateParseDictionary(string parse)
        {
            var matches = Regex.Matches(parse,
                @"^\s*((?<id>\w*)|({(?<agid>\w+)}))\s*->\s*\r\n(?<body>.*)\r\n\s*\$\s*$",
                RegexOptions.Multiline | RegexOptions.ExplicitCapture);

            var localDict = matches.Cast<Match>()
                .Where(match => match.Groups["agid"].Value.Length > 0)
                .ToDictionary(match => match.Groups["agid"].Value, match => match.Groups["body"].Value);

            var dict = matches.Cast<Match>()
                .Where(match => match.Groups["agid"].Value.Length == 0)
                .ToDictionary(match => match.Groups["id"].Value, match => match.Groups["body"].Value);

            while (ExpandExpressions(localDict, localDict)) { }
            ExpandExpressions(dict, localDict);
            return dict;
        }

        private static bool ExpandExpressions(IDictionary<string, string> target, IDictionary<string, string> lookup)
        {
            var temp = target.ToList();
            return temp.Count(pair => ExpandExpression(pair, target, lookup)) > 0;
        }

        private static bool ExpandExpression(KeyValuePair<string, string> pair, IDictionary<string, string> target, IDictionary<string, string> lookup)
        {
            var matches = Regex.Matches(pair.Value, "{(?<id>[a-z_]+)}", RegexOptions.ExplicitCapture);
            var expanded = matches.Cast<Match>().Reverse().Aggregate(pair.Value, (ag, match) => UpdateString(ag, match.Groups["id"], lookup));
            target[pair.Key] = expanded;
            return matches.Count > 0;
        }

        private static string UpdateString(string input, Capture group, IDictionary<string, string> lookup)
        {
            var output = input.Remove(group.Index - 1, group.Length + 2);
            return output.Insert(group.Index - 1, lookup[group.Value]);
        }

        public XElement Parse(string input)
        {
            var node = ParseString(_parseDictionary, "", input);
            var doc = new XDocument();
            AddNode(doc, node);
            return doc.Root;
        }

        private static void AddNode(XContainer parent, ParseTreeNode node)
        {
            var element = new XElement(node.Name.Length == 0 ? "root" : node.Name,
                                       new XAttribute("data", node.Text));
            parent.Add(element);
            if (node.Children != null)
                node.Children.Do(child => AddNode(element, child));
        }

        private static ParseTreeNode ParseString(IDictionary<string, string> dict, string nodeName, string input)
        {
            var node = new ParseTreeNode {Name = nodeName, Text = input};

            if (!dict.ContainsKey(nodeName)) return node;

            var pattern = dict[nodeName];
            var subPattern = @"\(\?<(?<groupName>\w+)>";
            var matches = Regex.Matches(pattern, subPattern);
            var groupNames = matches.Cast<Match>().Select(match => match.Groups["groupName"].Value).Distinct();

            matches = Regex.Matches(input, pattern, RegexOptions.IgnorePatternWhitespace);

            var captures = groupNames
                .Select(
                name => matches
                            .Cast<Match>()
                            .Select(
                            match => match
                                         .Groups[name]
                                         .Captures
                                         .Cast<Capture>()
                                         .Where(cap => cap.Value.Length > 0)
                                         .Select(cap => ParseString(dict, name, cap.Value))
                            ).Flatten())
                .Flatten();
            node.Children = captures.ToList();

            return node;
        }

        private class ParseTreeNode
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public IEnumerable<ParseTreeNode> Children { get; set; }
        }
    }
}