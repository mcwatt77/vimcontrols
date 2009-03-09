using System;

namespace DataProcessors
{
    public class LaunchableAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public LaunchableAttribute()
        {
        }

        public LaunchableAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}