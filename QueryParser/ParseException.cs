using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HigginsThomas.QueryParser.Core
{
    public class ParserException : Exception
    {
        public ParserException(string msg) : base(msg) { }
        public ParserException(string msg, Exception e) : base(msg, e) { }
    }

    public class ParseException : ParserException
    {
        public ParseException(string msg, StreamLocation loc) : base(String.Format("Syntax Error: {0} @ {1}", msg, loc)) { }
        public ParseException(string msg, StreamLocation loc, Exception e) : base(String.Format("Syntax Error: {0} @ {1}", msg, loc), e) { }
    }
}
