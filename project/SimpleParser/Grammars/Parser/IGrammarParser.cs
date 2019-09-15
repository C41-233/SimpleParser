
using System.Collections.Generic;

namespace SimpleParser.Grammars.Parser
{
    public interface IGrammarParser
    {

        void Parse(IEnumerable<Token> tokens, IASTVisitor visitor);

    }

}
