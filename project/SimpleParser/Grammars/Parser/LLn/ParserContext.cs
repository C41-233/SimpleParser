using System.Diagnostics;

namespace SimpleParser.Grammars.Parser.LLn
{
    internal partial class LLnParser
    {
        internal class ParserContext
        {

            private readonly LLnParser parser;

            private bool? value;

            public ParserContext(LLnParser parser)
            {
                this.parser = parser;
            }

            public bool PopResult()
            {
                Debug.Assert(value.HasValue);
                var r = value.Value;
                value = null;
                return r;
            }

            public void PushResult(bool r)
            {
                Debug.Assert(!value.HasValue);
                value = r;
            }

            public bool PeekResult()
            {
                Debug.Assert(value.HasValue);
                return value.Value;
            }
        }

    }
}
