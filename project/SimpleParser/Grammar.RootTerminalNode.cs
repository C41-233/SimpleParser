using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private class RootTerminalNode : NonTerminalNode
        {
            public RootTerminalNode(List<string[]> symbols) : base("<root>", symbols)
            {
            }

        }
    }
}