using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaGeek.WiFi.Filters
{
    public class ParsingError
    {
        public List<ErrorType> Errors { get; private set; }
        public static ParsingError None = new ParsingError(ErrorType.None);

        public ParsingError()
        {
            Errors = new List<ErrorType>();
        }

        public ParsingError(params ErrorType[] errors) : this()
        {
            Errors.AddRange(errors);
        }

        public bool IsError
        {
            get
            {
                return Errors.Where(er => er != ErrorType.None).Count() > 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ParsingError)) return false;
            return ((ParsingError)obj).Errors == Errors;
        }

        public override int GetHashCode()
        {
            return Errors.GetHashCode();
        }
    }
}
