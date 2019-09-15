using System;

namespace SimpleParser
{
    public class ParseException : Exception
    {

        public ParseException(string msg) : base(msg)
        {
        }

    }
}