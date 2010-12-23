using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class FlakExpressionParser
    {
        public static string Parse(string expr, out ParsingError error)
        {
            string output = expr;
            error = ParsingError.None;

            // Check for missing ()'s around single or 1 layer dual expression
            if (!output.StartsWith("(") && !output.EndsWith(")"))
            {
                if (!output.Contains("&&") || !output.Contains("||"))
                {
                    // Single expression
                    output = "(" + output + ")";
                }
                else
                {
                    if (output.Contains("&&") && output.IndexOf("&&") == output.LastIndexOf("&&"))
                    {
                        // This is a single layer dual expression
                        output = "(" + output + ")";
                    }
                    else if (output.Contains("||") && output.IndexOf("||") == output.LastIndexOf("||"))
                    {
                        // This is a single layer dual expression
                        output = "(" + output + ")";
                    }
                }
            }

            return output;
        }

        public static string Parenthesize(string expr, out ParsingError error)
        {
            error = ParsingError.None;
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
                        paraCount--;
                    if (exprChars[i] == ')')
                        paraCount++;

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
            if (paraCount != 0)
            {
                error.Errors.Add(ErrorType.ParentheseMismatch);
            }
            if (inQuote)
            {
                error.Errors.Add(ErrorType.QuoteMismatch);
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
                    if(right.Contains("||") || right.Contains("&&"))
                        right = Parenthesize(right, out error);
                    if (left.Contains("||") || left.Contains("&&"))
                        left = Parenthesize(left, out error);
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
            if (paraCount != 0)
            {
                error.Errors.Add(ErrorType.ParentheseMismatch);
            }
            if (inQuote)
            {
                error.Errors.Add(ErrorType.QuoteMismatch);
            }
            if (error.IsError)
                return string.Empty;

            // If we haven't found an or operator, but have found an and, parse it
            if (!foundOr && andIndex > -1)
            {
                string left = expr.Substring(0, andIndex - 1).Trim();
                string right = expr.Substring(andIndex + 1).Trim();
                if (right.Contains("&&"))
                    right = Parenthesize(right, out error);
                if (left.Contains("&&"))
                    left = Parenthesize(left, out error);
                output = "(" + left + " && " + right + ")";
            }
            
            return output;
        }
    }
}
