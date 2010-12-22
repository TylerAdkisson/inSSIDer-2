using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class RootSection : IFilterComparable
    {
        private IFilterComparable Left;
        private IFilterComparable Right;
        private LogicOperator Operator;

        public bool Solve(AccessPoint ap)
        {
            switch (Operator)
            {
                case LogicOperator.And:
                    return Left.Solve(ap) && Right.Solve(ap);
                case LogicOperator.Or:
                    return Left.Solve(ap) || Right.Solve(ap);
                default:
                    return false;
            }
        }

        public RootSection(IFilterComparable left, LogicOperator operation, IFilterComparable right)
        {
            Left = left;
            Right = right;
            Operator = operation;
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", Left.ToString(), Operator.ToSymbolic(), Right.ToString());
        }
    }
}
