using System.Collections.Generic;

namespace SimpleParser.Grammars.Parser.LLn
{
    internal struct ParseReport
    {

        public readonly IEnumerator<ParseReport> Next;

        private ParseReport(IEnumerator<ParseReport> next)
        {
            Next = next;
        }

        public static ParseReport Wait(IEnumerator<ParseReport> next)
        {
            return new ParseReport(next);
        }

    }
}
