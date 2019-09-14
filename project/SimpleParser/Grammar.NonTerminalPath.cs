using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleParser
{
    public partial class Grammar
    {
        private class NonTerminalPath : ParseNode
        {

            private readonly string[] path;
            private ParseNode[] nodes;
            private readonly Stack<int> offsets = new Stack<int>();
            private bool init = true;
            private bool isClosed = false;

            public override bool IsClosed => isClosed;


            public NonTerminalPath(string name, string[] path) : base(name)
            {
                this.path = path;
            }

            public override bool IsTerminal => false;
            public override IEnumerable<string> Symbols => path;

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

            public override IEnumerator Parse(Grammar grammar, TokenStream stream)
            {
                if (nodes == null)
                {
                    nodes = new ParseNode[path.Length];
                    for (var i = 0; i < path.Length; i++)
                    {
                        nodes[i] = CreateNode(grammar, i);
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
                        yield return false;
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
                    yield return nodes[n].Parse(grammar, stream);
                    if (grammar.rst)
                    {
                        n++;
                        continue;
                    }

                    offsets.Pop();
                    n--;
                    if (!BackTrace(ref n, stream))
                    {
                        isClosed = true;
                        yield return false;
                        yield break;
                    }

                    for (var i = n + 1; i < nodes.Length; i++)
                    {
                        nodes[i].Clear();
                    }
                }

                yield return true;
            }

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                //if (nodes == null)
                //{
                //    nodes = new ParseNode[path.Length];
                //    for (var i = 0; i < path.Length; i++)
                //    {
                //        nodes[i] = CreateNode(grammar, i);
                //    }
                //}

                //int n;
                //if (init)
                //{
                //    init = false;
                //    n = 0;
                //}
                //else
                //{
                //    n = nodes.Length - 1;
                //    if (!BackTrace(ref n, stream))
                //    {
                //        isClosed = true;
                //        return false;
                //    }

                //    for (var i = n + 1; i < nodes.Length; i++)
                //    {
                //        nodes[i].Clear();
                //    }
                //}

                //while (n < nodes.Length)
                //{
                //    offsets.Push(stream.Offset);
                //    var rst = nodes[n].Parse(grammar, stream);
                //    if (rst)
                //    {
                //        n++;
                //        continue;
                //    }

                //    offsets.Pop();
                //    n--;
                //    if (!BackTrace(ref n, stream))
                //    {
                //        isClosed = true;
                //        return false;
                //    }

                //    for (var i = n + 1; i < nodes.Length; i++)
                //    {
                //        nodes[i].Clear();
                //    }
                //}

                return true;
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

            private ParseNode CreateNode(Grammar grammar, int i)
            {
                var name = path[i];
                if (name == null)
                {
                    return new EOFNode();
                }

                if (grammar.TryParseNonTerminal(name, out var symbols))
                {
                    return new NonTerminalNode(name, symbols);
                }

                if (grammar.TryParseTerminal(name, out var terminal))
                {
                    return new TerminalNode(name, terminal);
                }

                Debug.Assert(false);
                return null;
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

        }
    }
}