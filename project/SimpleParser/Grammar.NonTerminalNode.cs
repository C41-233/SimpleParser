using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private class NonTerminalNode : ParseNode
        {

            private readonly List<string[]> symbols;

            private int index;
            private NonTerminalPath[] paths;

            public NonTerminalNode(string name, List<string[]> symbols) : base(name)
            {
                this.symbols = symbols;
            }

            public override bool IsTerminal => false;
            public override IEnumerable<string> Symbols => paths[index].Symbols;

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

            public override bool IsClosed => isClosed;
            private bool isClosed;

            protected override bool DoParse(Grammar grammar, TokenStream stream)
            {
                if (paths == null)
                {
                    paths = new NonTerminalPath[symbols.Count];
                    for (var i = 0; i < symbols.Count; i++)
                    {
                        paths[i] = new NonTerminalPath(Name, symbols[i]);
                    }
                }

                while (index < paths.Length)
                {
                    var path = paths[index];
                    var rst = path.Parse(grammar, stream);
                    if (path.IsClosed)
                    {
                        index++;
                    }

                    if (index >= paths.Length)
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

            public override void Visit(IASTVisitor visitor)
            {
                paths[index].Visit(visitor);
            }
        }
    }
}