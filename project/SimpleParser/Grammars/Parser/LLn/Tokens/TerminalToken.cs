using System.Text.RegularExpressions;
using SimpleParser.Grammars.Parser.LLn.Nodes;

namespace SimpleParser.Grammars.Parser.LLn.Tokens
{

    internal class TerminalToken : LLnToken
    {

        public int Id { get; }
        public bool IsExplicit { get; }
        private readonly int token;
        private readonly string pattern;

        public TerminalToken(int id, bool isExplicit, string name, int token, string pattern) : base(name)
        {
            Id = id;
            IsExplicit = isExplicit;
            this.token = token;
            this.pattern = pattern;
        }

        public override ParseNode CreateNode()
        {
            return new TerminalNode(this);
        }

        public bool Match(Token t)
        {
            if (t.Type != token)
            {
                return false;
            }

            if (pattern != null && !Regex.IsMatch(t.Value, pattern))
            {
                return false;
            }

            return true;
        }

    }

}
