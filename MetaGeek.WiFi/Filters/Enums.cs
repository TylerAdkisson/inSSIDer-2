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

namespace MetaGeek.WiFi.Filters
{
    public enum LogicOperator
    {
        None, And, Or
    }

    public enum Operator
    {
        None,
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessEqual,
        GreaterEqual,
        StartsWith,
        EndsWith,
        NotStartsWith,
        NotEndsWith,
        Contains,
        NotContains
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

    //[Flags]
    public enum ErrorType
    {
        None,
        ParentheseMismatch,
        QuoteMismatch,
        SectionLengthToShort,
        SectionLengthToLong,
        PropertyNameInvalid,
        ExpressionBlank,
        InvalidOperator,
        ValueNotComparable,
        OtherError
    }
}
