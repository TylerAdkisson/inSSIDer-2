using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace inSSIDer.UI.Theme
{
    public class ColorScheme
    {
        public List<SchemeElement> Elements { get; set; }

        public ColorScheme()
        {
            Elements = new List<SchemeElement>();
        }
        //public SchemeElement this[string elementName]
        //{

        //}

        public IEnumerable<SchemeElement> GetElementsForClass(ColorClass type)
        {
            return Elements.Where(element => element.Class == type);
        }

        public static ColorScheme FromString(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            ColorScheme cs = new ColorScheme();
            SchemeElement tempElement;

            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("//") ||
                    string.IsNullOrEmpty(line.Trim())) continue;

                string[] parts = new string[2];

                // Break at first '='
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != '=') continue;
                    parts[0] = line.Remove(i).Trim();
                    parts[1] = line.Substring(i + 1).Trim();
                    break;
                }

                string[] property = parts[0].Split('.');
                tempElement = new SchemeElement(ParseClass(property[0]), property[1], parts[1]);

                // Add the element
                cs.Elements.Add(tempElement);
            }
            return cs;
        }

        private static ColorClass ParseClass(string name)
        {
            if (string.IsNullOrEmpty(name)) return ColorClass.None;

            try
            {
                object obj = Enum.Parse(typeof(ColorClass), name, true);
                return obj == null ? ColorClass.None : (ColorClass)obj;
            }
            catch (ArgumentException)
            {
                return ColorClass.None;
            }
        }
    }

    public class SchemeElement
    {
        public ColorClass Class { get; private set; }
        public string Property { get; private set; }
        public string Value { get; private set; }

        public SchemeElement(ColorClass _class, string property, string value)
        {
            Class = _class;
            Property = property;
            Value = value;
        }
    }

    public enum ColorClass
    {
        None,
        All,
        Graph,
        Datagrid,
        Menu
    }
}
