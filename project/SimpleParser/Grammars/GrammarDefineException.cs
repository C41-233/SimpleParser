using System;

namespace SimpleParser.Grammars
{
    public class GrammarDefineException : Exception
    {

        public GrammarDefineException(string msg) : base(msg)
        {
        }

        internal static GrammarDefineException DuplicateTerminalDefinition(string terminal)
        {
            throw new GrammarDefineException($"duplicate terminal definition : {terminal}");
        }

        public static Exception NonTerminalRefTokenNotFound(string name, string refName)
        {
            throw new GrammarDefineException($"token {refName} in non-terminal {name} not defined");
        }
    }
}
