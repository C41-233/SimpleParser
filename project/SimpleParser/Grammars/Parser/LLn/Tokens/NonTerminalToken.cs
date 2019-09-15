using System.Collections.Generic;
using SimpleParser.Grammars.Parser.LLn.Nodes;

namespace SimpleParser.Grammars.Parser.LLn.Tokens
{
    internal class NonTerminalToken : LLnToken
    {
        public NonTerminalToken(string name, int count) : base(name)
        {
            paths = new List<LLnToken[]>(count);
            ids = new List<int>(count);
        }

        private readonly List<LLnToken[]> paths;
        private readonly List<int> ids;

        public void AddPath(int id, LLnToken[] path)
        {
            ids.Add(id);
            paths.Add(path);
        }

        public LLnToken[] Path(int n) => paths[n];
        public int Id(int n) => ids[n];

        public int Count => paths.Count;

        public override ParseNode CreateNode()
        {
            return new NonTerminalNode(this);
        }

    }
}
