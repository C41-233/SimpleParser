using System.Collections.Generic;

namespace SimpleParser.Grammars.Parser.LLn.Nodes
{

    internal abstract class ParseNode
    {

        public abstract void Clear();
        public abstract bool IsClosed { get; }

        public abstract IEnumerator<ParseReport> Generate(LLnParser.ParserContext ctx, TokenStream stream);

        public abstract void Visit(IASTVisitor visitor);
    }

}
