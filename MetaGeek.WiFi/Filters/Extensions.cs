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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MetaGeek.WiFi.Filters
{
    public static class Extensions
    {
        public static Operator ParseOperator(string data)
        {
            data = data.Trim();
            if (data == "==") return Operator.Equal;
            if (data == "!=") return Operator.NotEqual;
            if (data == ">") return Operator.GreaterThan;
            if (data == ">=") return Operator.GreaterEqual;
            if (data == "<") return Operator.LessThan;
            if (data == "<=") return Operator.LessEqual;
            if (data.ToLower() == "sw") return Operator.StartsWith;
            if (data.ToLower() == "ew") return Operator.EndsWith;
            if (data.ToLower() == "!sw") return Operator.NotStartsWith;
            if (data.ToLower() == "!ew") return Operator.NotEndsWith;
            if (data.ToLower() == "con") return Operator.Contains;
            if (data.ToLower() == "!con") return Operator.NotContains;
            return Operator.None;
        }

        //public static CompareAs ParseCompareAs(string expr)
        //{
        //    if (expr.Contains('"')) return CompareAs.String;
        //    if (expr.ToLower().Contains("true") || expr.ToLower().Contains("false")) return CompareAs.Bool;
        //    else return CompareAs.Int;
        //}

        public static CompareAs ParseCompareAs(string expr, Operator operation)
        {
            // Check by operation bias first
            switch (operation)
            {
                case Operator.LessThan:
                case Operator.GreaterThan:
                case Operator.LessEqual:
                case Operator.GreaterEqual:
                    // Int
                    return CompareAs.Int;
                case Operator.StartsWith:
                case Operator.EndsWith:
                case Operator.NotStartsWith:
                case Operator.NotEndsWith:
                    // String
                    return CompareAs.String;
                case Operator.Equal:
                case Operator.NotEqual:
                default:
                    // Anything else goes through normal compare
                    break;
            }
            if (expr.Contains('"')) return CompareAs.String;
            if (expr.ToLower().Contains("true") || expr.ToLower().Contains("false")) return CompareAs.Bool;
            else return CompareAs.Int;
        }

        public static bool ParseBool(string expr)
        {
            return expr.ToLower().Contains("true");
        }

        //None, WEP, WPA_TKIP, WPA_CCMP, WPA2_TKIP, WPA2_CCMP
        public static int SecurityRanking(string security)
        {
            security = security.ToLower();
            if (security.Contains("tkip"))
            {
                if (security.Contains("wpa2") || security.Contains("rsna"))
                {
                    return (int)SecurityType.Wpa2Tkip;
                }
                if (security.Contains("wpa"))
                {
                    return (int)SecurityType.WpaTkip;
                }
            }
            else if (security.Contains("ccmp") || security.Contains("aes"))
            {
                if (security.Contains("wpa2") || security.Contains("rsna"))
                {
                    return (int)SecurityType.Wpa2Ccmp;
                }
                if (security.Contains("wpa"))
                {
                    return (int)SecurityType.WpaCcmp;
                }
            }
            else if (security.StartsWith("wpa2") || security.StartsWith("rsna")) return (int)SecurityType.Wpa2Ccmp;
            else if (security.StartsWith("wpa")) return (int)SecurityType.WpaTkip;
            else if (security.StartsWith("wep")) return (int)SecurityType.Wep;
            else if (security.StartsWith("none")) return (int)SecurityType.None;

            return -1;
        }

        public static CompareAs EstimateCompare(PropertyInfo pinfo)
        {
            if (pinfo == null) return CompareAs.String;

            if (typeof(double).IsAssignableFrom(pinfo.PropertyType))
                return CompareAs.Int;

            if (typeof(bool).IsAssignableFrom(pinfo.PropertyType))
                return CompareAs.Bool;

            return CompareAs.String;

        }

        public static string ToSymbolic(this Operator operation)
        {
            switch (operation)
            {
                case Operator.Equal:
                    return "==";
                case Operator.NotEqual:
                    return "!=";
                case Operator.LessThan:
                    return "<";
                case Operator.GreaterThan:
                    return ">";
                case Operator.LessEqual:
                    return "<=";
                case Operator.GreaterEqual:
                    return ">=";
                case Operator.StartsWith:
                    return "sw";
                case Operator.EndsWith:
                    return "ew";
                case Operator.NotStartsWith:
                    return "!sw";
                case Operator.NotEndsWith:
                    return "!ew";
                case Operator.Contains:
                    return "con";
                case Operator.NotContains:
                    return "!con";
                default:
                    return string.Empty;
                    break;
            }
        }

        public static string ToSymbolic(this LogicOperator operation)
        {
            switch (operation)
            {
                case LogicOperator.And:
                    return "&&";
                case LogicOperator.Or:
                    return "||";
                default:
                    return string.Empty;
            }
        }
    }
}
