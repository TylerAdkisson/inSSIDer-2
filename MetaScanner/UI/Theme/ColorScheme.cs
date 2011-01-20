using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using inSSIDer.Misc;

namespace inSSIDer.UI.Theme
{
    public class ColorScheme
    {
        public List<SchemeElement> Elements { get; set; }
        //private Func<SchemeElement, SchemeElement, bool> CompareProperties = (el1, el2) => el1.Property.Equals(el2.Property, StringComparison.InvariantCultureIgnoreCase);

        public ColorScheme()
        {
            Elements = new List<SchemeElement>();
        }
        //public SchemeElement this[string elementName]
        //{

        //}

        public IEnumerable<SchemeElement> GetElementsForClass(ColorClass ctype, Type type)
        {
            // All
            var global = Elements.Where(element => element.Class == ColorClass.All);

            IEnumerable<SchemeElement> typeSpecific = null;
            IEnumerable<SchemeElement> group = Elements.Where(element => element.Class == ctype);

            if(type != null)
                typeSpecific = Elements.Where(element => element.Control.Equals(type.Name, StringComparison.InvariantCultureIgnoreCase));

            if (ctype == ColorClass.Custom)
            {
                // Merge specifics and the global
                
                var simple = global.Merge(typeSpecific, CompareProperties);
                return simple;
            }
            
            // Merge global with class
            var groupClass = global.Merge(group, CompareProperties);

            IEnumerable<SchemeElement> output = groupClass;

            if (typeSpecific != null)
            {
                // Thne merge type-specific with merged class
                output = groupClass.Merge(typeSpecific, CompareProperties);
            }

            return output;
        }

        private bool CompareProperties(SchemeElement el1, SchemeElement el2)
        {
            return el1.Property.Equals(el2.Property, StringComparison.InvariantCultureIgnoreCase);
        }

        public static ColorScheme FromString(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            ColorScheme cs = new ColorScheme();
            SchemeElement tempElement;
            ColorClass cClass = ColorClass.Custom;

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
                cClass = ParseClass(property[0]);
                tempElement = cClass == ColorClass.Custom ? 
                                            new SchemeElement(cClass, property[1], parts[1], property[0]) :
                                            new SchemeElement(cClass, property[1], parts[1]);

                // Add the element
                cs.Elements.Add(tempElement);
            }
            return cs;
        }

        private static ColorClass ParseClass(string name)
        {
            if (string.IsNullOrEmpty(name)) return ColorClass.Custom;

            try
            {
                object obj = Enum.Parse(typeof(ColorClass), name, true);
                return obj == null ? ColorClass.Custom : (ColorClass)obj;
            }
            catch (ArgumentException)
            {
                return ColorClass.Custom;
            }
        }
    }

    public class SchemeElement
    {
        public ColorClass Class { get; private set; }
        public string Property { get; private set; }
        public string Value { get; private set; }
        public string Control { get; private set; }

        public SchemeElement(ColorClass _class, string property, string value)
        {
            Control = string.Empty;
            Class = _class;
            Property = property;
            Value = value;
        }
        public SchemeElement(ColorClass _class, string property, string value, string control)
            : this(_class, property, value)
        {
            Control = control;
        }
    }

    public enum ColorClass
    {
        Custom,
        All,
        Graph,
        Datagrid,
        Menu
    }
}
