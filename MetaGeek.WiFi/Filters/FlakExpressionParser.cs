using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace MetaGeek.WiFi.Filters
{
    public class FlakExpressionParser
    {
        public static string Parse(string expr, out ParsingError error)
        {
            string output = expr;
            error = new ParsingError();

            // Add missing parenthese
            output = Parenthesize(expr, out error);

            return output;
        }

        public static string MiscCheck(string expr, out ParsingError error)
        {
            string output = expr;
            error = new ParsingError();

            if (string.IsNullOrEmpty(expr.Trim()))
            {
                error.SetError(ErrorType.ExpressionBlank);
                output = string.Empty;
            }

            return output;
        }

        public static string Parenthesize(string expr, out ParsingError error)
        {
            error = new ParsingError();
            if(expr == null || expr.Length == 0) return "";
            
            expr = expr.Trim();

            string output = "";
            bool inQuote = false;
            int paraCount = 0;
            bool foundOr = false;
            int andIndex = -1;

            char[] exprChars = expr.ToCharArray();

            // Check if the expression is enclosed in parentheses, and if is is,
            // check to see if they are related
            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                bool areRelated = true;
                for (int i = 0; i < exprChars.Length; i++)
                {
                    if (exprChars[i] == '"')
                        inQuote = !inQuote;

                    if (inQuote) continue;

                    if (exprChars[i] == '(')
                        paraCount++;
                    if (exprChars[i] == ')')
                        paraCount--;

                    if ((i != 0 && i < exprChars.Length -1) && paraCount < 1)
                    {
                        areRelated = false;
                    }
                }
                if (areRelated)
                {
                    // Remove them
                    expr = expr.Remove(expr.Length - 1, 1);
                    expr = expr.Remove(0, 1);
                }
            }

            // Check for errors
            // Check for quote mismatch first, because it can cause a parenthese mismatch.
            if (inQuote)
            {
                error.SetError(ErrorType.QuoteMismatch, expr);
            }
            else if (paraCount != 0)
            {
                error.SetError(ErrorType.ParentheseMismatch, expr);
            }

            if (error.IsError)
                return string.Empty;

            // Reset variables
            output = expr;
            exprChars = expr.ToCharArray();
            paraCount = 0;

            // Loop backwards to find delimites
            for (int i = exprChars.Length - 1; i >= 0; i--)
            {
                // Quote detection
                if (exprChars[i] == '"')
                    inQuote = !inQuote;

                if (inQuote) continue;

                if (exprChars[i] == '(')
                    paraCount--;
                if (exprChars[i] == ')')
                    paraCount++;

                if (paraCount == 0 && exprChars[i] == '|' && exprChars[i - 1] == '|')
                {
                    foundOr = true;
                    // We've found them
                    string left = expr.Substring(0, i - 1).Trim();
                    string right = expr.Substring(i+1).Trim();
                    if (right.Contains("||") || right.Contains("&&"))
                    {
                        right = Parenthesize(right, out error);
                        if (error.IsError)
                            return string.Empty;
                    }
                    if (left.Contains("||") || left.Contains("&&"))
                    {
                        left = Parenthesize(left, out error);
                        if (error.IsError)
                            return string.Empty;
                    }
                    output = "(" + left + " || " + right + ")";
                    break;
                }
                else if (andIndex == -1 && paraCount == 0 && exprChars[i] == '&' && exprChars[i - 1] == '&')
                {
                    // Store the last and location of later
                    andIndex = i;
                }
            }

            // Check for errors
            // Check for quote mismatch first, because it can cause a parenthese mismatch.
            if (inQuote)
            {
                error.SetError(ErrorType.QuoteMismatch, expr);
            }
            else if (paraCount != 0)
            {
                error.SetError(ErrorType.ParentheseMismatch, expr);
            }
            if (error.IsError)
                return string.Empty;

            // If we haven't found an or operator, but have found an and, parse it
            if (!foundOr && andIndex > -1)
            {
                string left = expr.Substring(0, andIndex - 1).Trim();
                string right = expr.Substring(andIndex + 1).Trim();
                if (right.Contains("&&"))
                {
                    right = Parenthesize(right, out error);
                    if (error.IsError)
                        return string.Empty;
                }
                if (left.Contains("&&"))
                {
                    left = Parenthesize(left, out error);
                    if (error.IsError)
                        return string.Empty;
                }
                output = "(" + left + " && " + right + ")";
            }
            else if(!foundOr)
            {
                // It's a single expression
                output = "(" + output + ")";
            }
            
            return output;
        }

        public static bool CheckSections(string expr, out ParsingError error)
        {
            // Remove outer parentheses
            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                expr = expr.Remove(0, 1);
                expr = expr.Remove(expr.Length - 1, 1);
            }

            //string output = expr;
            error = new ParsingError();

            bool inQuote = false;
            int paraCount = 0;
            int sepIndex = -1;

            char[] exprChars = expr.ToCharArray();

            for (int i = 0; i < exprChars.Length; i++)
            {
                if (exprChars[i] == '"')
                    inQuote = !inQuote;

                if (exprChars[i] == '(')
                    paraCount++;

                if (exprChars[i] == ')')
                    paraCount--;

                // Ignore anything that's in quotes or parenthese
                if (inQuote || paraCount > 0)
                    continue;

                if ((exprChars[i] == '&' && exprChars[i + 1] == '&') ||
                    (exprChars[i] == '|' && exprChars[i + 1] == '|'))
                {
                    sepIndex = i;
                    break;
                }
            }

            if (sepIndex > -1)
            {
                // Dual expression
                string left = expr.Substring(0, sepIndex - 1);
                string right = expr.Substring(sepIndex + 3);

                CheckSections(left, out error);
                if (error.IsError)
                    return false;
                CheckSections(right, out error);
                if (error.IsError)
                    return false;

            }
            else
            {
                // Single expression
                for (int i = 0; i < exprChars.Length; i++)
                {
                    if (exprChars[i] == '"')
                        inQuote = !inQuote;
                    if (inQuote && exprChars[i] == ' ')
                    {
                        exprChars[i] = '\x1F';
                    }
                }

                string newString = new string(exprChars);

                string[] parts = newString.Split(' ');

                //output = parts.Aggregate((s, next) => s += " " + next.Replace('\x31', ' ')));

                // Check for section length
                if (parts.Length != 3)
                {
                    error.SetError(parts.Length < 3 ? ErrorType.SectionLengthToShort : ErrorType.SectionLengthToLong, expr);
                    return false;
                }

                CompareAs compareAs;
                parts[0] = CheckPropertyname(parts[0], out error, out compareAs);
                if (error.IsError)
                    return false;

                Operator operation = Extensions.ParseOperator(parts[1]);
                if (operation == Operator.None || !CheckOperator(operation,compareAs))
                {
                    error.SetError(ErrorType.InvalidOperator, parts[1]);
                    return false;
                }

                bool isComparable = CheckValueType(compareAs, parts[2]);
                if (!isComparable)
                {
                    error.SetError(ErrorType.ValueNotComparable, parts[2], parts[0]);
                    return false;
                }

                // Rebuild string
                //output = parts.Aggregate((s, next) => s += " " + next.Replace('\x31', ' '));
            }

            return true;
        }

        public static string CheckPropertyname(string name, out ParsingError error, out CompareAs compareAs)
        {
            string output = name;
            error = new ParsingError();
            compareAs = CompareAs.None;

            Type apType = typeof(AccessPoint);

            PropertyInfo info = FindProperty(name, apType, out compareAs);
            if (info == null)
                error.SetError(ErrorType.PropertyNameInvalid, name);
            else
            {
                output = info.Name;
                //if (info.PropertyType.Assembly == Assembly.GetExecutingAssembly())
                //{
                //    compareAs = ((FilterableAttribute)info.GetCustomAttributes(typeof(FilterableAttribute), false)[0]).CompareAs;
                //}
                //else
                //{
                //    compareAs = Extensions.EstimateCompare(info);
                //}
            }

            return output;
        }

        private static PropertyInfo FindProperty(string name, Type type, out CompareAs compare)
        {
            IEnumerable<PropertyInfo> filterable = type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(FilterableAttribute), false).Length > 0);
            IEnumerable<PropertyInfo> classes = filterable.Where(pi => ((FilterableAttribute)pi.GetCustomAttributes(typeof(FilterableAttribute), false)[0]).CompareAs == CompareAs.Class);

            PropertyInfo output = null;
            FilterableAttribute fa;
            compare = CompareAs.None;

            if (type.Assembly == Assembly.GetExecutingAssembly())
            {
                foreach (PropertyInfo prop in filterable)
                {
                    fa = (FilterableAttribute)prop.GetCustomAttributes(typeof(FilterableAttribute), false)[0];
                    if (prop.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                        fa.DisplayName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        compare = fa.CompareAs;
                        return prop;
                    }
                }

                // We've not found the property, look in sub-properties
                foreach (PropertyInfo prop in classes)
                {
                    output = FindProperty(name, prop.PropertyType, out compare);
                    if (output != null)
                        return output;
                }
            }
            else
            {
                // The type is out of assembly, don't use FilterableAttribute
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if (prop.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        compare = Extensions.EstimateCompare(prop);
                        return prop;
                    }
                }
            }
            return output;
        }

        public static bool CheckValueType(CompareAs propCompare, string value)
        {
            switch (propCompare)
            {
                case CompareAs.String:
                    return value.StartsWith("\"") && value.EndsWith("\"");
                case CompareAs.Int:
                    int intValue;
                    return int.TryParse(value, out intValue);
                case CompareAs.Bool:
                    bool result;
                    return bool.TryParse(value, out result);
                case CompareAs.SecurityInt:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CheckOperator(Operator operation, CompareAs compareAs)
        {
            if (operation == Operator.None || compareAs == CompareAs.None) return false;
            // Everything supports equals and not equals
            if (operation == Operator.Equal || operation == Operator.NotEqual) return true;
            switch (compareAs)
            {
                case CompareAs.String:
                    return operation != Operator.LessEqual && 
                        operation != Operator.LessThan && 
                        operation != Operator.GreaterEqual && 
                        operation != Operator.GreaterThan; 
                    // Security is evaluated as an int
                case CompareAs.Int:
                case CompareAs.SecurityInt:
                    return operation == Operator.LessEqual ||
                        operation == Operator.LessThan ||
                        operation == Operator.GreaterEqual ||
                        operation == Operator.GreaterThan;
                default:
                    return false;
            }
        }

        public static Dictionary<string[], CompareAs> GetFilterableProperties(Type t)
        {
            Dictionary<string[], CompareAs> props = new Dictionary<string[], CompareAs>();

            Type type = t;//typeof(AccessPoint);

            IEnumerable<PropertyInfo> filterable = type.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(FilterableAttribute), false).Length > 0);
            IEnumerable<PropertyInfo> notClass = filterable.Where(pi => ((FilterableAttribute)pi.GetCustomAttributes(typeof(FilterableAttribute), false)[0]).CompareAs != CompareAs.Class);
            IEnumerable<PropertyInfo> classes = filterable.Where(pi => ((FilterableAttribute)pi.GetCustomAttributes(typeof(FilterableAttribute), false)[0]).CompareAs == CompareAs.Class);

            if (type.Assembly == Assembly.GetExecutingAssembly())
            {
                foreach (PropertyInfo prop in notClass)
                {
                    FilterableAttribute fa = (FilterableAttribute)prop.GetCustomAttributes(typeof(FilterableAttribute), false)[0];
                    props.Add(new[] { prop.Name, fa.DisplayName }, fa.CompareAs);
                }

                // Look in sub-properties too
                foreach (PropertyInfo prop in classes)
                {
                    Dictionary<string[], CompareAs> more = GetFilterableProperties(prop.PropertyType);
                    foreach (KeyValuePair<string[], CompareAs> item in more)
                    {
                        props.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                // The type is out of assembly, don't use FilterableAttribute
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    props.Add(new[] { prop.Name, "" }, Extensions.EstimateCompare(prop));
                }
            }

            return props;
        }
    }
}
