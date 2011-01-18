using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inSSIDer.Misc
{
    public class TabLayout
    {
        public static readonly TabLayout Empty = new TabLayout();

        public int SelectedTabIndex { get; set; }
        public string[] TabNames { get; set; }

        /// <summary>
        /// Parses a tabconfig from a string
        /// </summary>
        /// <param name="expr">The string to parse</param>
        public bool FromString(string expr)
        {
            //TabLayout output = new TabLayout();
            if (string.IsNullOrEmpty(expr)) return false;

            string[] parts = expr.Split('|');
            if (parts.Length != 2) return false;
            TabNames = parts[0].Split(',');
            int value;
            if (!int.TryParse(parts[1], out value)) return false;
            SelectedTabIndex = value;
            return true;
        }

        /// <summary>
        /// Converts this tabconfig to a string
        /// </summary>
        /// <returns>the string expression</returns>
        public override string ToString()
        {
            string expr = string.Empty;

            // Tabnames|selectedIndex
            if(TabNames != null && TabNames.Length > 0)
                expr += TabNames.Aggregate((total, current) => total += "," + current);
            expr += "|" + SelectedTabIndex.ToString();

            return expr;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is TabLayout)) return false;
            return TabNames.SequenceEqual(((TabLayout)obj).TabNames) &&
                ((TabLayout)obj).SelectedTabIndex == SelectedTabIndex;
        }

        public override int GetHashCode()
        {
            return TabNames.GetHashCode() | SelectedTabIndex.GetHashCode();
        }
    }
}
