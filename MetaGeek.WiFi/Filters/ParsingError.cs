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
    public class ParsingError
    {
        public /*List<*/ErrorType/*>*/ Error { get; private set; }
        public static readonly ParsingError None = new ParsingError(ErrorType.None);

        public ParsingError()
        {
            Error = ErrorType.None;
            ErrorData = string.Empty;
        }

        public ParsingError(ErrorType error) : this()
        {
            Error = error;
        }

        public void SetError(ErrorType error)
        {
            Error = error;
        }

        public void SetError(ErrorType error, string data)
        {
            SetError(error);
            ErrorData = data;
        }

        public void SetError(ErrorType error, string data, string data2)
        {
            SetError(error);
            ErrorData = data;
            ErrorData2 = data2;
        }

        public bool IsError
        {
            get
            {
                return Error != ErrorType.None;
            }
        }

        public string ErrorData { get; private set; }

        public string ErrorData2 { get; private set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ParsingError)) return false;
            return ((ParsingError)obj).Error == Error;
        }

        public override int GetHashCode()
        {
            return Error.GetHashCode();
        }
    }
}
