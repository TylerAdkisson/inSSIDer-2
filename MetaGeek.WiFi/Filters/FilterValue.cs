using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class FilterValue
    {
        public CompareAs CompareAs;
        public object Value;

        public FilterValue(object value, CompareAs compareAs)
        {
            CompareAs = compareAs;
            switch (compareAs)
            {
                case CompareAs.String:
                    Value = value.ToString();
                    break;
                case CompareAs.Int:
                    Value = double.Parse(value.ToString());
                    break;
                case CompareAs.Bool:
                    Value = bool.Parse(value.ToString());
                    break;
                default:
                    Value = value;
                    break;
            }
        }

        public bool Compare(FilterValue value, Operator operation)
        {
            if (value.CompareAs != CompareAs) return false;
            return false;
        }
    }
}
