using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class CorrectedExpressionParser
    {
        public static IFilterComparable Parse(string expr)
        {
            //Sections can be enclosed in (). remove for processing
            /*if (expr.StartsWith("("))*/ expr = expr.Remove(0, 1);
            /*if (expr.EndsWith(")"))*/ expr = expr.Remove(expr.Length - 1, 1);

            int paraTally = 0;
            char tempChar = '\0';

            int indexLastOpen = -1;

            string sectionLeft = "";
            string sectionRight = "";
            LogicOperator op = LogicOperator.None;

            //To preserve data in quotes, like strings
            bool isInQuotes = false;
            bool dualExpr = false;

            //bool leftSection = true;
            //eg. Ssid == "fuji-2" || (Ssid sw "Hot" && Channel == 11)

            for (int i = 0; i < expr.Length; i++)
            {
                tempChar = expr[i];

                // If we aren't in quotes, set true, if we are, set false
                if (tempChar == '"') isInQuotes = !isInQuotes;

                if (tempChar == '(' && !isInQuotes)
                {
                    paraTally++;
                    if (paraTally == 1)
                    {
                        // Only mark the outermost
                        indexLastOpen = i;
                    }
                }
                else if (tempChar == ')' && !isInQuotes)
                {
                    //if (string.IsNullOrEmpty(sectionLeft))
                    //{
                    //    sectionLeft = expr.Substring(indexLastOpen, i - indexLastOpen + 1);
                    //}
                    //else
                    //{
                    //    sectionRight = expr.Substring(indexLastOpen, i - indexLastOpen + 1);
                    //}
                    paraTally--;
                }

                if (paraTally == 0)
                {
                    if ((expr[i] == '&' && expr[i + 1] == '&') ||
                        (expr[i] == '|' && expr[i + 1] == '|'))
                    {
                        //We've found the delimiter
                        sectionLeft = expr.Substring(0, i - 1);
                        sectionRight = expr.Substring(i + 3);
                        op = expr[i] == '&' ? LogicOperator.And : LogicOperator.Or;
                        // This is dial expression, return a RootSection
                        dualExpr = true;
                    }
                }


            }
            if (dualExpr)
            {
                IFilterComparable left;
                IFilterComparable right;

                // Left section
                if (sectionLeft.StartsWith("("))
                {
                    left = Parse(sectionLeft);
                }
                else
                {
                    left = new Section(sectionLeft);//new FilterValue(sectionLeft, ParseCompare(sectionLeft));
                }

                // Right section
                if (sectionRight.StartsWith("("))
                {
                    right = Parse(sectionRight);
                }
                else
                {
                    right = new Section(sectionRight);
                }
                return new RootSection(left, op, right);
            }
            else
            {
                return new Section(expr);
            }

            //return dualExpr? new RootSection(new FilterValue(sectionLeft, ParseCompare(sectionLeft)),op, 
        }
    }
}
