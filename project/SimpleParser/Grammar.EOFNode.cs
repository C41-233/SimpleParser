using System.Collections;
using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private class EOFNode : ParseNode
        {
            
            public EOFNode() : base("<EOF>")
            {
            }

            public override bool IsTerminal => true;

            public override IEnumerable<string> Symbols
            {
                get
                {
                    yield return "<eof>";
                }
            }

            public override void Clear()
            {
            }

            public override bool IsClosed => true;

            public override IEnumerator Parse(Grammar grammar, TokenStream stream)
            {
                var next = stream.Next();
                yield return next == null;
            }

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                var next = stream.Next();
                return next == null;
            }

            public override void Visit(IASTVisitor visitor)
            {
            }
        }
    }
}