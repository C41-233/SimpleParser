using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private class TerminalNode : ParseNode
        {

            private readonly Terminal terminal;
            private Token token;

            public TerminalNode(string name, Terminal terminal) : base(name)
            {
                this.terminal = terminal;
            }

            public override bool IsClosed => true;

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                var next = stream.Next();
                if (next == null)
                {
                    return false;
                }

                var rst = terminal.Match(next);
                if (rst)
                {
                    token = next;
                }

                return rst;
            }

            public override void Visit(IASTVisitor visitor)
            {
                visitor.BeginNode(this);
                visitor.EndNode(this);
            }

            public override bool IsTerminal => true;

            public override IEnumerable<string> Symbols
            {
                get
                {
                    yield return token.Value;
                }
            }
            

            public override void Clear()
            {
                token = null;
            }
        }
    }
}