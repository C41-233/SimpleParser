using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private abstract class ParseNode : IASTNode
        {
            public string Name { get; }
            public abstract bool IsTerminal { get; }
            public abstract IEnumerable<string> Symbols { get; }
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

            public abstract void Visit(IASTVisitor visitor);
        }
    }
}