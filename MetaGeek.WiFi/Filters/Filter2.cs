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
    public class Filter2
    {
        public IFilterComparable InnerExpression { get; set; }
        public bool Enabled { get; set; }
        public Guid Id { get; private set; }

        #region Methods

        public bool Eval(AccessPoint ap)
        {
            if (InnerExpression == null) return false;
            return InnerExpression.Solve(ap);
        }


        /// <summary>
        /// Sets the expression specified string
        /// </summary>
        /// <param name="expr">A string expression</param>
        // /// <remarks>WARNING: THE EXPRESSION MUST BE IN CORRECT SYNTAX</remarks>
        public void SetExpression(string expr)
        {
            ParsingError error;
            InnerExpression = CorrectedExpressionParser.Parse(FlakExpressionParser.Parse(expr, out error));
        }

        public override string ToString()
        {
            return InnerExpression.ToString();
        }

        public object[] GetData()
        {
            return new object[] { Id, Enabled, ToString(), 0 };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Filter2()
        {
            Id = Guid.NewGuid();
            Enabled = true;
        }

        /// <summary>
        /// Creates a new filter with the supplied expression object as it's base
        /// </summary>
        /// <param name="expression">An IFilterComparable object that represents the expression to filter by</param>
        public Filter2(IFilterComparable expression) : this()
        {
            InnerExpression = expression;
        }

        /// <summary>
        /// Creates a new filter with the specified string as the expression
        /// </summary>
        /// <param name="expr">A string expression</param>
        // /// <remarks>WARNING: THE EXPRESSION MUST BE IN CORRECT SYNTAX</remarks>
        public Filter2(string expr) : this()
        {
            SetExpression(expr);
        }

        #endregion
    }
}
