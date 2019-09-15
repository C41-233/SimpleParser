using System.Collections.Generic;

namespace SimpleParser.Grammars.Parser.LLn.Nodes
{
    internal class EOFNode : ParseNode
    {

        public override void Clear()
        {
        }

        public override bool IsClosed => true;

        public override IEnumerator<ParseReport> Generate(LLnParser.ParserContext ctx, TokenStream stream)
        {
            var next = stream.Next();
            ctx.PushResult(next == null);
            yield break;
        }

        public override void Visit(IASTVisitor visitor)
        {
        }
    }
}
