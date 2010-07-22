using System;

namespace Navigator.Containers
{
    public class ComponentNotFound : Exception
    {
        private readonly Type _type;

        public ComponentNotFound(Type type)
        {
            _type = type;
        }

        public override string Message
        {
            get
            {
                return "Cound not find a member registered for the type '" + _type + "'";
            }
        }
    }
}