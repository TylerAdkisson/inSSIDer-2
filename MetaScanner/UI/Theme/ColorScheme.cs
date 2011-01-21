using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using inSSIDer.Misc;
using System.Windows.Forms;

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
            return el1.Properties.SequenceEqual(el2.Properties, new GenericComparer<string>((s1, s2) => s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase)));
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
                if (property.Length > 2)
                    // Skip first 2 because their already there
                    tempElement.Properties.AddRange(property.Skip(2));

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

        public static void ApplyColorScheme(ColorScheme scheme, Control control, ColorClass ctype)
        {
            // Apply all settings for graph controls
            Type type = control.GetType();
            IEnumerable<SchemeElement> elements = scheme.GetElementsForClass(ctype, type);
            foreach (SchemeElement element in elements)
            {
                System.Reflection.PropertyInfo pi = Utilities.GetPropertyByName(type, element.Properties[0]);
                if (pi == null)
                    continue;
                if (element.Properties.Count > 1)
                {
                    System.Reflection.PropertyInfo pi2 = Utilities.GetPropertyByName(pi.PropertyType, element.Properties[1]);
                    object rootObj = pi.GetValue(control, null);
                    object obj2 = Utilities.ConvertType(element.Value, pi2.PropertyType);
                    if (obj2 == null || rootObj == null)
                        continue;
                    pi2.SetValue(rootObj, obj2, null);
                }
                
                object obj = Utilities.ConvertType(element.Value, pi.PropertyType);
                if (obj == null)
                {
                    continue;
                }
                pi.SetValue(control, obj, null);
            }
            control.Invalidate();
        }
    }

    public class SchemeElement
    {
        private List<string> _properties;
        public ColorClass Class { get; private set; }
        public List<string> Properties { get { return _properties; } }
        public string Value { get; private set; }
        public string Control { get; private set; }

        public SchemeElement(ColorClass _class, string property, string value)
        {
            _properties = new List<string>();
            Control = string.Empty;
            Class = _class;

            if (!string.IsNullOrEmpty(property))
                _properties.Add(property);
            Value = value;
        }

        public SchemeElement(ColorClass _class, string property, string value, string control)
            : this(_class, property, value)
        {
            Control = control;
        }

        //public SchemeElement(ColorClass _class, string[] properties, string value, string control)
        //    : this(_class, "", value)
        //{
        //    Control = control;
        //}
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
