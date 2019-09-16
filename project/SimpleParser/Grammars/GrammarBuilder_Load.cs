using System;
using System.Text.RegularExpressions;

namespace SimpleParser.Grammars
{
    public sealed partial class GrammarBuilder
    {

        private MetaParser parser;

        public int Load(string definition)
        {
            parser = parser ?? new MetaParser();
            parser.Parse(definition);
            //return parser.ParseFromFile(definition);
            return 0;
        }

        private class MetaParser : LexicalParser
        {

            private enum ParseStep
            {
                Begin,
                WaitForToken,
                WaitForArrow,
            }

            private ParseStep Step;

            protected override void OnBegin()
            {
                Step = ParseStep.Begin;
            }

            protected override void OnEnd()
            {
            }

            protected override bool Read()
            {
                var ch = (char) ByteValue;
                return true;
            }

        }

    }
}
