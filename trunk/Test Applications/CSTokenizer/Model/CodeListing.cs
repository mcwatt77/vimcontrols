using System.Collections.Generic;

namespace Model
{
    public class Assembly
    {
        public List<Class> Classes = new List<Class>();
    }
    public class CodeListing
    {
        public Assembly Assembly { get; set; }
        public List<Using> Usings = new List<Using>();
    }
    public class Namespace
    {
        public CodeListing CodeListing { get; set; }
        public string Name { get; set; }
    }
    public class Using
    {
        public string Namespace { get; set; }
    }
    public class Class
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<Method> Methods = new List<Method>();
    }
    public class Method
    {
        public string Name { get; set; }
    }
}