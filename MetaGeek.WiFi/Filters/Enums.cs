using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public enum LogicOperator
    {
        None, And, Or
    }

    public enum Operator
    {
        None, Equal, NotEqual, LessThan, GreaterThan, LessEqual, GreaterEqual, StartsWith, EndsWith, NotStartsWith, NotEndsWith
    }

    public enum CompareAs
    {
        None, String, Int, Bool, SecurityInt, Class
    }

    public enum SecurityType
    {
        None = 0,
        Wep = 1,
        WpaTkip = 2,
        WpaCcmp = 3,
        Wpa2Tkip = 4,
        Wpa2Ccmp = 5
    }
}
