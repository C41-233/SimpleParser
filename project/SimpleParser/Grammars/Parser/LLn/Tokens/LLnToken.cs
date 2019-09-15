using SimpleParser.Grammars.Parser.LLn.Nodes;

namespace SimpleParser.Grammars.Parser.LLn.Tokens
{

    internal abstract class LLnToken
    {

        public string Name { get; }

        protected LLnToken(string name)
        {
            Name = name;
        }

        public abstract ParseNode CreateNode();
    }

}
