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
