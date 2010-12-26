using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public interface IFilterComparable
    {
        bool Solve(AccessPoint ap);
    }

    public interface IStringSection
    {
        ParsingError CheckLength();
    }
}
