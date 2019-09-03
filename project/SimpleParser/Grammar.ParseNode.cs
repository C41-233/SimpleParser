using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleParser
{
    public partial class Grammar
    {
        private abstract class ParseNode
        {
            protected string Name { get; }
            public abstract void Clear();
            public abstract bool IsClosed { get; }

            protected ParseNode(string name)
            {
                Name = name;
            }

            public bool Parse(Grammar grammar, TokenStream stream)
            {
                var offset = stream.Offset;
                var rst = DoParse(grammar, stream);
                if (!rst)
                {
                    stream.Reset(offset);
                }
                return rst;
            }

            protected abstract bool DoParse(Grammar grammar, TokenStream stream);

        }

        private class NonTerminalNode : ParseNode
        {

            private readonly List<string[]> symbols;

            private int index;
            private List<NonTerminalPath> paths;

            public NonTerminalNode(string name, List<string[]> symbols) : base(name)
            {
                this.symbols = symbols;
            }

            public override void Clear()
            {
                index = 0;
                isClosed = false;
                foreach (var path in paths)
                {
                    path.Clear();
                }
            }

            public override bool IsClosed => isClosed;
            private bool isClosed;

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                if (paths == null)
                {
                    paths = new List<NonTerminalPath>(symbols.Count);
                    foreach (var path in symbols)
                    {
                        paths.Add(new NonTerminalPath(Name, path));
                    }
                }

                while (index < paths.Count)
                {
                    var path = paths[index];
                    var rst = path.Parse(grammar, stream);
                    if (path.IsClosed)
                    {
                        index++;
                    }

                    if (index >= paths.Count)
                    {
                        isClosed = true;
                    }

                    if (rst)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private class NonTerminalPath : ParseNode
        {

            private readonly string[] path;
            private List<ParseNode> nodes;
            private Stack<int> offsets = new Stack<int>();

            public NonTerminalPath(string name, string[] path) : base(name)
            {
                this.path = path;
            }

            public override void Clear()
            {
            }

            public override bool IsClosed { get; }

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                if (nodes == null)
                {
                    for (var i = 0; i < path.Length; i++)
                    {
                        nodes.Add(CreateNode(grammar, i));
                    }
                }
            }

            private ParseNode CreateNode(Grammar grammar, int i)
            {
                if (grammar.TryParseNonTerminal(path[i], out var symbols))
                {
                    return new NonTerminalNode(path[i], symbols);
                }

                if (grammar.TryParseTerminal(path[i], out var terminal))
                {
                    return new TerminalNode(path[i], terminal);
                }

                Debug.Assert(false);
                return null;
            }

        }

        private class TerminalNode : ParseNode
        {

            private readonly Terminal terminal;

            public TerminalNode(string name, Terminal terminal) : base(name)
            {
                this.terminal = terminal;
            }

            public override bool IsClosed => true;

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                var token = stream.Next();
                if (token == null)
                {
                    return false;
                }

                return terminal.Match(token);
            }

            public override void Clear()
            {
            }
        }
    }
}