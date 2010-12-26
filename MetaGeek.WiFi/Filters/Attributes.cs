using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class FilterableAttribute : Attribute
    {
        public CompareAs CompareAs { get; private set; }
        public string DisplayName { get; private set; }

        public FilterableAttribute(CompareAs compareAs)
        {
            CompareAs = compareAs;
            DisplayName = string.Empty;
        }

        public FilterableAttribute(CompareAs compareAs, string displayName) : this(compareAs)
        {
            DisplayName = displayName;
        }
    }
}
