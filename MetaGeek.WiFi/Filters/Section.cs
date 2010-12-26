﻿using System;
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
        PropertyInfo CachedParentInfo = null;
        CompareAs CachedCompare = CompareAs.None;

        object compareProperty = null;
        Type comparentType = null;

        public bool Solve(AccessPoint ap)
        {
            if (ap == null) return false;

            if (CachedInfo == null)
            {
                if (!CacheValues()) return false;
            }
            if (CachedCompare != RightValue.CompareAs) return false;

            if (CachedParentInfo != null)
            {
                // Get instance of sub-property
                compareProperty = CachedParentInfo.GetValue(ap, null);
                if (compareProperty == null)
                {
                    compareProperty = Activator.CreateInstance(CachedParentInfo.PropertyType);
                    //return false;
                }
            }
            else
            {
                // Compare to property of parent
                compareProperty = ap;
            }
            object obj = CachedInfo.GetValue(compareProperty, null);

            switch (CachedCompare)
            {
                case CompareAs.String:
                    return CompareString(obj.ToString(), Operator, (string)RightValue.Value);
                case CompareAs.Int:
                    return CompareInt(Convert.ToDouble(obj), Operator, (double)RightValue.Value);
                case CompareAs.Bool:
                    switch (Operator)
                    {
                        case Operator.Equal:
                            return obj == null ? false : (bool)obj == (bool)RightValue.Value;
                        case Operator.NotEqual:
                            return obj == null ? false : (bool)obj != (bool)RightValue.Value;
                        default:
                            return false;
                    }
                case CompareAs.SecurityInt:
                    // Security is ranked as an int, then compares as such
                    return CompareInt(Extensions.SecurityRanking((string)CachedInfo.GetValue(compareProperty, null)), Operator, Extensions.SecurityRanking((string)RightValue.Value));
                default:
                    return false;
            }
        }

        private bool CacheValues()
        {
            Type t = typeof(AccessPoint);
            PropertyInfo pi = null;
            PropertyInfo pip = null;
            bool foundIt = false;
            object[] attrs;
            FilterableAttribute fa = null;
            //FilterableAttribute faOverride = null;

            foreach (PropertyInfo info in t.GetProperties())
            {
                // Look for the display name too
                attrs = info.GetCustomAttributes(typeof(FilterableAttribute), false);
                if (attrs.Length == 0) continue;
                fa = attrs[0] as FilterableAttribute;

                if (info.Name.Equals(FieldName, StringComparison.InvariantCultureIgnoreCase) ||
                    (!string.IsNullOrEmpty(fa.DisplayName) && FieldName.Equals(fa.DisplayName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    pi = info;
                    foundIt = true;
                    break;
                }
            }
            
            if (!foundIt)
            {
                // If we haven't found the property, look in all properties marked with filter-class
                foreach (PropertyInfo info in t.GetProperties())
                {
                    if (foundIt) break;
                    attrs = info.GetCustomAttributes(typeof(FilterableAttribute), false);
                    if (attrs.Length == 0) continue;
                    fa = attrs[0] as FilterableAttribute;
                    if (fa.CompareAs != CompareAs.Class) continue;

                    // Ok, We've found a class of filterable objects
                    //TODO: Make a recursive function to look into more than 1 level deep

                    Type t2 = info.PropertyType;
                    comparentType = t2;
                    foreach (PropertyInfo info2 in t2.GetProperties())
                    {
                        if (info2.Name.Equals(FieldName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            pip = info;
                            pi = info2;
                            foundIt = true;

                            // Check for filterable attribute
                            if (pip.PropertyType.Assembly == Assembly.GetExecutingAssembly())
                            {
                                attrs = pi.GetCustomAttributes(typeof(FilterableAttribute), false);
                                if (attrs.Length < 1) continue;

                                fa = attrs[0] as FilterableAttribute;
                            }
                            else
                            {
                                fa = new FilterableAttribute(Extensions.EstimateCompare(pi));
                            }
                            break;
                        }
                    }
                }
            }

            if (pi == null || !foundIt) return false;

            //Save the property info for later
            CachedInfo = pi;
            CachedParentInfo = pip;

            //if (faOverride == null)
            //{
            //if (pi.PropertyType.Assembly == Assembly.GetExecutingAssembly())
            //{
            if (fa == null)
            {
                attrs = pi.GetCustomAttributes(typeof(FilterableAttribute), false);
                if (attrs.Length < 1) return false;

                fa = attrs[0] as FilterableAttribute;
            }
            //}
            //else
            //{
            //    fa = new FilterableAttribute(Extensions.EstimateCompare(pi));
            //}
            //}
            //else
            //{
            //    fa = faOverride;
            //}
            if (fa == null) return false;

            //Save the CompareAs too
            CachedCompare = fa.CompareAs;

            return true;
        }

        private bool CompareString(string string1, Operator operation, string string2)
        {
            string1 = string1.ToLower();
            string2 = string2.ToLower();
            switch (operation)
            {
                case Operator.Equal:
                    return string1.Equals(string2);
                case Operator.NotEqual:
                    return !string1.Equals(string2);
                case Operator.StartsWith:
                    return string1.StartsWith(string2);
                case Operator.EndsWith:
                    return string1.EndsWith(string2);
                case Operator.NotStartsWith:
                    return !string1.StartsWith(string2);
                case Operator.NotEndsWith:
                    return !string1.EndsWith(string2);
                case Operator.Contains:
                    return string1.Contains(string2);
                case Operator.NotContains:
                    return !string1.Contains(string2);
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
                        newExpr[i] = '\x1F';
                    }
                }
                expr = new string(newExpr);
            }

            string[] parts = expr.Split(' ');
            FieldName = parts[0];

            Operator = Extensions.ParseOperator(parts[1]);

            parts[2] = parts[2].Replace('\x1F', ' ');
            RightValue = new FilterValue(parts[2].Replace("\"", string.Empty),
                                         FieldName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) ? CompareAs.SecurityInt : Extensions.ParseCompareAs(parts[2], Operator));

            CacheValues();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", FieldName, Operator.ToSymbolic(), RightValue.ToString());
        }
    }
}
