////////////////////////////////////////////////////////////////
//
// Copyright (c) 2007-2011 MetaGeek, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//
//	http://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
//
////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace inSSIDer.Properties
{
    internal sealed partial class Settings
    {
        //private bool _useSettings = true;
        //private bool _ranCheck;

        private static Dictionary<string, string> _overrides;

        static Settings()
        {
            _overrides = new Dictionary<string, string>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(Application.StartupPath + "\\overrides.txt");

                // Parse them
                foreach (string line in lines)
                {
                    if(string.IsNullOrEmpty(line) || line.Trim().StartsWith("//")) continue;
                    string[] parts = new string[2];
                    for (int i = 0; i < line.Length; i++)
                    {
                        // Break at the first '='
                        if (line[i] != '=') continue;
                        parts[0] = line.Remove(i);
                        parts[1] = line.Substring(i + 1);
                        break;
                    }

                    _overrides.Add(parts[0], parts[1]);
                }
            }
            catch
            {
                // This isn't important to the operation of inSSIDer, catch everything
            }
        }

        public override object this[string propertyName]
        {
            get
            {
                Debug.WriteLine(string.Format("Requested setting \"{0}\"", propertyName));
                // Check for overrides
                // If an override is set, use it...unless it's invalid
                if (_overrides.ContainsKey(propertyName))
                {
                    Debug.WriteLine(string.Format("Override setting: \"{0}\" with value \"{1}\"", propertyName, _overrides[propertyName]));
                    SettingsProperty sp = Properties[propertyName];
                    if (sp != null && sp.PropertyType != null)
                    {
                        try
                        {
                            return ConvertType(_overrides[propertyName], sp.PropertyType);
                        }
                        catch
                        {
                            // It's bad, remove the override
                            _overrides.Remove(propertyName);
                        }
                    }
                }

                return base[propertyName];
            }
            set
            {
                // If a settings is overridden, don't save it
                if (_overrides.ContainsKey(propertyName)) return;
                base[propertyName] = value;
            }
        }

        //public override object this[string propertyName]
        //{
        //    get
        //    {
        //        //Check config system
        //        if(!_ranCheck)
        //        {
        //            CheckSettingsSystem();
        //        }
        //        if (_useSettings)
        //        {
        //            return base[propertyName];
        //        }
        //        //object o = null;
        //        //SettingsProperty sp = Properties[propertyName];

        //        //if (sp != null)
        //        //{
        //        //        if(sp.SerializeAs == SettingsSerializeAs.String)
        //        //        {
        //        //            o = Parse(sp.DefaultValue.ToString(), sp.PropertyType);
        //        //        }
        //        //}

        //        //return o;
        //    }
        //    set
        //    {
        //        //Check config system
        //        if(!_ranCheck)
        //        {
        //            CheckSettingsSystem();
        //        }
        //        if (_useSettings)
        //        {
        //            base[propertyName] = value;
        //        }
        //    }
        //}

        /// <summary>
        /// Checks the configuration system to make sure it can be used
        /// </summary>
        /// <returns>True is reading and writing work, otherwise false</returns>
        public bool CheckSettingsSystem()
        {
            try
            {
                //Try settings get
                string setting = (string)base["settingsTest"];
                //Then set
                base["settingsTest"] = "OK";
                //If we haven't exceptioned yet, return true
                //_useSettings = true;
                return true;
            }
            catch (ConfigurationException)
            {
                //_useSettings = false;
                return false;
            }
        }

        static object ConvertType(string value, Type type)
        {
            // might need ConvertFromString
            // (rather than Invariant)
            return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
        }
    }
}
