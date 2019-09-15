using System.Collections.Generic;
using SimpleParser.Grammars.Parser.LLn.Tokens;

namespace SimpleParser.Grammars.Parser.LLn.Nodes
{
    internal class TerminalNode : ParseNode, IASTNode
    {

        private readonly TerminalToken terminal;
        private Token token;

        public TerminalNode(TerminalToken terminal)
        {
            this.terminal = terminal;
        }

        public override void Clear()
        {
        }

        public override bool IsClosed => true;

        public override IEnumerator<ParseReport> Generate(LLnParser.ParserContext ctx, TokenStream stream)
        {
            var next = stream.Next();
            if (next == null)
            {
                ctx.PushResult(false);
                yield break;
            }

            var rst = terminal.Match(next);
            if (rst)
            {
                token = next;
            }
            ctx.PushResult(rst);
        }

        public override void Visit(IASTVisitor visitor)
        {
            visitor.BeginNode(this);
            visitor.EndNode(this);
        }

        int IASTNode.Id => terminal.Id;

        string IASTNode.Name => terminal.Name;

        string IASTNode.Value => token.Value;

        bool IASTNode.IsTerminal => true;
    }
}
