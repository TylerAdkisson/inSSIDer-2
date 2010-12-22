using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class FlakExpressionParser
    {
        public static string Parse(string expr, out string error)
        {
            string output = expr;
            error = string.Empty;

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
    }
}
