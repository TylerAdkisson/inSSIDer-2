using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MetaGeek.WiFi.Filters
{
    public class Section : IFilterComparable
    {
        string FieldName;
        FilterValue RightValue;
        Operator Operator;

        PropertyInfo CachedInfo = null;
        CompareAs CachedCompare = CompareAs.None;

        public bool Solve(AccessPoint ap)
        {
            if (ap == null) return false;

            if (CachedInfo == null)
            {
                if (!CacheValues()) return false;
            }
            if (CachedCompare != RightValue.CompareAs) return false;

            switch (CachedCompare)
            {
                case CompareAs.String:
                    return CompareString((string)CachedInfo.GetValue(ap,null), Operator, (string)RightValue.Value);
                case CompareAs.Int:
                    return CompareInt((double)CachedInfo.GetValue(ap, null), Operator, (double)RightValue.Value);
                case CompareAs.Bool:
                    switch (Operator)
                    {
                        case Operator.Equal:
                            return (bool)CachedInfo.GetValue(ap, null) == (bool)RightValue.Value;
                        case Operator.NotEqual:
                            return (bool)CachedInfo.GetValue(ap, null) != (bool)RightValue.Value;
                        default:
                            return false;
                    }
                case CompareAs.SecurityInt:
                    return CompareInt(Extensions.SecurityRanking((string)CachedInfo.GetValue(ap, null)), Operator, Extensions.SecurityRanking((string)RightValue.Value));
                default:
                    return false;
            }
        }

        private bool CacheValues()
        {
            Type t = typeof(AccessPoint);
            PropertyInfo pi = null;

            foreach (PropertyInfo info in t.GetProperties())
            {
                if (info.Name.Equals(FieldName, StringComparison.InvariantCultureIgnoreCase))
                {
                    pi = info;
                    break;
                }
            }

            if (pi == null) return false;

            //Save the property info for later
            CachedInfo = pi;

            object[] attrs = pi.GetCustomAttributes(typeof(FilterableAttribute), false);
            if (attrs.Length < 1) return false;

            FilterableAttribute fa = attrs[0] as FilterableAttribute;
            if (fa == null) return false;

            //Save the CompareAs too
            CachedCompare = fa.CompareAs;

            return true;
        }

        private bool CompareString(string string1, Operator operation, string string2)
        {
            switch (operation)
            {
                case Operator.Equal:
                    return string1.Equals(string2, StringComparison.InvariantCultureIgnoreCase);
                case Operator.NotEqual:
                    return !string1.Equals(string2, StringComparison.InvariantCultureIgnoreCase);
                case Operator.StartsWith:
                    return string1.StartsWith(string2, StringComparison.InvariantCultureIgnoreCase);
                case Operator.EndsWith:
                    return string1.EndsWith(string2, StringComparison.InvariantCultureIgnoreCase);
                case Operator.NotStartsWith:
                    return !string1.StartsWith(string2, StringComparison.InvariantCultureIgnoreCase);
                case Operator.NotEndsWith:
                    return !string1.EndsWith(string2, StringComparison.InvariantCultureIgnoreCase);
                default:
                    return false;
            }
        }

        private bool CompareInt(double value1, Operator operation, double value2)
        {
            switch (operation)
            {
                case Operator.Equal:
                    return value1 == value2;
                case Operator.NotEqual:
                    return value1 != value2;
                case Operator.LessThan:
                    return value1 < value2;
                case Operator.GreaterThan:
                    return value1 > value2;
                case Operator.LessEqual:
                    return value1 <= value2;
                case Operator.GreaterEqual:
                    return value1 >= value2;
                default:
                    return false;
            }
        }

        public Section(string property, Operator operation, object value, CompareAs compareValueAs)
        {
            FieldName = property;
            Operator = operation;
            RightValue = new FilterValue(value, compareValueAs);

            CacheValues();
        }

        public Section(string expr)
        {
            //expr will be in form: fieldname op value

            // If this expression contains a string, convert spaces into delimiter
            // so we can split the expression with a space
            if (expr.Contains('"'))
            {
                char[] newExpr = expr.ToCharArray();
                bool isInQuote = false;
                for (int i = 0; i < newExpr.Length; i++)
                {
                    if (newExpr[i] == '"')
                        isInQuote = !isInQuote;

                    if (isInQuote && newExpr[i] == ' ')
                    {
                        newExpr[i] = '|';
                    }
                }
                expr = new string(newExpr);
            }

            string[] parts = expr.Split(' ');
            FieldName = parts[0];

            Operator = Extensions.ParseOperator(parts[1]);

            parts[2] = parts[2].Replace('|', ' ');
            RightValue = new FilterValue(parts[2].Replace("\"", string.Empty), 
                                         FieldName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) ? CompareAs.SecurityInt : Extensions.ParseCompareAs(parts[2]));

            CacheValues();
        }
    }
}
