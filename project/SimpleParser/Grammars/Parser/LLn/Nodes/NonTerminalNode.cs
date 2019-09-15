using System.Collections.Generic;
using SimpleParser.Grammars.Parser.LLn.Tokens;

namespace SimpleParser.Grammars.Parser.LLn.Nodes
{
    internal class NonTerminalNode : ParseNode
    {

        private readonly NonTerminalToken token;
        private int index;
        private NonTerminalPath[] paths;

        public override bool IsClosed => isClosed;
        private bool isClosed;

        public NonTerminalNode(NonTerminalToken token)
        {
            this.token = token;
        }

        public override void Clear()
        {
            if (paths != null)
            {
                index = 0;
                isClosed = false;
                foreach (var path in paths)
                {
                    path.Clear();
                }
            }
        }

        public override IEnumerator<ParseReport> Generate(LLnParser.ParserContext ctx, TokenStream stream)
        {
            if (paths == null)
            {
                paths = new NonTerminalPath[token.Count];
                for (int i=0, len=token.Count; i < len; i++)
                {
                    paths[i] = new NonTerminalPath(token, i);
                }
            }

            while (index < paths.Length)
            {
                var path = paths[index];
                yield return ParseReport.Wait(path.Generate(ctx, stream));
                if (path.IsClosed)
                {
                    index++;
                }

                if (index >= paths.Length)
                {
                    isClosed = true;
                }

                if (ctx.PopResult())
                {
                    ctx.PushResult(true);
                    yield break;
                }
            }

            ctx.PushResult(false);
        }

        public override void Visit(IASTVisitor visitor)
        {
            paths[index].Visit(visitor);
        }
    }
}
