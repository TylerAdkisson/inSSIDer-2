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
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class FilterableAttribute : Attribute
    {
        public CompareAs CompareAs { get; private set; }
        public string DisplayName { get; private set; }

        public FilterableAttribute(CompareAs compareAs)
        {
            CompareAs = compareAs;
            DisplayName = string.Empty;
        }

        public FilterableAttribute(CompareAs compareAs, string displayName) : this(compareAs)
        {
            DisplayName = displayName;
        }
    }
}
