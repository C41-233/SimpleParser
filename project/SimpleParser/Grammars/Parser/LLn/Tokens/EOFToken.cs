using SimpleParser.Grammars.Parser.LLn.Nodes;

namespace SimpleParser.Grammars.Parser.LLn.Tokens
{
    internal class EOFToken : LLnToken
    {

        internal static readonly EOFToken Instance = new EOFToken();

        private EOFToken() : base("$eof")
        {
        }

        public override ParseNode CreateNode()
        {
            return new EOFNode();
        }
    }
}
