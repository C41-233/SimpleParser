using System.Collections.Generic;
using SimpleParser.Grammars.Parser.LLn.Tokens;

namespace SimpleParser.Grammars.Parser.LLn.Nodes
{
    internal class NonTerminalPath : ParseNode, IASTNode
    {

        private readonly int id;
        private readonly string name;

        private readonly LLnToken[] path;
        private ParseNode[] nodes;
        private readonly Stack<int> offsets = new Stack<int>();
        private bool init = true;
        private bool isClosed = false;

        public override bool IsClosed => isClosed;

        public NonTerminalPath(NonTerminalToken token, int n)
        {
            id = token.Id(n);
            path = token.Path(n);
            name = token.Name;
        }

        public override void Clear()
        {
            if (nodes != null)
            {
                init = true;
                isClosed = false;
                offsets.Clear();
                foreach (var node in nodes)
                {
                    node.Clear();
                }
            }
        }

        public override IEnumerator<ParseReport> Generate(LLnParser.ParserContext ctx, TokenStream stream)
        {
            if (nodes == null)
            {
                nodes = new ParseNode[path.Length];
                for (var i = 0; i < path.Length; i++)
                {
                    nodes[i] = path[i].CreateNode();
                }
            }

            int n;
            if (init)
            {
                init = false;
                n = 0;
            }
            else
            {
                n = nodes.Length - 1;
                if (!BackTrace(ref n, stream))
                {
                    isClosed = true;
                    ctx.PushResult(false);
                    yield break;
                }

                for (var i = n + 1; i < nodes.Length; i++)
                {
                    nodes[i].Clear();
                }
            }

            while (n < nodes.Length)
            {
                offsets.Push(stream.Offset);
                yield return ParseReport.Wait(nodes[n].Generate(ctx, stream));
                if (ctx.PopResult())
                {
                    n++;
                    continue;
                }

                offsets.Pop();
                n--;
                if (!BackTrace(ref n, stream))
                {
                    isClosed = true;
                    ctx.PushResult(false);
                    yield break;
                }

                for (var i = n + 1; i < nodes.Length; i++)
                {
                    nodes[i].Clear();
                }
            }

            ctx.PushResult(true);
        }

        private bool BackTrace(ref int n, TokenStream stream)
        {
            for (var i = n; i >= 0; i--)
            {
                var offset = offsets.Pop();
                if (!nodes[i].IsClosed)
                {
                    stream.Reset(offset);
                    n = i;
                    return true;
                }
            }

            return false;
        }

        public override void Visit(IASTVisitor visitor)
        {
            visitor.BeginNode(this);
            foreach (var node in nodes)
            {
                node.Visit(visitor);
            }
            visitor.EndNode(this);
        }

        int IASTNode.Id => id;

        string IASTNode.Name => name;

        string IASTNode.Value => null;

        bool IASTNode.IsTerminal => false;
    }
}
